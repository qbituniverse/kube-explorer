using System;
using System.Collections.Generic;
using qu.nuget.azure.Dto;

namespace qu.nuget.azure.Models
{
    public class ImagesCatalog : Dictionary<string, List<Image>>
    {
    }

    public class Image
    {
        public string tag { get; set; } = string.Empty;
        public string digest { get; set; } = string.Empty;
        public DateTime created { get; set; }
        public string architecture { get; set; } = string.Empty;
        public ImageDto.Labels labels { get; set; } = new ImageDto.Labels();
    } 
}