using System;
using FluentAssertions;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Utils;
using Xunit;
using Gehtsoft.Barcodes.Encoding;
using Gehtsoft.Barcodes.UnitTests.Barcodes;
using Gehtsoft.Barcodes.Rendering;
using Gehtsoft.Barcodes.Data;
using System.IO;
using SkiaSharp;

namespace Gehtsoft.Barcodes.UnitTests
{
    public sealed class BarcodesTests
    {
        [Theory]
        [InlineData(BarcodeType.EAN_13, "1234567890", true, "Fails because length is not 13.")]
        [InlineData(BarcodeType.EAN_13, "123456789012345", true, "Fails because length is not 13.")]
        [InlineData(BarcodeType.EAN_13, null, true, "Fails because input string is null.")]
        [InlineData(BarcodeType.EAN_13, "1256523451b45", true, "Fails because consists not only of digits.")]
        [InlineData(BarcodeType.EAN_13, "1206523451945", false, "")]
        [InlineData(BarcodeType.EAN_8, "123456", true, "Fails because length is not 8.")]
        [InlineData(BarcodeType.EAN_8, "1234567890", true, "Fails because length is not 8.")]
        [InlineData(BarcodeType.EAN_8, null, true, "Fails because input string is null.")]
        [InlineData(BarcodeType.EAN_8, "13451b45", true, "Fails because consists not only of digits.")]
        [InlineData(BarcodeType.EAN_8, "13451945", false, "")]
        [InlineData(BarcodeType.UPC_A, "12345678901", true, "Fails because length is not 12.")]
        [InlineData(BarcodeType.UPC_A, "1234567890123", true, "Fails because length is not 12.")]
        [InlineData(BarcodeType.UPC_A, null, true, "Fails because input string is null.")]
        [InlineData(BarcodeType.UPC_A, "125623451b45", true, "Fails because consists not only of digits.")]
        [InlineData(BarcodeType.UPC_A, "120623451945", false, "")]
        public void Test_ValidationOfBarcodeInputString(BarcodeType type, string input, bool shouldFail, string because)
        {
            void Validate()
            {
                BarcodesUtils.ValidateBarcodeData(input, type);
            }

            if (shouldFail)
            {
                ((Action)Validate).Should().ThrowExactly<InvalidOperationException>();
            }
            else
            {
                Validate();
            }
        }

        [Theory]
        [InlineData(MeasureBarcodeType.Percent, 20, "Create MeasureBarcodeUnit in Percent")]
        [InlineData(MeasureBarcodeType.Pixel, 100, "Create MeasureBarcodeUnit in Pixel")]
        public void Test_MeasureBarcodeUnit(MeasureBarcodeType heightType, float heightValue, string caseName)
        {
            MeasureBarcodeUnit height = new MeasureBarcodeUnit(0);
            if (heightType == MeasureBarcodeType.Percent)
            {
                height = MeasureBarcodeUnit.FromPercent(heightValue);
            }
            else if (heightType == MeasureBarcodeType.Pixel)
            {
                height = MeasureBarcodeUnit.FromPixel(heightValue);
            }
            height.Type.Should().Be(heightType);
            height.Value.Should().Be(heightValue);
        }

        [Theory]
        [InlineData(MeasureBarcodeType.Percent, 100, true, "Fails because percent is more than 99.")]
        [InlineData(MeasureBarcodeType.Percent, 0, false, "")]
        [InlineData(MeasureBarcodeType.Percent, 30, false, "")]
        [InlineData(MeasureBarcodeType.Percent, -20, true, "Fails because percent is negative.")]
        [InlineData(MeasureBarcodeType.Pixel, 0, false, "")]
        [InlineData(MeasureBarcodeType.Pixel, 50, false, "")]
        public void Test_ValidationOfInputParameters(MeasureBarcodeType heightToCutType, float heightToCutValue, bool shouldFail, string because)
        {
            void Validate()
            {
                MeasureBarcodeUnit heightToCut = new MeasureBarcodeUnit(heightToCutValue, heightToCutType);
                BarcodesUtils.ValidateInputParameters(heightToCut, 4);
            }

            if (shouldFail)
            {
                 ((Action)Validate).Should().ThrowExactly<InvalidOperationException>();
            }
            else
            {
                Validate();
            }
        }

        [Theory]
        [InlineData(BarcodeType.EAN_13, "Encoding EAN-13")]
        [InlineData(BarcodeType.EAN_8, "Encoding EAN-8")]
        [InlineData(BarcodeType.UPC_A, "Encoding UPC-A")]
        public void Test_Encoder_EAN_UPC(BarcodeType type, string caseName)
        {
            switch(type)
            {
                case BarcodeType.EAN_13:
                    BarcodesEncoder.EncodeBarcodeDataEAN_UPC(BarcodesTestData.inputTestStringEAN_13, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataEAN_13);
                    break;
                case BarcodeType.EAN_8:
                    BarcodesEncoder.EncodeBarcodeDataEAN_UPC(BarcodesTestData.inputTestStringEAN_8, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataEAN_8);
                    break;
                case BarcodeType.UPC_A:
                    BarcodesEncoder.EncodeBarcodeDataEAN_UPC(BarcodesTestData.inputTestStringUPC_A, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataUPC_A);
                    break;
            }

            Assert.True(true, caseName);
        }

