using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FacesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {
        private readonly AzureFaceConfiguration Configuration;

        public FacesController(AzureFaceConfiguration config)
        {
            Configuration = config;
        }

        [HttpPost]
        public async Task<Tuple<List<byte[]>, Guid>> ReadFaces(Guid orderId)
        {
            List<byte[]> facesCropped = null;

            using (var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                byte[] bytes = ms.ToArray();
                Image img = Image.Load(bytes);
                img.Save("dummy.jpg");

                facesCropped = await UploadAndDetectFaces(img, new MemoryStream(bytes));
            }

            return new Tuple<List<byte[]>, Guid>(facesCropped, orderId);
        }


        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        private async Task<List<byte[]>> UploadAndDetectFaces(Image image1, MemoryStream imageStream)
        {

            // Convert the byte array into jpeg image and Save the image coming from the source
            //in the root directory for testing purposes. 

            string subKey = Configuration.AzureSubscriptionKey;
            string endPoint = Configuration.AzureEndPoint;

            var client = Authenticate(endPoint, subKey);
            var faceList = new List<byte[]>();

            IList<DetectedFace> faces = null;
            try
            {
                //using (var ms1 = new FileStream("dummy.jpg", FileMode.Open))
                //{
                faces = await client.Face.DetectWithStreamAsync(imageStream, true, false, null);
                //}
                int j = 0;

                foreach (var face in faces)
                {
                    var s = new MemoryStream();
                    var zoom = 1.0;
                    int h = (int)(face.FaceRectangle.Height / zoom);
                    int w = (int)(face.FaceRectangle.Width / zoom);
                    int x = (int)(face.FaceRectangle.Left);
                    int y = (int)(face.FaceRectangle.Top);

                    image1.Clone(ctx => ctx.Crop(new Rectangle(x, y, w, h))).Save("face" + j + ".jpg");
                    image1.Clone(ctx => ctx.Crop(new Rectangle(x, y, w, h))).SaveAsJpeg(s);
                    faceList.Add(s.ToArray());

                    j++;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            return faceList;
        }

        //[HttpPost]
        //public async Task<Tuple<List<byte[]>, Guid>> ReadFaces(Guid orderId)
        //{
        //    using (var ms = new MemoryStream(2048))
        //    {
        //        await Request.Body.CopyToAsync(ms);
        //        var faces = GetFaces(ms.ToArray());
        //        return new Tuple<List<byte[]>, Guid>(faces, orderId);
        //    }
        //}

        //private List<byte[]> GetFaces(byte[] image)
        //{
        //    Mat src = Cv2.ImDecode(image, ImreadModes.Color);
        //    src.SaveImage("image.jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));

        //    var file = Path.Combine(Directory.GetCurrentDirectory(), "CascadeFile", "haarcascade_frontalface_default.xml");

        //    var faceCascade = new CascadeClassifier();
        //    faceCascade.Load(file);

        //    var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionType.DoRoughSearch, new Size(60, 60));
        //    var faceList = new List<byte[]>();

        //    int j = 0;

        //    foreach (var rect in faces)
        //    {
        //        var faceImage = new Mat(src, rect);
        //        faceList.Add(faceImage.ToBytes(".jpg"));
        //        faceImage.SaveImage("face" + j + ".jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
        //        j++;
        //    }

        //    return faceList;
        //}
    }
}