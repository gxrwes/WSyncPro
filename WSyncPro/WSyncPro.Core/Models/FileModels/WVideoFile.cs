using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Models.FileModels
{
    public class WVideoFile : WFile
    {
        public WVideoFile()
        {
            FileType = FileType.Video;
        }
        public TimeOnly Length { get; set; }
        public Resolution Resolution { get; set; }

        public void SetResolutionFromString(string value)
        {
            // Normalize the input string to lowercase and remove 'p', 'k', 'mp', or 'r' characters
            string normalizedValue = value.ToLower().Replace("p", "").Replace("k", "").Replace("mp", "").Replace("r", "").Trim();

            switch (normalizedValue)
            {
                case "240":
                    Resolution = Resolution.P240;
                    break;
                case "360":
                    Resolution = Resolution.P360;
                    break;
                case "480":
                    Resolution = Resolution.P480;
                    break;
                case "720":
                    Resolution = Resolution.P720;
                    break;
                case "1080":
                    Resolution = Resolution.HD1080;
                    break;
                case "1440":
                    Resolution = Resolution.HD1440;
                    break;
                case "2160":
                    Resolution = Resolution.UHD2160; // 4K
                    break;
                case "4320":
                    Resolution = Resolution.UHD4320; // 8K
                    break;
                case "8640":
                    Resolution = Resolution.UHD8640; // 16K
                    break;
                case "17280":
                    Resolution = Resolution.UHD17280; // 32K
                    break;
                case "34560":
                    Resolution = Resolution.UHD34560; // 64K
                    break;
                case "1":
                case "1280x720":
                    Resolution = Resolution.R_1MP; // 1 Megapixel
                    break;
                case "2":
                case "1920x1080":
                    Resolution = Resolution.R_2MP; // 2 Megapixel
                    break;
                case "4":
                case "2560x1440":
                    Resolution = Resolution.R_4MP; // 4 Megapixel
                    break;
                case "6":
                case "3072x2048":
                    Resolution = Resolution.R_6MP; // 6 Megapixel
                    break;
                case "8":
                case "3264x2448":
                    Resolution = Resolution.R_8MP; // 8 Megapixel
                    break;
                case "12":
                case "4000x3000":
                    Resolution = Resolution.R_12MP; // 12 Megapixel
                    break;
                case "16":
                case "4920x3264":
                    Resolution = Resolution.R_16MP; // 16 Megapixel
                    break;
                case "24":
                case "6000x4000":
                    Resolution = Resolution.R_24MP; // 24 Megapixel
                    break;
                case "36":
                case "7360x4912":
                    Resolution = Resolution.R_36MP; // 36 Megapixel
                    break;
                case "48":
                case "8000x6000":
                    Resolution = Resolution.R_48MP; // 48 Megapixel
                    break;
                default:
                    Resolution = Resolution.CUSTOM;
                    break;
            }
        }


    }

    public enum Resolution
    {
        CUSTOM,
        P240,
        P360,
        P480,
        P720,
        HD1080,
        HD1440,
        UHD2160,  // 4K
        UHD4320,  // 8K
        UHD8640,  // 16K
        UHD17280, // 32K
        UHD34560, // 64K
        R_1MP,    // 1280 x 720
        R_2MP,    // 1920 x 1080
        R_4MP,    // 2560 x 1440
        R_6MP,    // 3072 x 2048
        R_8MP,    // 3264 x 2448
        R_12MP,   // 4000 x 3000
        R_16MP,   // 4920 x 3264
        R_24MP,   // 6000 x 4000
        R_36MP,   // 7360 x 4912
        R_48MP    // 8000 x 6000
    }


}
