using SkiaSharp;
using System.IO;
using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Encoding;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Utils;
using System;

namespace Gehtsoft.Barcodes.Rendering
{
    /// <summary>
    /// Describes the logic of QR codes rendering.
    /// </summary>
    internal static class QRCodesRenderer
    {
        /// <summary>
        /// Renders a QR code on bitmap from the encoded data and returns an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="version">The QR code version.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="quietZone">The size of the quiet zone.</param>
        /// <returns>byte[]</returns>
        internal static byte[] GetQRCodeImage(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, QRCodeVersion version, int scaleMultiplier, SKColor foregroundColor, SKColor backgroundColor, int quietZone = 4)
        {
            if (!Enum.IsDefined(typeof(QRCodeVersion), version))
                throw new ArgumentOutOfRangeException();

            // Only the binary method is implemented
            if (encoding != QRCodeEncodingMethod.Binary)
                throw new NotImplementedException();

            int numVersion;

            if (version == QRCodeVersion.Automatic)
                numVersion = (int)QRCodesUtils.GetMinVersionForBinaryMode(data, levelCorrection);
            else
                numVersion = (int)version;

            var numberMask = 4;


            // Get the image data array
            byte[] dataBinary = QRCodesEncoder.EncodeQRCodeData(data, numVersion, levelCorrection);

            // Create a bitmap for the QR code
            using (var bitmapSource = CreateBitmap(dataBinary, numVersion, numberMask, levelCorrection,
                                            foregroundColor,
                                            backgroundColor,
                                            quietZone))
            {
                // Pixel scaling
                using (var bitmap = QRCodesUtils.Scale(bitmapSource, scaleMultiplier))
                {
                    // Save the resulting image in the PNG format
                    using (var memStream = new MemoryStream())
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        encoded.SaveTo(memStream);
                        return memStream.ToArray();
                    }
                }
            }
        }

        private static SKBitmap CreateBitmap(byte[] dataBinary, int version, int numberMask, QRCodeErrorCorrection levelCorrection, SKColor foregroundColor, SKColor backgroundColor, int margin)
        {
            int sizeCanvas = 21 + 4 * (version - 1);
            int size = sizeCanvas + 2 * margin;
            var image = new SKBitmap(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
            var imageMaskQRCode = new SKBitmap(size, size, SKColorType.Rgba8888, SKAlphaType.Premul); // system zones

            // Create a base system template
            using (var canvas = new SKCanvas(image))
            using (var canvasMask = new SKCanvas(imageMaskQRCode))
            using (var foregroundPaint = new SKPaint { Color = foregroundColor, Style = SKPaintStyle.Fill })
            using (var backgroundPaint = new SKPaint { Color = backgroundColor, Style = SKPaintStyle.Fill })
            {
                canvas.Clear(backgroundColor);
                canvasMask.Clear(SKColors.Transparent);

                AddTemplateToCanvas(canvas, canvasMask, version, sizeCanvas,
                                      foregroundPaint, backgroundPaint, margin);
                AddLevelDataToMask(canvasMask, sizeCanvas, margin);
            }

            // Add sync lines to the image
            AddSyncToImage(image, imageMaskQRCode, sizeCanvas,
                           foregroundColor, margin);

            // Add the version code to the image
            AddVersionDataToImage(image, imageMaskQRCode, version, sizeCanvas,
                                  foregroundColor, margin);

            // Add the data to the image
            AddBinaryDataToImage(image, imageMaskQRCode, dataBinary, sizeCanvas,
                                 foregroundColor, numberMask, margin);

            // Add the code of the selected correction level and mask
            AddLevelAndMaskCodes(image, numberMask, levelCorrection, sizeCanvas,
                                 foregroundColor, margin);
            return image;
        }

        private static void AddLevelAndMaskCodes(SKBitmap image, int numberMask, QRCodeErrorCorrection levelCorrection, int sizeCanvas, SKColor foregroundColor, int margin)
        {
            int code = QRCodesData.CodeMaskAndLevels[8 * ((int)levelCorrection) + numberMask];
            for (int i = 0; i < 15; i++)
            {
                if (((code >> i) & 1) == 1)
                {
                    if (i < 6)
                    {
                        SetPixelToBitmap(image, 8, i, margin, foregroundColor);
                        SetPixelToBitmap(image, sizeCanvas - i - 1, 8, margin, foregroundColor);
                    }
                    else if (i < 8)
                    {
                        SetPixelToBitmap(image, 8, i + 1, margin, foregroundColor);
                        SetPixelToBitmap(image, sizeCanvas - i - 1, 8, margin, foregroundColor);
                    }
                    else if (i < 9)
                    {
                        SetPixelToBitmap(image, 8, sizeCanvas - 7, margin, foregroundColor);
                        SetPixelToBitmap(image, 7, 8, margin, foregroundColor);
                    }
                    else
                    {
                        SetPixelToBitmap(image, 5 - (i - 9), 8, margin, foregroundColor);
                        SetPixelToBitmap(image, 8, sizeCanvas - 7 + (i - 8), margin, foregroundColor);
                    }
                }
            }
            SetPixelToBitmap(image, 8, sizeCanvas - 8, margin, foregroundColor);
        }

        private static void AddLevelDataToMask(SKCanvas canvasMask, int sizeCanvas, int margin)
        {
            var coord8 = GetPosition(8, margin);
            var coord0 = GetPosition(0, margin);
            var coordCanvasMinus1 = GetPosition(sizeCanvas - 1, margin);
            var coordCanvasMinus8 = GetPosition(sizeCanvas - 8, margin);

            using (var paint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 1 })
            {
                canvasMask.DrawLine(coord8, coord0, coord8, coord8, paint);
                canvasMask.DrawLine(coord0, coord8, coord8, coord8, paint);
                canvasMask.DrawLine(coordCanvasMinus8, coord8, coordCanvasMinus1, coord8, paint);
                canvasMask.DrawLine(coord8, coordCanvasMinus8, coord8, coordCanvasMinus1, paint);
            }
        }

