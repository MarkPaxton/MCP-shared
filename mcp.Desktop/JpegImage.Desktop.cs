using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SensorShare.Desktop
{
   public static class JpegImage
   {
      public static Bitmap LoadFromBytes(byte[] bytes)
      {
         Bitmap bmp = new Bitmap(0,0);
         using (MemoryStream st = new MemoryStream(bytes, 0, bytes.Length, false, true))
         {
            bmp = new Bitmap(st);
         }
         return bmp;
      }

      private static bool target()
      { return false; }

      public static Bitmap GetThumbnail(Bitmap bmpImage, Size size)
      {
         double width = bmpImage.Width;
         double height = bmpImage.Height;

         Double xFactor = new Double();
         Double yFactor = new Double();
         xFactor = (double)size.Width / width;
         yFactor = (double)size.Height / height;

         Double scaleFactor = Math.Min(xFactor, yFactor);

         Size scaledSize = new Size();
         scaledSize.Width = Convert.ToInt32(width * scaleFactor);
         scaledSize.Height = Convert.ToInt32(height * scaleFactor);

         Bitmap scaledOriginal = (Bitmap) bmpImage.GetThumbnailImage(scaledSize.Width, scaledSize.Height, new Image.GetThumbnailImageAbort(target), IntPtr.Zero);
         Bitmap thumnailBmp = new Bitmap(size.Width, size.Height);

         for (int y = 0; y < thumnailBmp.Height; y++)
         {
            for (int x = 0; x < thumnailBmp.Width; x++)
            {
               if ((x < scaledOriginal.Width) && (y < scaledOriginal.Height))
               {
                  thumnailBmp.SetPixel(x, y, scaledOriginal.GetPixel(x, y));
               }
               else
               {
                  thumnailBmp.SetPixel(x, y, Color.Transparent);
               }
            }
         }
         return thumnailBmp;
      }

      public static byte[] GetBytes(Bitmap image)
      {
         byte[] toReturn;

         using (MemoryStream ms = new MemoryStream())
         {
            image.Save(ms, ImageFormat.Jpeg);
            toReturn = ms.ToArray();
         }
         return toReturn;
      }

   }
}

