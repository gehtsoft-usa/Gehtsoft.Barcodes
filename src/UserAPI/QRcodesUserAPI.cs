using System;
using SkiaSharp;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Rendering;
using Gehtsoft.Barcodes.Utils;
// ReSharper disable InconsistentNaming

namespace Gehtsoft.Barcodes.UserAPI
{
    /// <summary>
    /// Provides methods for creating barcodes and QR codes.
    /// </summary>
    public static partial class BarcodesMaker
    {
        /// <summary>
        /// Generates a QR code with specified error correction level, version, and pixel scaling from the input data and returns the QR code image as an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="version">The QR code version.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <returns>byte[]</returns>
        public static byte[] GetQRCode(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, QRCodeVersion version, int scaleMultiplier = 1)
        {
            return GetQRCode(data, encoding, levelCorrection, version, scaleMultiplier, SKColors.Black, SKColors.White);
        }

        /// <summary>
        /// Generates a QR code with specified error correction level, version, pixel scaling, color, background color, and quiet zone from the input data and returns the QR code image as an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="version">The QR code version.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="hasQuietZones">Defines whether the QR code has a "quiet zone".</param>
        /// <returns>byte[]</returns>
        public static byte[] GetQRCode(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, QRCodeVersion version, int scaleMultiplier, SKColor foregroundColor, SKColor backgroundColor, bool hasQuietZones = true)
        {
            // Validation
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(nameof(data));
            if (!Enum.IsDefined(typeof(QRCodeEncodingMethod), encoding))
                throw new ArgumentException(nameof(encoding));
            if (!Enum.IsDefined(typeof(QRCodeErrorCorrection), levelCorrection))
                throw new ArgumentException(nameof(levelCorrection));
            if (!Enum.IsDefined(typeof(QRCodeVersion), version))
                throw new ArgumentOutOfRangeException(nameof(version));
            if (scaleMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(scaleMultiplier));
            // Results
            return QRCodesRenderer.GetQRCodeImage(data, encoding, levelCorrection, version, scaleMultiplier, foregroundColor, backgroundColor, hasQuietZones ? 4 : 0);
        }

        /// <summary>
        /// Generates a QR code with the automatically defined version and specified error correction level, pixel scaling, color, background color, and quiet zone from the input data and returns the QR code image as an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="hasQuietZones">Defines whether the QR code has a "quiet zone".</param>
        /// <returns>byte[]</returns>
        public static byte[] GetQRCode(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, int scaleMultiplier, SKColor foregroundColor, SKColor backgroundColor, bool hasQuietZones = true)
        {
            return GetQRCode(data, encoding, levelCorrection,
                QRCodesUtils.GetMinVersionForBinaryMode(data, levelCorrection),
                scaleMultiplier,
                foregroundColor, backgroundColor,
                hasQuietZones);
        }

        /// <summary>
        /// Generates a QR code with the automatically defined version and specified error correction level and pixel scaling from the input data and returns the QR code image as an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <returns>byte[]</returns>
        public static byte[] GetQRCode(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, int scaleMultiplier = 1)
        {
            return GetQRCode(data, encoding, levelCorrection, scaleMultiplier, SKColors.Black, SKColors.White);
        }
    }
}
