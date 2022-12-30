using SevenZip;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    internal class QOILZMA : QOI
    {
        public override string Name => "QOI + LZMA";

        public override byte[] Compress(byte[] rawPixels, int width, int height)
        {
            byte[] qoi = base.Compress(rawPixels, width, height);
            using var qoiStream = new MemoryStream(qoi);
            using var output = new MemoryStream();

            Helper.Compress(qoiStream, output, Helper.LzmaSpeed.Fastest, Helper.DictionarySize.VerySmall);

            return output.ToArray();
        }

        public override byte[] Decompress(byte[] compressedData, int width, int height)
        {
            using var output = new MemoryStream();
            using var compressedStream = new MemoryStream(compressedData);

            Helper.Decompress(compressedStream, output);

            byte[] qoi = output.ToArray();
            return base.Decompress(qoi, width, height);
        }

        // https://gist.github.com/ststeiger/cb9750664952f775a341
        private class Helper
        {
            public enum LzmaSpeed : int
            {
                Fastest = 5,
                VeryFast = 8,
                Fast = 16,
                Medium = 32,
                Slow = 64,
                VerySlow = 128,
            }

            public enum DictionarySize : int
            {
                ///<summary>64 KiB</summary>
                VerySmall = 1 << 16,
                ///<summary>1 MiB</summary>
                Small = 1 << 20,
                ///<summary>4 MiB</summary>
                Medium = 1 << 22,
                ///<summary>8 MiB</summary>
                Large = 1 << 23,
                ///<summary>16 MiB</summary>
                Larger = 1 << 24,
                ///<summary>64 MiB</summary>
                VeryLarge = 1 << 26,
            }

            public static void Compress(Stream input, Stream output, LzmaSpeed speed = LzmaSpeed.Fastest, DictionarySize dictionarySize = DictionarySize.VerySmall)
            {
                int posStateBits = 2; // default: 2
                int litContextBits = 3; // 3 for normal files, 0; for 32-bit data
                int litPosBits = 0; // 0 for 64-bit data, 2 for 32-bit.
                var numFastBytes = (int)speed;
                string matchFinder = "BT4"; // default: BT4
                bool endMarker = true;

                CoderPropID[] propIDs =
                {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits, // (0 <= x <= 4).
                    CoderPropID.LitContextBits, // (0 <= x <= 8).
                    CoderPropID.LitPosBits, // (0 <= x <= 4).
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder, // "BT2", "BT4".
                    CoderPropID.EndMarker
                };

                object[] properties =
                {
                    (int)dictionarySize,
                    posStateBits,
                    (int)litContextBits,
                    (int)litPosBits,
                    numFastBytes,
                    matchFinder,
                    endMarker
                };

                var lzmaEncoder = new SevenZip.Compression.LZMA.Encoder();

                lzmaEncoder.SetCoderProperties(propIDs, properties);
                lzmaEncoder.WriteCoderProperties(output);
                var fileSize = input.Length;
                for (int i = 0; i < 8; i++) output.WriteByte((byte)(fileSize >> (8 * i)));

                ICodeProgress? prg = null;
                lzmaEncoder.Code(input, output, -1, -1, prg);
            }

            public static void Decompress(Stream input, Stream output)
            {
                var decoder = new SevenZip.Compression.LZMA.Decoder();

                byte[] properties = new byte[5];
                if (input.Read(properties, 0, 5) != 5)
                {
                    throw new Exception("input .lzma is too short");
                }
                decoder.SetDecoderProperties(properties);

                long fileLength = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = input.ReadByte();
                    if (v < 0) throw new Exception("Can't Read 1");
                    fileLength |= ((long)(byte)v) << (8 * i);
                }

                ICodeProgress? prg = null;
                long compressedSize = input.Length - input.Position;

                decoder.Code(input, output, compressedSize, fileLength, prg);
            }
        }
    }
}