        private static void AddBinaryDataToImage(SKBitmap image, SKBitmap imageMask, byte[] dataBinary, int sizeCanvas, SKColor foregroundColor, int numberMask, int margin)
        {
            int indexByte = 0;
            int indexBit = 7;
            int stripX = sizeCanvas - 2;
            int stripY = sizeCanvas - 1;
            bool isUp = true;
            int offsetX = 1;
            bool hasPixel = false;

            while (indexByte < dataBinary.Length)
            {
                var posX = stripX + offsetX;
                var posY = stripY;
                var x = GetPosition(posX, margin);
                var y = GetPosition(posY, margin);

                // If not a system area pixel, then write a bit of data
                if (imageMask.GetPixel(x, y).Alpha == 0)
                {
                    if (((dataBinary[indexByte] >> indexBit) & 1) == 1)
                        hasPixel = true;
                    else
                        hasPixel = false;

                    // Write the mask
                    if (GetMaskValueOfXY(numberMask, posX, posY) == 0)
                        hasPixel = !hasPixel;

                    if (hasPixel)
                        image.SetPixel(x, y, foregroundColor);

                    indexBit--;
                    if (indexBit < 0)
                    {
                        indexBit = 7;
                        indexByte++;
                    }
                }
                CalcNextCurrentPosition(ref stripX, ref stripY, ref offsetX, ref isUp,
                                        sizeCanvas);
            }
        }

        private static void CalcNextCurrentPosition(ref int stripX, ref int stripY, ref int offsetX, ref bool isUp, int sizeCanvas)
        {
            if (offsetX > 0)
                offsetX = 0;
            else
            {
                offsetX = 1;
                stripY += isUp ? -1 : 1;
                if (stripY < 0)
                {

                    // then move from top to bottom
                    isUp = !isUp;
                    stripY = 0;
                    stripX -= 2;
                    if (stripX == 5)
                        stripX -= 1;
                }
                else if (stripY >= sizeCanvas)
                {

                    // then move from bottom to top
                    isUp = !isUp;
                    stripY = sizeCanvas - 1;
                    stripX -= 2;
                    if (stripX == 5)
                        stripX -= 1;
                }
            }
        }

        private static int GetMaskValueOfXY(int numberMask, int x, int y)
        {

            // Mask to improve the QR code reading
            switch(numberMask)
            {
                case 0:
                    return (x + y) % 2;
                case 1:
                    return y % 2;
                case 2:
                    return x % 3;
                case 3:
                    return (x + y) % 3;
                case 4:
                    return (x / 3 + y / 2) % 2;
                case 5:
                    return (x * y) % 2 + (x * y) % 3;
                case 6:
                    return ((x * y) % 2 + (x * y) % 3) % 2;
                case 7:
                    return ((x * y) % 3 + (x + y) % 2) % 2;
            }
            throw new NotImplementedException();
        }

