using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Gehtsoft.Barcodes.Utils
{
    /// <summary>

    /// Provides methods used in the QR codes logic.
    /// </summary>
    internal static class QRCodesUtils
    {
        /// <summary>

        /// Scales pixels of the barcode image.
        /// </summary>
        /// <param name="image">The bitmap image of the barcode.</param>
        /// <param name="scaleMultiplier">The pixel scaling factor.</param>
        /// <returns>SKBitmap</returns>
        internal static SKBitmap Scale(SKBitmap image, int scaleMultiplier)
        {
            int size = image.Width * scaleMultiplier;
            var imageScaled = new SKBitmap(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(imageScaled))
            using (var paint = new SKPaint())
            {
                paint.FilterQuality = SKFilterQuality.None; // Nearest neighbor
                paint.IsAntialias = false;

                canvas.Scale(scaleMultiplier, scaleMultiplier);
                canvas.DrawBitmap(image, 0, 0, paint);
            }
            return imageScaled;
        }

        /// <summary>
        /// Gets the version number for the QR code encoding.
        /// </summary>

        /// <param name="textUTF8">The user unicode string of UTF-8.</param>
        /// <param name="errorCorrection">The QR code error correction level.</param>
        /// <returns>int</returns>
        internal static QRCodeVersion GetMinVersionForBinaryMode(string textUTF8, QRCodeErrorCorrection levelCorrection)
        {
            // Validation
            if (string.IsNullOrEmpty(textUTF8))
                throw new ArgumentException(nameof(textUTF8));
            if (!Enum.IsDefined(typeof(QRCodeErrorCorrection), levelCorrection))
                throw new ArgumentException(nameof(levelCorrection));

            for (int version = 0; version < 40; version++)
            {
                int bitsCountFullSizeOfHeader = version < 10 ? 16 : 24;
                int bitsFullSizeOfData = (new UTF8Encoding().GetBytes(textUTF8).Length << 3) +
                                         bitsCountFullSizeOfHeader;

                if (QRCodesData.QRCodeMaxLengths[version][(int)levelCorrection] >=
                    bitsFullSizeOfData)
                {
                    return (QRCodeVersion) (version + 1);
                }
            }
            throw new InvalidOperationException();
        }

        /// <summary>

        /// Gets the shifted bit array.
        /// </summary>

        /// <param name="arr">The bit array.</param>
        /// <param name="shift">The shift parameter.</param>
        /// <returns>BitArray</returns>
        internal static BitArray LeftShift(BitArray arr, int shift)
        {
            bool[] result = new bool[arr.Length];
            arr.CopyTo(result, 0);
            Array.Copy(result, 0, result, shift, arr.Length - shift);
            for (int i = 0; i < shift; i++)
                result[i] = false;
            return new BitArray(result);
        }
    }
}
