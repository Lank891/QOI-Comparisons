using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    [SupportedOSPlatform("windows")]
    internal abstract class CSExistingFormat : ImageAlgorithm
    {
        public abstract ImageFormat Format { get; }

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            Bitmap bmp = new(width, height, PixelFormat);
            Rectangle bitmapRect = new(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(bitmapRect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(rawPixels, 0, bitmapData.Scan0, rawPixels.Length);
            bmp.UnlockBits(bitmapData);

            using var ms = new MemoryStream();
            bmp.Save(ms, Format);
            bmp.Dispose();
            return ms.ToArray();
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            Bitmap bmp = (Bitmap)Image.FromStream(new MemoryStream(compressedData));
            Rectangle bitmapRect = new(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(bitmapRect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;
            byte[] rawImageData = new byte[length];
            Marshal.Copy(bitmapData.Scan0, rawImageData, 0, length);
            bmp.UnlockBits(bitmapData);
            bmp.Dispose();
            return rawImageData;
        }
    }
}