        private static void AddVersionDataToImage(SKBitmap image, SKBitmap imageMask, int version, int sizeCanvas, SKColor foregroundColor, int margin)
        {
            if (version >= 7)
            {
                byte line1 = QRCodesData.VersionData[version - 7][0];
                byte line2 = QRCodesData.VersionData[version - 7][1];
                byte line3 = QRCodesData.VersionData[version - 7][2];

                for (int i = 0; i < 6; i++)
                {
                    if ((line1 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 11, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 11, margin, foregroundColor);
                    }
                    if ((line2 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 10, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 10, margin, foregroundColor);
                    }
                    if ((line3 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 9, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 9, margin, foregroundColor);
                    }
                    SetPixelToBitmap(imageMask, sizeCanvas - 11, 5 - i, margin, SKColors.Black);
                    SetPixelToBitmap(imageMask, 5 - i, sizeCanvas - 11, margin, SKColors.Black);
                    SetPixelToBitmap(imageMask, sizeCanvas - 10, 5 - i, margin, SKColors.Black);
                    SetPixelToBitmap(imageMask, 5 - i, sizeCanvas - 10, margin, SKColors.Black);
                    SetPixelToBitmap(imageMask, sizeCanvas - 9, 5 - i, margin, SKColors.Black);
                    SetPixelToBitmap(imageMask, 5 - i, sizeCanvas - 9, margin, SKColors.Black);
                    line1 >>= 1;
                    line2 >>= 1;
                    line3 >>= 1;
                }
            }
        }

        private static void AddSyncToImage(SKBitmap image, SKBitmap imageMask, int sizeCanvas, SKColor foregroundColor, int margin)
        {
            for (int t = 7; t <= sizeCanvas - 7; t+=1)
            {
                if (t % 2 == 0)
                {
                    SetPixelToBitmap(image, t, 6, margin, foregroundColor);
                    SetPixelToBitmap(image, 6, t, margin, foregroundColor);
                }
                SetPixelToBitmap(imageMask, t, 6, margin, foregroundColor);
                SetPixelToBitmap(imageMask, 6, t, margin, foregroundColor);
            }
        }

        private static void AddTemplateToCanvas(SKCanvas canvas, SKCanvas canvasMask, int version, int sizeCanvas, SKPaint foregroundPaint, SKPaint backgroundPaint, int margin)
        {
            AddFindingSquareToCanvas(canvas, canvasMask,
                                       GetPosition(3, margin),
                                       GetPosition(3, margin),
                                       foregroundPaint,
                                       backgroundPaint);
            AddFindingSquareToCanvas(canvas, canvasMask,
                                       GetPosition(3, margin),
                                       GetPosition(sizeCanvas - 4, margin),
                                       foregroundPaint,
                                       backgroundPaint);
            AddFindingSquareToCanvas(canvas, canvasMask,
                                       GetPosition(sizeCanvas - 4, margin),
                                       GetPosition(3, margin),
                                       foregroundPaint,
                                       backgroundPaint);

            foreach(int x in QRCodesData.CoordinatesOfAlignSquares[version - 1])
                foreach(int y in QRCodesData.CoordinatesOfAlignSquares[version - 1])
                {
                    if (!(x < 8 && y < 8 || x < 8 && y > sizeCanvas - 8 ||
                        x > sizeCanvas - 8 && y < 8))
                    {
                        AddAlignSquareToCanvas(canvas, canvasMask,
                                                 GetPosition(x, margin),
                                                 GetPosition(y, margin),
                                                 foregroundPaint,
                                                 backgroundPaint);
                    }
                }
        }

        private static void AddFindingSquareToCanvas(SKCanvas canvas, SKCanvas canvasMask, float centerX, float centerY, SKPaint foregroundPaint, SKPaint backgroundPaint)
        {
            using (var maskPaint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill })
            {
                canvas.DrawRect(centerX - 3, centerY - 3, 7, 7, foregroundPaint);
                canvas.DrawRect(centerX - 2, centerY - 2, 5, 5, backgroundPaint);
                canvas.DrawRect(centerX - 1, centerY - 1, 3, 3, foregroundPaint);
                canvasMask.DrawRect(centerX - 4, centerY - 4, 9, 9, maskPaint);
            }
        }

        private static void AddAlignSquareToCanvas(SKCanvas canvas, SKCanvas canvasMask, float centerX, float centerY, SKPaint foregroundPaint, SKPaint backgroundPaint)
        {
            using (var maskPaint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill })
            {
                canvas.DrawRect(centerX - 2, centerY - 2, 5, 5, foregroundPaint);
                canvas.DrawRect(centerX - 1, centerY - 1, 3, 3, backgroundPaint);
                canvas.DrawRect(centerX, centerY, 1, 1, foregroundPaint);
                canvasMask.DrawRect(centerX - 2, centerY - 2, 5, 5, maskPaint);
            }
        }

        /// <summary>
        /// Gets the absolute position from the QR code canvas.
        /// </summary>
        /// <param name="coord">The coordinate.</param>
        /// <param name="margin">The quiet zone.</param>
        /// <returns>int</returns>
        private static int GetPosition(int coord, int margin) => margin + coord;

        /// <summary>
        /// Sets pixel to the QR code bitmap.
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="margin">The quiet zone.</param>
        /// <returns>int</returns>
        private static void SetPixelToBitmap(SKBitmap image, int x, int y, int margin, SKColor foregroundColor)
        {
            image.SetPixel(GetPosition(x, margin),
                           GetPosition(y, margin),
                           foregroundColor);
        }
    }
}
