using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    [SupportedOSPlatform("windows")]
    internal abstract class ImageAlgorithm
    {
        public abstract string Name { get; }
        protected static PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        protected virtual bool LossyCompression => false;
        
        public static (byte[] bytes, int w, int h) LoadBmp(string bmpPath)
        {
            Bitmap readBitmap = new(bmpPath);
            Bitmap bitmap = new(readBitmap.Width, readBitmap.Height, PixelFormat);
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.DrawImage(readBitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }
            readBitmap.Dispose();

            Rectangle bitmapRect = new(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;
            byte[] rawImageData = new byte[length];
            Marshal.Copy(bitmapData.Scan0, rawImageData, 0, length);
            bitmap.UnlockBits(bitmapData);
            int w = bitmap.Width;
            int h = bitmap.Height;
            bitmap.Dispose();
            return (rawImageData, w, h);
        }

        public abstract byte[] Compress(byte[] rawPixels, int width, int height);
        public abstract byte[] Decompress(byte[] compressedData, int width, int height);

        public (TimeSpan compress, TimeSpan decompress, byte[] compressedData) MeasureTime(byte[] rawPixels, int width, int height, uint repetitions)
        {
            Stopwatch sw = new();
            TimeSpan[] compressTimes = new TimeSpan[repetitions];
            TimeSpan[] decompressTimes = new TimeSpan[repetitions];

            byte[] compressedData = Array.Empty<byte>();
            for(int i = 0; i < repetitions; i++)
            {
                sw.Start();
                compressedData = Compress(rawPixels, width, height);
                sw.Stop();
                compressTimes[i] = sw.Elapsed;
                sw.Reset();
            }

            byte[] decompressedData = Array.Empty<byte>();
            for (int i = 0; i < repetitions; i++)
            {
                sw.Start();
                decompressedData = Decompress(compressedData, width, height);
                sw.Stop();
                decompressTimes[i] = sw.Elapsed;
                sw.Reset();
            }

            if(!LossyCompression)
            {
                string errorMessage = $"[{Name}]: Decompression yielded different data than original!";
                if (decompressedData.Length != rawPixels.Length)
                    throw new Exception(errorMessage);
                for (int i = 0; i < decompressedData.Length; i++)
                {
                    if (rawPixels[i] != decompressedData[i])
                        throw new Exception(errorMessage);
                }
            }
            

            return (MeanTimeSpan(compressTimes), MeanTimeSpan(decompressTimes), compressedData);
        }

        private static TimeSpan MeanTimeSpan(ICollection<TimeSpan> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            long mean = 0L;
            long remainder = 0L;
            int n = source.Count;
            foreach (var item in source)
            {
                long ticks = item.Ticks;
                mean += ticks / n;
                remainder += ticks % n;
                mean += remainder / n;
                remainder %= n;
            }

            return TimeSpan.FromTicks(mean);
        }
    }
}
