using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersApi.Hubs;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Services;
using System;

namespace OrdersApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrdersContext>(options => options.UseSqlServer
            (
                Configuration.GetConnectionString("OrdersContextConnection")
            ));

            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddHttpClient();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddMassTransit(
                c =>
                {
                    c.AddConsumer<RegisterOrderCommandConsumer>();
                    c.AddConsumer<OrderDispatchedEventConsumer>();
                });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(
                cfg =>
                {
                    var host = cfg.Host("localhost", "/", h => { });

                    cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                        e.Consumer<RegisterOrderCommandConsumer>(provider);
                    });

                    cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, 100));


                        e.Consumer<OrderDispatchedEventConsumer>(provider);
                        //  EndpointConvention.Map<OrderDispatchedEvent>(e.InputAddress);
                    });

                    cfg.ConfigureEndpoints(provider);
                }));

            services.AddSingleton<IHostedService, BusService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials());
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderHub>("/orderhub");
            });
        }
    }
}