using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared
{
    public class Utilities
    {

        public static Image ResizeImage(int maxWidth, int maxHeight, Image imgPhoto)
        {

            int originalWidth = imgPhoto.Width;
            int originalHeight = imgPhoto.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            float sourceRatio = (float)originalWidth / originalHeight;

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(imgPhoto, 0, 0, newWidth, newHeight);
            }

            return newImage;


            //int sourceWidth = imgPhoto.Width;
            //int sourceHeight = imgPhoto.Height;

            ////if (sourceWidth < sourceHeight)
            ////{
            ////    int buff = newWidth;

            ////    newWidth = newHeight;
            ////    newHeight = buff;
            ////}

            //int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            //float nPercent = 0, nPercentW = 0, nPercentH = 0;

            //nPercentW = ((float)newWidth / (float)sourceWidth);
            //nPercentH = ((float)newHeight / (float)sourceHeight);
            //if (nPercentH < nPercentW)
            //{
            //    nPercent = nPercentH;
            //    destX = (int)((newWidth - (sourceWidth * nPercent)) / 2);
            //    newHeight = (int)(nPercentW * sourceHeight);
            //}
            //else
            //{
            //    nPercent = nPercentW;
            //    destY = (int)((newHeight -
            //              (sourceHeight * nPercent)) / 2);
            //     newWidth  = (int)(nPercentH * sourceWidth);
            //}

            //int destWidth = (int)(sourceWidth * nPercent);
            //int destHeight = (int)(sourceHeight * nPercent);


            //Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
            //              PixelFormat.Format24bppRgb);

            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //             imgPhoto.VerticalResolution);

            //Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.Clear(Color.Black);
            ////grPhoto.InterpolationMode =
            ////    System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //grPhoto.DrawImage(imgPhoto,
            //    new Rectangle(0, 0, sourceWidth, sourceHeight),
            //    new Rectangle(sourceX, sourceY, sourceWidth, sourceWidth),
            //    GraphicsUnit.Pixel);

            //grPhoto.Dispose();
            //imgPhoto.Dispose();
            //return bmPhoto;
        }

        public static string GetThumbnailPathImage(string path)
        {
            var parts = path.Split("\\").ToList();
            string thumbnailPath = "";

            for (int i = 0; i < parts.Count; i++)
            {

                thumbnailPath += parts[i];

                if (i != parts.Count - 1)
                {
                    thumbnailPath += "\\";
                }

                if (i == parts.Count - 2)
                {
                    thumbnailPath += "Thumbnail\\";
                }
            }

            return thumbnailPath;
        }


        public static string TimeFormatFromMinutes(int minutes)
        {
            int hour = minutes / 60;
            int remainingMinutes = minutes - (hour * 60);
            return hour.ToString().PadLeft(2, '0') + ":" + remainingMinutes.ToString().PadLeft(2, '0');
        }

        public static string  ToTurkishDayOfWeek(string dayOfWeek)
        {
            if (dayOfWeek == "Sunday") return "Pazar";
            if (dayOfWeek == "Monday") return "Pazartesi";
            if (dayOfWeek == "Tuesday") return "Salı";
            if (dayOfWeek == "Wednesday") return "Çarşamba";
            if (dayOfWeek == "Thursday") return "Perşembe";
            if (dayOfWeek == "Friday") return "Cuma";
            if (dayOfWeek == "Saturday") return "Cumartesi";

            return "";
        }
    }
}
