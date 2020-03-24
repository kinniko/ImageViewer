﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Models.Settings
{
    public class ImagesConfig
    {
        public class ImageData
        {
            public string Filename { get; set; }
            public string Alias { get; set; }
        }

        public List<ImageData> Images { get; set; } = new List<ImageData>();

        public static ImagesConfig LoadFromModels(ModelsEx models)
        {
            var res = new ImagesConfig();
            foreach (var img in models.Images.Images)
            {
                res.Images.Add(new ImageData
                {
                    Filename = img.Filename,
                    Alias = img.Alias
                });
            }

            return res;
        }
    }
}
