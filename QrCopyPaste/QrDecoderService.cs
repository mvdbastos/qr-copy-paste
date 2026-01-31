using System.Drawing;
using ZXing;
using ZXing.Common;

namespace QrCopyPaste;

/// <summary>
/// Service for decoding QR codes from images.
/// </summary>
public class QrDecoderService
{
    private readonly BarcodeReader reader;

    public QrDecoderService()
    {
        reader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
            }
        };
    }

    /// <summary>
    /// Attempts to decode a QR code from the given bitmap.
    /// </summary>
    /// <param name="bitmap">The image to decode.</param>
    /// <returns>The decoded text, or null if no QR code was found.</returns>
    public string? DecodeQrCode(Bitmap bitmap)
    {
        try
        {
            var result = reader.Decode(bitmap);
            return result?.Text;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to decode multiple QR codes from the given bitmap.
    /// </summary>
    public List<string> DecodeMultipleQrCodes(Bitmap bitmap)
    {
        var results = new List<string>();
        try
        {
            var multiReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                }
            };

            var result = multiReader.DecodeMultiple(bitmap);
            if (result != null)
            {
                foreach (var res in result)
                {
                    if (!string.IsNullOrEmpty(res.Text))
                    {
                        results.Add(res.Text);
                    }
                }
            }
        }
        catch
        {
            // Return empty list on error
        }
        return results;
    }

    /// <summary>
    /// Checks if a bitmap contains a QR code of minimum size.
    /// </summary>
    public bool ContainsQrCode(Bitmap bitmap, int minSize)
    {
        try
        {
            var result = reader.Decode(bitmap);
            if (result == null)
                return false;

            // Check if QR code is large enough
            var points = result.ResultPoints;
            if (points == null || points.Length < 2)
                return false;

            var width = Math.Abs(points[0].X - points[1].X);
            var height = Math.Abs(points[0].Y - points[1].Y);
            var size = Math.Max(width, height);

            return size >= minSize;
        }
        catch
        {
            return false;
        }
    }
}
