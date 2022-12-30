using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    internal class QOIDeflate : QOI
    {
        public override string Name => "QOI + Deflate";

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            byte[] qoi = base.Compress(rawPixels, width, height);

            using var output = new MemoryStream();
            using var qoiStream = new MemoryStream(qoi);

            using var deflate = new DeflateStream(output, CompressionMode.Compress);
            
            qoiStream.CopyTo(deflate);
            deflate.Close();
            byte[] deflated = output.ToArray();
            return deflated;
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            using var output = new MemoryStream();
            using var compressedStream = new MemoryStream(compressedData);

            using var deflate = new DeflateStream(compressedStream, CompressionMode.Decompress);

            deflate.CopyTo(output);
            deflate.Close();
            byte[] qoi = output.ToArray();
            return base.Decompress(qoi, width, height);
        }
    }
}
