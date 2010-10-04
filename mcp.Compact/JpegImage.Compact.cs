using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenNETCF.Drawing.Imaging;

namespace mcp.Compact
{
   public class JpegImage
   {
      private Bitmap bmpImage;
      private static ImagingFactoryClass imageFactory = new ImagingFactoryClass();

      public JpegImage()
      {
      }

      public void LoadFromFile(string filename)
      {
         bmpImage = new Bitmap(filename);
      }


      public void LoadFromStream(Stream st)
      {
         bmpImage = new Bitmap(st);
      }

      public static Bitmap LoadFromBytes(byte[] bytes)
      {
         Bitmap bmp = new Bitmap(1,1);
         using (MemoryStream st = new MemoryStream(bytes, 0, bytes.Length, false, true))
         {
            bmp = new Bitmap(st);
         }
         return bmp;
      }

      public void LoadFromBytes(byte[] bytes, int offset)
      {
         this.LoadFromBytes(bytes, offset, (bytes.Length - offset));
      }

      public void LoadFromBytes(byte[] bytes, int offset, int count)
      {
         using (MemoryStream st = new MemoryStream(bytes, offset, count, false, true))
         {
            bmpImage = new Bitmap(st);
         }
      }

      public void LoadFromBitmap(Bitmap bmp)
      {
         this.bmpImage = new Bitmap(bmp);
      }

      public static Bitmap GetThumbnail(Bitmap bmpImage, Size size)
      {
         double width = bmpImage.Width;
         double height = bmpImage.Height;

         Double WidthToHeightAspect = (double)bmpImage.Width / (double)bmpImage.Height;
         
         Double xFactor = (double)size.Width / width;
         Double yFactor = (double)size.Height / height;
         Size scaledSize = new Size();
         if (xFactor < yFactor)
         {
            scaledSize.Width = size.Width;
            scaledSize.Height = Convert.ToInt32(size.Width / WidthToHeightAspect);
         }
         else
         {
            scaledSize.Width = Convert.ToInt32(size.Height * WidthToHeightAspect);
            scaledSize.Height = size.Height;
         }

         Bitmap scaledOriginal;
         Bitmap thumnailBmp = new Bitmap(size.Width, size.Height);
         // Compact framework can't resize bitmaps!
         // Using OpenNETCF instead
         using (MemoryStream ms = new MemoryStream())
         {
            ImagingFactoryClass imageFactory = new ImagingFactoryClass();

            IImage image;
            IBitmapImage iBitmap;

            ImageInfo info;

            bmpImage.Save(ms, ImageFormat.Png);
            imageFactory.CreateImageFromStream(new StreamOnFile(ms), out image);
            image.GetImageInfo(out info);
            imageFactory.CreateBitmapFromImage(image, (uint)scaledSize.Width, (uint)scaledSize.Height,
                info.PixelFormat, InterpolationHint.InterpolationHintDefault, out iBitmap);

            //Set up the scaled bitmap to be copied
            scaledOriginal = ImageUtils.IBitmapImageToBitmap(iBitmap);

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

      public static Bitmap IImageToBitmap(IImage image)
      {
         ImageInfo imageInfo = new ImageInfo();
         IBitmapImage bitmapToConvert;
         Bitmap toReturn;

         image.GetImageInfo(out imageInfo);

         Size scaledSize = new Size();
         Double xFactor = new Double();
         Double yFactor = new Double();

         xFactor = 1;
         yFactor = 1;

         scaledSize.Width = Convert.ToInt16(imageInfo.Width * Math.Min(xFactor, yFactor));
         scaledSize.Height = Convert.ToInt16(imageInfo.Height * Math.Min(xFactor, yFactor));

         imageFactory.CreateBitmapFromImage(image,
            (uint)scaledSize.Width, (uint)scaledSize.Height,
         imageInfo.PixelFormat, InterpolationHint.InterpolationHintDefault,
            out bitmapToConvert);
         toReturn = new Bitmap(scaledSize.Width, scaledSize.Height);
         toReturn = ImageUtils.IBitmapImageToBitmap(bitmapToConvert);
         return toReturn;
      }
   }
}

