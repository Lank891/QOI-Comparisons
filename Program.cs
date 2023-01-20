using QOIComarisonImprovement;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows10.0.17763")]
internal class Program
{
    private static void Main(string[] args)
    {

        if (args.Length < 1)
        {
            Console.WriteLine("Provide arguments: path(s) to images to test (standard formats like bmp, jpeg or png are supported).");
            return;
        }

        foreach(string path in args)
        {
            ProcessArgument(path);
            Console.WriteLine("");
        }

        Console.WriteLine("Finished.");
        Console.Read();
    }

    private static void ProcessArgument(string path)
    {
        string fileName = Path.GetFileName(path);
        
        string testPath = Path.GetDirectoryName(path)+"/test.txt";

        var (rawPixels, width, height) = ImageAlgorithm.LoadBmp(path);

        if (!File.Exists(testPath))
        {
            File.Create(testPath).Dispose();
            using (TextWriter tw = new StreamWriter(testPath))
            {
                tw.WriteLine("File\t Size(bytes)\t Dimensions\t Type\t CompressionTime(ms)\t DecompressionTime(ms)\t SizeAfterCompression(bytes)\t CompressionRate\n");
            }
        }



        Console.WriteLine($"File: {fileName, 35}; Size: {rawPixels.Length,10} bytes; Dimensions: {width}x{height}");

        // Default
        PerformMeasurement(new PNG(), fileName,testPath, rawPixels, width, height);

        // Lossy compression especially good on real life images
        PerformMeasurement(new JPEG(), fileName, testPath, rawPixels, width, height);

        // Potentially better than png and jpg with much worse compression time
        PerformMeasurement(new Webp(), fileName, testPath, rawPixels, width, height);

        // Default qoi
        PerformMeasurement(new QOI(), fileName, testPath, rawPixels, width, height);

        // Additional LZ4 compression - generally worse than others in LZ family, but much faster
        PerformMeasurement(new QOILZ4(), fileName, testPath, rawPixels, width, height);

        // Additional dictionary (+ entropy) compression - LZ77 + Huffman
        PerformMeasurement(new QOIDeflate(), fileName, testPath, rawPixels, width, height);

        // Additional LZMA compression - better and slower LZ77-like used in 7-zip
        PerformMeasurement(new QOILZMA(), fileName, testPath, rawPixels, width, height);
    }

    private static void PerformMeasurement(ImageAlgorithm algorithm, string fileName, string testPath,  byte[] rawPixels, int width, int height)
    {
        var (compressTime, decompressTime, compressedData) = algorithm.MeasureTime(rawPixels, width, height, 3);

        double compressionRate = (double)rawPixels.Length/(double)compressedData.Length;
        using (TextWriter tw = new StreamWriter(testPath, true))
        {
            tw.WriteLine($"{fileName,35}\t{rawPixels.Length,10}\t{width}x{height}\t{algorithm.Name,15}\t{Math.Round(compressTime.TotalMilliseconds, 3),7}\t{Math.Round(decompressTime.TotalMilliseconds,3),7}\t{compressedData.Length,10}\t{Math.Round(compressionRate, 3),7}");
        }
        Console.WriteLine($"\t{"[" + algorithm.Name + "]",15}: Compress: {Math.Round(compressTime.TotalMilliseconds, 3),7} ms; Decompress: {Math.Round(decompressTime.TotalMilliseconds,3),7} ms; Size after compression: {compressedData.Length,10} bytes; Compression rate: {Math.Round(compressionRate,3),7}");
    }
}