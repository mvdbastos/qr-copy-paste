using System.Drawing;
using ZXing;
using ZXing.Common;

namespace QrCopyPaste;

/// <summary>
/// Service for decoding QR codes from images.
/// </summary>
public class QrDecoderService
{
    private readonly BarcodeReader<Bitmap> reader;

    public QrDecoderService()
    {
        reader = new BarcodeReader<Bitmap>(
            null,
            bitmap => new RGBLuminanceSource(
                BitmapToBytes(bitmap),
                bitmap.Width,
                bitmap.Height
            ),
            null
        )
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

    private static byte[] BitmapToBytes(Bitmap bitmap)
    {
        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb
        );

        try
        {
            var length = Math.Abs(bitmapData.Stride) * bitmap.Height;
            var bytes = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            return bytes;
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
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
            var multiReader = new BarcodeReader<Bitmap>(
                null,
                bmp => new RGBLuminanceSource(
                    BitmapToBytes(bmp),
                    bmp.Width,
                    bmp.Height
                ),
                null
            )
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