        [Theory]
        [InlineData(BarcodeType.GS1_128A, "Encoding GS1_128A")]
        [InlineData(BarcodeType.GS1_128B, "Encoding GS1_128B")]
        [InlineData(BarcodeType.GS1_128C, "Encoding GS1_128C")]
        public void Test_Encoder_GS1_128(BarcodeType type, string caseName)
        {
            switch (type)
            {
                case BarcodeType.GS1_128A:
                    BarcodesEncoder.EncodeBarcodeDataGS1_128(BarcodesTestData.inputTestStringGS1_128A, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataGS1_128A);
                    break;
                case BarcodeType.GS1_128B:
                    BarcodesEncoder.EncodeBarcodeDataGS1_128(BarcodesTestData.inputTestStringGS1_128B, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataGS1_128B);
                    break;
                case BarcodeType.GS1_128C:
                    BarcodesEncoder.EncodeBarcodeDataGS1_128(BarcodesTestData.inputTestStringGS1_128C, type)
                        .Should().BeEquivalentTo(BarcodesTestData.encodedTestDataGS1_128C);
                    break;
            }

            Assert.True(true, caseName);
        }

        [Theory]
        [InlineData(BarcodeType.EAN_13, MeasureBarcodeType.Percent, 30, 4, true, false, "Rendering EAN-13, cut 30%")]
        [InlineData(BarcodeType.EAN_8, MeasureBarcodeType.Pixel, 20, 2, true, false,  "Rendering EAN-8, cut 20 pixels")]
        [InlineData(BarcodeType.UPC_A, MeasureBarcodeType.Pixel, 0, 3, true, false,  "Rendering UPC-A")]
        [InlineData(BarcodeType.EAN_13, MeasureBarcodeType.Percent, 30, 4, false, true, "Rendering EAN-13, cut 30%, rotate 90")]
        [InlineData(BarcodeType.EAN_8, MeasureBarcodeType.Pixel, 20, 2, false, true, "Rendering EAN-8, cut 20 pixels, rotate 90")]
        [InlineData(BarcodeType.UPC_A, MeasureBarcodeType.Pixel, 0, 3, false, true, "Rendering UPC-A, rotate 90")]
        public void Test_Renderer_EAN_UPC(BarcodeType type, MeasureBarcodeType heightToCutType, float heightToCutValue, int scaleMultiplier, bool hasQuiteZones, bool rotate90, string caseName)
        {
            MeasureBarcodeUnit heightToCut = new MeasureBarcodeUnit(heightToCutValue, heightToCutType);
            BarcodeRotation rotation = BarcodeRotation.Clockwise_0;
            if (rotate90)
                rotation = BarcodeRotation.Clockwise_90;
            switch (type)
            {
                case BarcodeType.EAN_13:
                    int widthShould = EANData.default_barcode_width_EAN_13 * scaleMultiplier;
                    int heightShould = BarcodesUtils.GetBarcodeHeightInt(widthShould, type);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    SKBitmap bitmap = BarcodesRenderer.DrawBitmapEAN_UPC(
                        BarcodesTestData.encodedTestDataEAN_13,
                        BarcodesTestData.inputTestStringEAN_13,
                        type, heightToCut, true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (!hasQuiteZones)
                    {
                        widthShould = widthShould - (EANData.left_quite_zone_count_EAN_13 + EANData.right_quite_zone_count) * scaleMultiplier;
                    }
                    if (rotate90)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
                case BarcodeType.EAN_8:
                    widthShould = EANData.default_barcode_width_EAN_8 * scaleMultiplier;
                    heightShould = BarcodesUtils.GetBarcodeHeightInt(widthShould, type);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    bitmap = BarcodesRenderer.DrawBitmapEAN_UPC(
                        BarcodesTestData.encodedTestDataEAN_8,
                        BarcodesTestData.inputTestStringEAN_8,
                        type, heightToCut, true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (!hasQuiteZones)
                    {
                        widthShould = widthShould - (EANData.left_quite_zone_count_EAN_8 + EANData.right_quite_zone_count) * scaleMultiplier;
                    }
                    if (rotate90)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
                case BarcodeType.UPC_A:
                    widthShould = EANData.default_barcode_width_EAN_13 * scaleMultiplier;
                    heightShould = BarcodesUtils.GetBarcodeHeightInt(widthShould, type);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    bitmap = BarcodesRenderer.DrawBitmapEAN_UPC(
                        BarcodesTestData.encodedTestDataUPC_A,
                        BarcodesTestData.inputTestStringUPC_A,
                        type, heightToCut, true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (!hasQuiteZones)
                    {
                        widthShould = widthShould - (EANData.left_quite_zone_count_EAN_13 + EANData.right_quite_zone_count) * scaleMultiplier;
                    }
                    if (rotate90)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
            }

            Assert.True(true, caseName);
        }

        [Theory]
        [InlineData(BarcodeType.GS1_128A, MeasureBarcodeType.Percent, 30, 4, true, false, "Rendering GS1-128A, cut 30%")]
        [InlineData(BarcodeType.GS1_128B, MeasureBarcodeType.Pixel, 20, 2, true, false, "Rendering GS1-128B, cut 20 pixels")]
        [InlineData(BarcodeType.GS1_128C, MeasureBarcodeType.Pixel, 0, 3, true, false, "Rendering GS1-128C")]
        [InlineData(BarcodeType.GS1_128A, MeasureBarcodeType.Percent, 30, 4, false, true, "Rendering GS1-128A, cut 30%, rotate 270")]
        [InlineData(BarcodeType.GS1_128B, MeasureBarcodeType.Pixel, 20, 2, false, true, "Rendering GS1-128B, cut 20 pixels, rotate 270")]
        [InlineData(BarcodeType.GS1_128C, MeasureBarcodeType.Pixel, 0, 3, false, true, "Rendering GS1-128C, rotate 270")]
        public void Test_Renderer_GS1_128(BarcodeType type, MeasureBarcodeType heightToCutType, float heightToCutValue, int scaleMultiplier, bool hasQuiteZones, bool rotate270, string caseName)
        {
            MeasureBarcodeUnit heightToCut = new MeasureBarcodeUnit(heightToCutValue, heightToCutType);
            float labelHeight = scaleMultiplier * 2;
            int widthShould;
            // Font calculation for label height - using SkiaSharp internally now
            labelHeight += GS1_128Data.default_font_size * scaleMultiplier;
            BarcodeRotation rotation = BarcodeRotation.Clockwise_0;
            if (rotate270)
                rotation = BarcodeRotation.Clockwise_270;
            switch (type)
            {
                case BarcodeType.GS1_128A:
                    if (hasQuiteZones)
                        widthShould = (GS1_128Data.QuietZoneMinimumWidth * 2 + BarcodesTestData.encodedTestDataGS1_128A.Length) * scaleMultiplier;
                    else
                        widthShould = BarcodesTestData.encodedTestDataGS1_128A.Length * scaleMultiplier;
                    int heightShould = (int)(widthShould * 0.15f);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    heightShould = heightShould + (int)labelHeight;
                    SKBitmap bitmap = BarcodesRenderer.DrawBitmapGS1_128(
                        BarcodesTestData.encodedTestDataGS1_128A,
                        BarcodesTestData.inputTestStringGS1_128A,
                        heightToCut,
                        true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (rotate270)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
                case BarcodeType.GS1_128B:
                    if (hasQuiteZones)
                        widthShould = (GS1_128Data.QuietZoneMinimumWidth * 2 + BarcodesTestData.encodedTestDataGS1_128B.Length) * scaleMultiplier;
                    else
                        widthShould = BarcodesTestData.encodedTestDataGS1_128B.Length * scaleMultiplier;
                    heightShould = (int)(widthShould * 0.15f);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    heightShould = heightShould + (int)labelHeight;
                    bitmap = BarcodesRenderer.DrawBitmapGS1_128(
                        BarcodesTestData.encodedTestDataGS1_128B,
                        BarcodesTestData.inputTestStringGS1_128B,
                        heightToCut,
                        true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (rotate270)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
                case BarcodeType.GS1_128C:
                    if (hasQuiteZones)
                        widthShould = (GS1_128Data.QuietZoneMinimumWidth * 2 + BarcodesTestData.encodedTestDataGS1_128C.Length) * scaleMultiplier;
                    else
                        widthShould = BarcodesTestData.encodedTestDataGS1_128C.Length * scaleMultiplier;
                    heightShould = (int)(widthShould * 0.15f);
                    heightShould = heightShould - (int)(heightShould * BarcodesUtils.GetPercentToCut(heightShould, heightToCut, scaleMultiplier) / 100);
                    heightShould = heightShould + (int)labelHeight;
                    bitmap = BarcodesRenderer.DrawBitmapGS1_128(
                        BarcodesTestData.encodedTestDataGS1_128C,
                        BarcodesTestData.inputTestStringGS1_128C,
                        heightToCut,
                        true, scaleMultiplier, null, null, hasQuiteZones, rotation);
                    if (rotate270)
                    {
                        bitmap.Width.Should().Be(heightShould);
                        bitmap.Height.Should().Be(widthShould);
                    }
                    else
                    {
                        bitmap.Width.Should().Be(widthShould);
                        bitmap.Height.Should().Be(heightShould);
                    }
                    break;
            }
            Assert.True(true, caseName);
        }
    }
}
