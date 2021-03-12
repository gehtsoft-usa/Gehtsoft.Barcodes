using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using Gehtsoft.Barcodes.Utils;
using System.Drawing.Imaging;
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
        internal static byte[] GetBarcodeImageEAN_UPC(byte[] encodedData, string textDataString, BarcodeType barcodeType, bool showDataLabel, MeasureBarcodeUnit heightToCut, int scaleMultiplier = 2, Color? strokeColor = null, Color? backColor = null, bool hasQuietZones = true, BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            // Draw barcode on bitmap:
            Bitmap bitmap = DrawBitmapEAN_UPC(encodedData, textDataString, barcodeType, heightToCut, showDataLabel, scaleMultiplier, strokeColor, backColor, hasQuietZones, barcodeRotation);
            // Save bitmap to a byte array:
            using (var memStream = new MemoryStream())
            {
                bitmap.Save(memStream, ImageFormat.Png);
                return memStream.ToArray();
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
            Color? strokeColor = null, Color? backColor = null, bool hasQuietZones = true, BarcodeRotation barcodeRotation = BarcodeRotation.Clockwise_0)
        {
            // Draw barcode on bitmap:
            Bitmap bitmap = DrawBitmapGS1_128(encodedData, textDataString, heightToCut, showDataLabel, scaleMultiplier, strokeColor, backColor, hasQuietZones, barcodeRotation);
            // Save bitmap to a byte array:
            using (var memStream = new MemoryStream())
            {
                bitmap.Save(memStream, ImageFormat.Png);
                return memStream.ToArray();
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
        internal static Bitmap DrawBitmapEAN_UPC(byte[] encodedData, string textDataString, BarcodeType barcodeType, 
            MeasureBarcodeUnit heightToCut, bool showDataLabel, int scaleMultiplier = 2, Color? strokeColor = null, Color? backColor = null, 
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
            Font labelFont = new Font(EANData.font_family_name, EANData.default_font_size * scaleMultiplier);
            Font labelFontSmall = new Font(EANData.font_family_name, EANData.default_font_small_size * scaleMultiplier);
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

            Bitmap bitmap;

            bitmap = new Bitmap(barcodeWidth, barcodeHeight);

            bitmap.SetResolution(dotsPerInch, dotsPerInch);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                void DrawStrokeLine(int pos, bool isSeparator = false)
                { 
                    graphics.DrawLine(new Pen(strokeColor ?? Color.Black, scaleMultiplier),
                        new PointF(pos * scaleMultiplier + halfBarItemWidth, shiftFromTop),
                        new PointF(pos * scaleMultiplier + halfBarItemWidth, shiftFromTop + (isSeparator ? separatorHeight : strokeHeight)));
                }

                void DrawSpaceLine(int pos, bool isSeparator = false)
                {
                    graphics.DrawLine(new Pen(backColor ?? Color.White, scaleMultiplier),
                        new PointF(pos * scaleMultiplier + halfBarItemWidth, shiftFromTop),
                        new PointF(pos * scaleMultiplier + halfBarItemWidth, shiftFromTop + (isSeparator ? separatorHeight : strokeHeight)));
                }

                // Paint the background:
                graphics.Clear(backColor ?? Color.White);

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
                    var digitsPath = new GraphicsPath();
                    var digitsBrush = new SolidBrush(strokeColor ?? Color.Black);

                    int oneDigitShift = 0;
                    if (barcodeType == BarcodeType.UPC_A)
                    {
                        oneDigitShift = 7;
                    }

                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    float textPosY = barcodeHeight - 1 - labelFont.Size;

                    digitsPath.AddString(textDataLeft, labelFont.FontFamily, (int)labelFont.Style, labelFont.Size,
                        new RectangleF(
                            (leftQuiteZoneCount + leftSeparatorCount + oneDigitShift + 3) * scaleMultiplier,
                            textPosY,
                            (leftCodePartCount + 7) * scaleMultiplier,
                            labelFont.Height
                            ),
                        StringFormat.GenericTypographic);
                    digitsPath.AddString(textDataRight, labelFont.FontFamily, (int)labelFont.Style, labelFont.Size,
                        new RectangleF(
                            (leftQuiteZoneCount + leftSeparatorCount + 2 + leftCodePartCount + middleSeparatorCount) * scaleMultiplier,
                            textPosY,
                            (leftCodePartCount + 7) * scaleMultiplier,
                            labelFont.Height
                            ),
                        StringFormat.GenericTypographic);

                    // Print the first digit for EAN-13:
                    if ((barcodeType == BarcodeType.EAN_13) && hasQuietZones)
                    {
                        digitsPath.AddString(textDataString.Substring(0, 1), labelFont.FontFamily, (int)labelFont.Style, labelFont.Size,
                        new RectangleF(
                            1 * scaleMultiplier,
                            textPosY,
                            (leftCodePartCount + 7) * scaleMultiplier,
                            labelFont.Height
                            ),
                        StringFormat.GenericTypographic);
                    }

                    // Print the first and the last digits for UPC-A:
                    if ((barcodeType == BarcodeType.UPC_A) && hasQuietZones)
                    {
                        digitsPath.AddString(textDataString.Substring(0, 1), labelFontSmall.FontFamily, (int)labelFontSmall.Style, labelFontSmall.Size,
                        new RectangleF(
                            2 * scaleMultiplier,
                            (textPosY + labelFont.Size - labelFontSmall.Size),
                            (7) * scaleMultiplier,
                            labelFontSmall.Height
                            ),
                        StringFormat.GenericTypographic);

                        digitsPath.AddString(textDataRight, labelFontSmall.FontFamily, (int)labelFontSmall.Style, labelFontSmall.Size,
                        new RectangleF(
                            (leftQuiteZoneCount + leftSeparatorCount * 2 + 2 + leftCodePartCount * 2 + middleSeparatorCount) * scaleMultiplier,
                            (textPosY + labelFont.Size - labelFontSmall.Size),
                            (7) * scaleMultiplier,
                            labelFontSmall.Height
                            ),
                        StringFormat.GenericTypographic);
                    }

                    graphics.FillPath(digitsBrush, digitsPath);
                }
            }

           switch(barcodeRotation)
            {
                case BarcodeRotation.Clockwise_90:
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case BarcodeRotation.Clockwise_180:
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case BarcodeRotation.Clockwise_270:
                    bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
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
        internal static Bitmap DrawBitmapGS1_128(byte[] encodedData, string textDataString,
            MeasureBarcodeUnit heightToCut, bool showDataLabel = true, int scaleMultiplier = 2,
            Color? strokeColor = null, Color ? backColor = null, bool hasQuietZones = true, 
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

            Font labelFont = new Font(GS1_128Data.font_family_name, GS1_128Data.default_font_size * scaleMultiplier);

            float labelY = 0f, labelHeight = 0f;
            float labelWidth = 0f;
            if (showDataLabel)
            {
                labelHeight = scaleMultiplier * 2;
                labelY = labelHeight + internalHeight;
                labelHeight += labelFont.Size;
                using (var temp = new Bitmap(1, 1))
                    using (var g = Graphics.FromImage(temp))
                        labelWidth = g.MeasureString(textDataString, labelFont).Width;
            }

            var bitmap = new Bitmap(internalWidth, internalHeight + (int) labelHeight);

            int dotsPerInch = GS1_128Data.dots_per_inch;
            bitmap.SetResolution(dotsPerInch, dotsPerInch);

            int pos = 0;
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(backColor ?? Color.White);

                Pen backgroundPen = new Pen(backColor ?? Color.White, scaleMultiplier);
                Pen strokePen = new Pen(strokeColor ?? Color.Black, scaleMultiplier);
                Brush backgroundBrush = new SolidBrush(backColor ?? Color.White);

                // 1. Left quiet zone
                graphics.FillRectangle(backgroundBrush,
                    x: 0, y: 0,
                    width: quietZoneWidth, height: internalHeight);
                pos += quietZoneWidth; // quiet zone is 10 * X here

                // 2. Stop character
                for (int i = 0; i < encodedData.Length; i++)
                {
                    graphics.DrawLine(encodedData[i] == 1 ? strokePen : backgroundPen,
                        new Point(pos + halfBarItemWidth, 0),
                        new Point(pos + halfBarItemWidth, internalHeight));

                    pos += scaleMultiplier;
                }

                // 3. Right quiet zone
                graphics.FillRectangle(backgroundBrush,
                    x: pos + halfBarItemWidth, y: 0,
                    width: quietZoneWidth, height: internalHeight);

                // 4. Print data string under the barcode
                if (showDataLabel)
                {
                    labelY = internalHeight + 1;

                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    var labelPath = new GraphicsPath();
                    var labelBrush = new SolidBrush(strokeColor ?? Color.Black);

                    labelPath.AddString(textDataString, labelFont.FontFamily, (int)labelFont.Style, labelFont.Size,
                        new RectangleF(x: internalWidth/2f - labelWidth / 2, y: labelY, labelWidth, labelHeight),
                        new StringFormat(StringFormat.GenericTypographic) { Alignment = StringAlignment.Center });

                    graphics.FillPath(labelBrush, labelPath);
                }
            }

            switch (barcodeRotation)
            {
                case BarcodeRotation.Clockwise_90:
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case BarcodeRotation.Clockwise_180:
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case BarcodeRotation.Clockwise_270:
                    bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return bitmap;
        }

    }
}
