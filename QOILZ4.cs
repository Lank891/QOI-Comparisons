
using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    internal class QOILZ4 : QOI
    {
        public override string Name => "QOI + LZ4";

        private int _originalLength = 0;

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            byte[] qoi = base.Compress(rawPixels, width, height);
            _originalLength = qoi.Length;
            byte[] target = new byte[LZ4Codec.MaximumOutputSize(qoi.Length)];
            int encodedLength = LZ4Codec.Encode(qoi, target);
            return target[..encodedLength];
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            byte[] qoi = new byte[_originalLength];
            LZ4Codec.Decode(compressedData, qoi);
            return base.Decompress(qoi, width, height);
        }
    }
}
