using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        /// <returns>Bitmap</returns>        
        internal static Bitmap Scale(Bitmap image, int scaleMultiplier)
        {

            int size = image.Width * scaleMultiplier;
            var imageScaled = new Bitmap(size, size);
            using (Graphics graph = Graphics.FromImage(imageScaled))
            {
                graph.InterpolationMode = InterpolationMode.NearestNeighbor;
                graph.SmoothingMode = SmoothingMode.None;
                graph.PixelOffsetMode = PixelOffsetMode.Half;
                graph.PageUnit = GraphicsUnit.Pixel;

                graph.ScaleTransform(scaleMultiplier, scaleMultiplier);
                graph.DrawImage(image, 0, 0);
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
