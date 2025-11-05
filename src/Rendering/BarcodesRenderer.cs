using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using SkiaSharp;
using System.IO;
using Gehtsoft.Barcodes.Utils;
using System;

namespace Gehtsoft.Barcodes.Rendering
{
    /// <summary>
    /// Describes the logic of barcodes rendering.
    /// </summary>
    internal static class BarcodesRenderer
    {
        /// <summary>
        /// Renders a EAN/UPC barcode from the encoded data depending on the barcode type and returns byte array.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="strokeColor">The color of barcode lines.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="hasQuietZones">Defines whether the barcode has quiet zones.</param>
        /// <param name="showDataLabel">Defines whether to print the barcode data under the lines.</param>
        /// <param name="textDataString">The text to be printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines. Can be 0.</param>
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="barcodeRotation">Defines barcode rotation angle.</param>
        /// <returns>Byte array</returns>
        internal static byte[] GetBarcodeImageEAN_UPC(byte[] encodedData, string textDataString, BarcodeType barcodeType, bool showDataLabel, MeasureBarcodeUnit heightToCut, int scaleMultiplier = 2, SKColor? strokeColor = null, SKColor? backColor = null, bool hasQuietZones = true, BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            // Draw barcode on bitmap:
            using (var bitmap = DrawBitmapEAN_UPC(encodedData, textDataString, barcodeType, heightToCut, showDataLabel, scaleMultiplier, strokeColor, backColor, hasQuietZones, barcodeRotation))
            {
                // Save bitmap to a byte array:
                using (var memStream = new MemoryStream())
                using (var image = SKImage.FromBitmap(bitmap))
                using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    encoded.SaveTo(memStream);
                    return memStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Renders a GS1-128 barcode on bitmap from the encoded data.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="textDataString">The text to be printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines. Can be 0.</param>
        /// <param name="showDataLabel">Defines whether to print the barcode data under the lines.</param>
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="hasQuietZones">Defines whether the barcode has quiet zones.</param>
        /// <param name="strokeColor">The color of barcode lines.</param>
        /// <param name="barcodeRotation">Defines barcode rotation angle.</param>
        /// <returns></returns>
        internal static byte[] GetBarcodeImageGS1_128(byte[] encodedData, string textDataString,
            MeasureBarcodeUnit heightToCut, bool showDataLabel = true, int scaleMultiplier = 2,
            SKColor? strokeColor = null, SKColor? backColor = null, bool hasQuietZones = true, BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            // Draw barcode on bitmap:
            using (var bitmap = DrawBitmapGS1_128(encodedData, textDataString, heightToCut, showDataLabel, scaleMultiplier, strokeColor, backColor, hasQuietZones, barcodeRotation))
            {
                // Save bitmap to a byte array:
                using (var memStream = new MemoryStream())
                using (var image = SKImage.FromBitmap(bitmap))
                using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    encoded.SaveTo(memStream);
                    return memStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Draws a EAN/UPC barcode on bitmap from the encoded data depending on the barcode type and returns bitmap.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="strokeColor">The color of barcode lines.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="showDataLabel">Defines whether to print the barcode data under the lines.</param>
        /// <param name="textDataString">The text to be printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines. Can be 0.</param>
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="hasQuietZones">Defines whether the barcode has quiet zones.</param>
        /// <param name="barcodeRotation">Defines barcode rotation angle.</param>
        /// <returns>Bitmap</returns>
        internal static SKBitmap DrawBitmapEAN_UPC(byte[] encodedData, string textDataString, BarcodeType barcodeType,
            MeasureBarcodeUnit heightToCut, bool showDataLabel, int scaleMultiplier = 2, SKColor? strokeColor = null, SKColor? backColor = null,
            bool hasQuietZones = true, BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            // Bitmap accepts only integer values:
            int barcodeWidth;
            int barcodeHeight;
            int strokeHeight;
            int separatorHeight;
            int leftQuiteZoneCount;
            int leftCodePartCount;
            int shiftFromTop = EANData.shift_from_top * scaleMultiplier;
            float labelFontSize = EANData.default_font_size * scaleMultiplier;
            float labelFontSmallSize = EANData.default_font_small_size * scaleMultiplier;
            string textDataLeft;
            string textDataRight;
            int dotsPerInch = EANData.dots_per_inch;
            int halfBarItemWidth = scaleMultiplier / 2;

            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                case BarcodeType.UPC_A:
                    barcodeWidth = EANData.default_barcode_width_EAN_13 * scaleMultiplier;
                    barcodeHeight = BarcodesUtils.GetBarcodeHeightInt(EANData.default_barcode_width_EAN_13 * scaleMultiplier, barcodeType);
                    strokeHeight = BarcodesUtils.GetStrokeHeight(EANData.default_barcode_width_EAN_13 * scaleMultiplier, barcodeType);
                    separatorHeight = BarcodesUtils.GetSeparatorHeight(EANData.default_barcode_width_EAN_13 * scaleMultiplier, barcodeType);
                    leftQuiteZoneCount = EANData.left_quite_zone_count_EAN_13;
                    leftCodePartCount = 42;
                    textDataLeft = textDataString.Substring(0, 6);
                    textDataRight = textDataString.Substring(6, 6);
                    if (barcodeType == BarcodeType.UPC_A)
                    {
                        textDataLeft = textDataLeft.Substring(1, 5);
                        textDataRight = textDataRight.Substring(0, 5);
                    }
                    break;
                case BarcodeType.EAN_8:
                    barcodeWidth = EANData.default_barcode_width_EAN_8 * scaleMultiplier;
                    barcodeHeight = BarcodesUtils.GetBarcodeHeightInt(EANData.default_barcode_width_EAN_8 * scaleMultiplier, barcodeType);
                    strokeHeight = BarcodesUtils.GetStrokeHeight(EANData.default_barcode_width_EAN_8 * scaleMultiplier, barcodeType);
                    separatorHeight = BarcodesUtils.GetSeparatorHeight(EANData.default_barcode_width_EAN_8 * scaleMultiplier, barcodeType);
                    leftQuiteZoneCount = EANData.left_quite_zone_count_EAN_8;
                    leftCodePartCount = 28;
                    textDataLeft = textDataString.Substring(0, 4);
                    textDataRight = textDataString.Substring(4, 4);
                    break;
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }

            int leftSeparatorCount = 3;
            int middleSeparatorCount = 5;
            int rightQuiteZoneCount = EANData.right_quite_zone_count;

            if (!hasQuietZones)
            {
                barcodeWidth = barcodeWidth - (leftQuiteZoneCount + rightQuiteZoneCount) * scaleMultiplier;
                leftQuiteZoneCount = 0;
                rightQuiteZoneCount = 0;
            }

            // Calculate the percent the user wants to cut from the top depending on height:
            float percentToCut = BarcodesUtils.GetPercentToCut(barcodeHeight, heightToCut, scaleMultiplier);

            int cut = (int)(barcodeHeight * percentToCut / 100);

            strokeHeight = Math.Max(strokeHeight - cut, 0);
            separatorHeight = Math.Max(separatorHeight - cut, 0);
            barcodeHeight = barcodeHeight - cut;

            var bitmap = new SKBitmap(barcodeWidth, barcodeHeight, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(bitmap))
            using (var strokePaint = new SKPaint { Color = strokeColor ?? SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = scaleMultiplier, IsAntialias = false })
            using (var spacePaint = new SKPaint { Color = backColor ?? SKColors.White, Style = SKPaintStyle.Stroke, StrokeWidth = scaleMultiplier, IsAntialias = false })
            {
                void DrawStrokeLine(int pos, bool isSeparator = false)
                {
                    canvas.DrawLine(
                        pos * scaleMultiplier + halfBarItemWidth, shiftFromTop,
                        pos * scaleMultiplier + halfBarItemWidth, shiftFromTop + (isSeparator ? separatorHeight : strokeHeight),
                        strokePaint);
                }

                void DrawSpaceLine(int pos, bool isSeparator = false)
                {
                    canvas.DrawLine(
                        pos * scaleMultiplier + halfBarItemWidth, shiftFromTop,
                        pos * scaleMultiplier + halfBarItemWidth, shiftFromTop + (isSeparator ? separatorHeight : strokeHeight),
                        spacePaint);
                }

                // Paint the background:
                canvas.Clear(backColor ?? SKColors.White);

                int x = 0;

                // Left quite zone:
                while (x < leftQuiteZoneCount)
                {
                    DrawSpaceLine(x++);
                }

                // Left separator:
                DrawStrokeLine(x++, true);
                DrawSpaceLine(x++, true);
                DrawStrokeLine(x++, true);

                // Left part of the barcode:
                while (x - leftQuiteZoneCount - leftSeparatorCount < leftCodePartCount)
                {
                    // First digit of UPC must be longer than the others:
                    bool isFirstDigit = (x - leftQuiteZoneCount - leftSeparatorCount >= 0) && (x - leftQuiteZoneCount - leftSeparatorCount <= 6);
                    if (encodedData[x - leftQuiteZoneCount - leftSeparatorCount] == 1)
                        DrawStrokeLine(x, (barcodeType == BarcodeType.UPC_A) && isFirstDigit);
                    else
                        DrawSpaceLine(x, (barcodeType == BarcodeType.UPC_A) && isFirstDigit);
                    x++;
                }

                // Middle separator:
                DrawSpaceLine(x++, true);
                DrawStrokeLine(x++, true);
                DrawSpaceLine(x++, true);
                DrawStrokeLine(x++, true);
                DrawSpaceLine(x++, true);

                // Right part of the barcode:
                while (x - leftQuiteZoneCount - leftSeparatorCount - middleSeparatorCount < leftCodePartCount * 2)
                {
                    // Last digit of UPC must be longer than the others:
                    bool isLastDigit = (x - leftQuiteZoneCount - leftSeparatorCount - middleSeparatorCount >= 77) && (x - leftQuiteZoneCount - leftSeparatorCount - middleSeparatorCount <= 83);
                    if (encodedData[x - leftQuiteZoneCount - leftSeparatorCount - middleSeparatorCount] == 1)
                        DrawStrokeLine(x, (barcodeType == BarcodeType.UPC_A) && isLastDigit);
                    else
                        DrawSpaceLine(x, (barcodeType == BarcodeType.UPC_A) && isLastDigit);
                    x++;
                }

                // Right separator:
                DrawStrokeLine(x++, true);
                DrawSpaceLine(x++, true);
                DrawStrokeLine(x++, true);

                // Right quite zone:
                for (int i = 0; i < rightQuiteZoneCount; i++)
                {
                    DrawSpaceLine(x++);
                }

                if (showDataLabel)
                {
                    using (var textPaint = new SKPaint
                    {
                        Color = strokeColor ?? SKColors.Black,
                        TextSize = labelFontSize,
                        IsAntialias = true,
                        Typeface = SKTypeface.FromFamilyName(EANData.font_family_name)
                    })
                    using (var textPaintSmall = new SKPaint
                    {
                        Color = strokeColor ?? SKColors.Black,
                        TextSize = labelFontSmallSize,
                        IsAntialias = true,
                        Typeface = SKTypeface.FromFamilyName(EANData.font_family_name)
                    })
                    {
                        int oneDigitShift = 0;
                        if (barcodeType == BarcodeType.UPC_A)
                        {
                            oneDigitShift = 7;
                        }

                        float textPosY = barcodeHeight - 1;

                        // Draw left text
                        float textLeftX = (leftQuiteZoneCount + leftSeparatorCount + oneDigitShift + 3) * scaleMultiplier;
                        canvas.DrawText(textDataLeft, textLeftX, textPosY, textPaint);

                        // Draw right text
                        float textRightX = (leftQuiteZoneCount + leftSeparatorCount + 2 + leftCodePartCount + middleSeparatorCount) * scaleMultiplier;
                        canvas.DrawText(textDataRight, textRightX, textPosY, textPaint);

                        // Print the first digit for EAN-13:
                        if ((barcodeType == BarcodeType.EAN_13) && hasQuietZones)
                        {
                            canvas.DrawText(textDataString.Substring(0, 1), 1 * scaleMultiplier, textPosY, textPaint);
                        }

                        // Print the first and the last digits for UPC-A:
                        if ((barcodeType == BarcodeType.UPC_A) && hasQuietZones)
                        {
                            float smallTextPosY = textPosY - (labelFontSize - labelFontSmallSize);
                            canvas.DrawText(textDataString.Substring(0, 1), 2 * scaleMultiplier, smallTextPosY, textPaintSmall);

                            float lastDigitX = (leftQuiteZoneCount + leftSeparatorCount * 2 + 2 + leftCodePartCount * 2 + middleSeparatorCount) * scaleMultiplier;
                            canvas.DrawText(textDataString.Substring(11, 1), lastDigitX, smallTextPosY, textPaintSmall);
                        }
                    }
                }
            }

           switch(barcodeRotation)
            {
                case BarcodeRotation.Clockwise_90:
                    return RotateBitmap(bitmap, 90);
                case BarcodeRotation.Clockwise_180:
                    return RotateBitmap(bitmap, 180);
                case BarcodeRotation.Clockwise_270:
                    return RotateBitmap(bitmap, 270);
            }

            return bitmap;
        }

        /// <summary>
        /// Draws a GS1-128 barcode on bitmap from the encoded data and returns bitmap.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="textDataString">The text to be printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines. Can be 0.</param>
        /// <param name="showDataLabel">Defines whether to print the barcode data under the lines.</param>
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="hasQuietZones">Defines whether the barcode has quiet zones.</param>
        /// <param name="strokeColor">The color of barcode lines.</param>
        /// <param name="barcodeRotation">Defines barcode rotation angle.</param>
        /// <returns>Bitmap</returns>
        internal static SKBitmap DrawBitmapGS1_128(byte[] encodedData, string textDataString,
            MeasureBarcodeUnit heightToCut, bool showDataLabel = true, int scaleMultiplier = 2,
            SKColor? strokeColor = null, SKColor ? backColor = null, bool hasQuietZones = true,
            BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            int halfBarItemWidth = scaleMultiplier / 2;

            int quietZoneWidth = GS1_128Data.QuietZoneMinimumWidth * scaleMultiplier;

            if (!hasQuietZones)
            {
                quietZoneWidth = 0;
            }

            int internalWidth = quietZoneWidth * 2 + encodedData.Length * scaleMultiplier;

            // Barcode height is 15% of the width:
            int internalHeight = (int)(internalWidth * 0.15f);

            // Calculate the percent depending on height:
            float percentToCut = BarcodesUtils.GetPercentToCut(internalHeight, heightToCut, scaleMultiplier);

            internalHeight = internalHeight - (int)(internalHeight * percentToCut / 100);

            float labelFontSize = GS1_128Data.default_font_size * scaleMultiplier;

            float labelY = 0f, labelHeight = 0f;
            float labelWidth = 0f;
            if (showDataLabel)
            {
                labelHeight = scaleMultiplier * 2;
                labelY = labelHeight + internalHeight;
                labelHeight += labelFontSize;

                using (var tempPaint = new SKPaint
                {
                    TextSize = labelFontSize,
                    Typeface = SKTypeface.FromFamilyName(GS1_128Data.font_family_name)
                })
                {
                    labelWidth = tempPaint.MeasureText(textDataString);
                }
            }

            var bitmap = new SKBitmap(internalWidth, internalHeight + (int) labelHeight, SKColorType.Rgba8888, SKAlphaType.Premul);

            int dotsPerInch = GS1_128Data.dots_per_inch;

            int pos = 0;
            using (var canvas = new SKCanvas(bitmap))
            using (var backgroundPaint = new SKPaint { Color = backColor ?? SKColors.White, Style = SKPaintStyle.Fill })
            using (var strokePaint = new SKPaint { Color = strokeColor ?? SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = scaleMultiplier, IsAntialias = false })
            using (var backgroundStrokePaint = new SKPaint { Color = backColor ?? SKColors.White, Style = SKPaintStyle.Stroke, StrokeWidth = scaleMultiplier, IsAntialias = false })
            {
                canvas.Clear(backColor ?? SKColors.White);

                // 1. Left quiet zone
                canvas.DrawRect(0, 0, quietZoneWidth, internalHeight, backgroundPaint);
                pos += quietZoneWidth; // quiet zone is 10 * X here

                // 2. Stop character
                for (int i = 0; i < encodedData.Length; i++)
                {
                    canvas.DrawLine(
                        pos + halfBarItemWidth, 0,
                        pos + halfBarItemWidth, internalHeight,
                        encodedData[i] == 1 ? strokePaint : backgroundStrokePaint);

                    pos += scaleMultiplier;
                }

                // 3. Right quiet zone
                canvas.DrawRect(pos + halfBarItemWidth, 0, quietZoneWidth, internalHeight, backgroundPaint);

                // 4. Print data string under the barcode
                if (showDataLabel)
                {
                    labelY = internalHeight + 1;

                    using (var textPaint = new SKPaint
                    {
                        Color = strokeColor ?? SKColors.Black,
                        TextSize = labelFontSize,
                        IsAntialias = true,
                        TextAlign = SKTextAlign.Center,
                        Typeface = SKTypeface.FromFamilyName(GS1_128Data.font_family_name)
                    })
                    {
                        float centerX = internalWidth / 2f;
                        canvas.DrawText(textDataString, centerX, labelY + labelFontSize, textPaint);
                    }
                }
            }

            switch (barcodeRotation)
            {
                case BarcodeRotation.Clockwise_90:
                    return RotateBitmap(bitmap, 90);
                case BarcodeRotation.Clockwise_180:
                    return RotateBitmap(bitmap, 180);
                case BarcodeRotation.Clockwise_270:
                    return RotateBitmap(bitmap, 270);
            }

            return bitmap;
        }

        /// <summary>
        /// Rotates a bitmap by the specified angle.
        /// </summary>
        private static SKBitmap RotateBitmap(SKBitmap source, float degrees)
        {
            // For 90 and 270 degree rotations, swap width and height
            int newWidth = (degrees == 90 || degrees == 270) ? source.Height : source.Width;
            int newHeight = (degrees == 90 || degrees == 270) ? source.Width : source.Height;

            var rotated = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);

            using (var canvas = new SKCanvas(rotated))
            {
                canvas.Clear(SKColors.Transparent);

                // Move to center, rotate, move back
                canvas.Translate(newWidth / 2f, newHeight / 2f);
                canvas.RotateDegrees(degrees);
                canvas.Translate(-source.Width / 2f, -source.Height / 2f);

                canvas.DrawBitmap(source, 0, 0);
            }

            source.Dispose();
            return rotated;
        }
    }
}
