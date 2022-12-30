using QOIComarisonImprovement;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows10.0.17763")]
internal class Program
{
    private static void Main(string[] args)
    {

        if (args.Length != 1)
        {
            Console.WriteLine("Provide 1 argument: path to bmp file.");
            return;
        }

        string path = args[0];
        string fileName = Path.GetFileName(path);

        var (rawPixels, width, height) = ImageAlgorithm.LoadBmp(path);

        Console.WriteLine($"File: {fileName}; Size: {rawPixels.Length,10} bytes; Dimensions: {width}x{height}");

        // Default
        PerformMeasurement(new PNG(), rawPixels, width, height);

        // Lossy compression especially good on real life images
        PerformMeasurement(new JPEG(), rawPixels, width, height);

        // Potentially better than png and jpg with much worse compression time
        PerformMeasurement(new Webp(), rawPixels, width, height);

        // Default qoi
        PerformMeasurement(new QOI(), rawPixels, width, height);

        // Additional LZ4 compression - generally worse than others in LZ family, but much faster
        PerformMeasurement(new QOILZ4(), rawPixels, width, height);

        // Additional dictionary (+ entropy) compression - LZ77 + Huffman
        PerformMeasurement(new QOIDeflate(), rawPixels, width, height);

        // Additional LZMA compression - better and slower LZ77-like used in 7-zip
        PerformMeasurement(new QOILZMA(), rawPixels, width, height);
    }

    private static void PerformMeasurement(ImageAlgorithm algorithm, byte[] rawPixels, int width, int height)
    {
        var (compressTime, decompressTime, compressedData) = algorithm.MeasureTime(rawPixels, width, height, 3);

        double compressionRate = (double)compressedData.Length / (double)rawPixels.Length * 100;
        Console.WriteLine($"{"[" + algorithm.Name + "]",15}: Compress: {Math.Round(compressTime.TotalMilliseconds, 2),7} ms; Decompress: {Math.Round(decompressTime.TotalMilliseconds),7} ms; Size after compression: {compressedData.Length,10} bytes; Compression rate: {Math.Round(compressionRate,2),5}%");
    }
}