using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QoiSharp;

namespace QOIComarisonImprovement
{
    internal class QOI : ImageAlgorithm
    {
        public override string Name => "QOI";

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            var qoiImage = new QoiImage(rawPixels, width, height, QoiSharp.Codec.Channels.RgbWithAlpha);
            return QoiEncoder.Encode(qoiImage);
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            var qoiImage = QoiDecoder.Decode(compressedData);
            return qoiImage.Data;
        }
    }
}
