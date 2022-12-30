using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    [SupportedOSPlatform("windows10.0.17763")]
    internal class Webp : ImageAlgorithm
    { 
        public override string Name => "WebP";

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            using var image = Image<Rgba32>.LoadPixelData<Rgba32>(rawPixels, width, height);
            using var ms = new MemoryStream();
            image.Save(ms, new WebpEncoder()
            {
                FileFormat = WebpFileFormatType.Lossless
            });
            return ms.ToArray();
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            Image<Rgba32> image = Image<Rgba32>.Load<Rgba32>(compressedData, new WebpDecoder());
            byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
            image.CopyPixelDataTo(pixelBytes);
            return pixelBytes;
        }
    }
}
