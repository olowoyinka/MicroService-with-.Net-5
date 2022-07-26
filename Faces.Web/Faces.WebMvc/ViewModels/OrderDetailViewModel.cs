﻿using System.ComponentModel.DataAnnotations;

namespace Faces.WebMvc.ViewModels
{
    public class OrderDetailViewModel
    {
        [Display(Name = "Order Detail Id:")]
        public int OrderDetailId { get; set; }

        public byte[] FaceData { get; set; }
        
        public string ImageString { get; set; }
    }
}