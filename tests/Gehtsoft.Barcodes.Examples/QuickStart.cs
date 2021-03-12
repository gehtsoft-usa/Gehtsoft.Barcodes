using System;
using Xunit;
using System.IO;
using System.Drawing;
using Gehtsoft.Barcodes.Examples.Extensions;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.UserAPI;
using FluentAssertions;

namespace Gehtsoft.Barcodes.Examples
{
    public class QuickStart
    {
        [Fact]
        public void QuickStart_BarcodeEAN8()
        {
            byte[] dataBarcode8 = BarcodesMaker.GetBarcode("01234567",
                                                          BarcodeType.EAN_8,
                                                          Color.Black,
                                                          Color.White,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0));
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode8)))
            {
                image.Save("barcodeEAN8.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeEAN8.png", "barcodeEAN8.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeEAN13()
        {
            byte[] dataBarcode13 = BarcodesMaker.GetBarcode("0123456789123",
                                                          BarcodeType.EAN_13,
                                                          Color.Black,
                                                          Color.White,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0));
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
            {
                image.Save("barcodeEAN13.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeEAN13.png", "barcodeEAN13.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeGS128()
        {
            byte[] dataBarcodeGS128 = BarcodesMaker.GetBarcode("012345678912ABCD",
                                                          BarcodeType.GS1_128A,
                                                          Color.Black,
                                                          Color.White,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0));
            using (Image image = Image.FromStream(new MemoryStream(dataBarcodeGS128)))
            {
                image.Save("barcodeGS128.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeGS128.png", "barcodeGS128.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeQRCode()
        { 
            byte[] dataQR = BarcodesMaker.GetQRCode("QRCode example",
                                                    QRCodeEncodingMethod.Binary,
                                                    QRCodeErrorCorrection.M,
                                                    8);
            using (Image image = Image.FromStream(new MemoryStream(dataQR)))
            {
                image.Save("QRCode.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_QRCode.png", "QRCode.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeEAN8_Colors_NoQuiteZones()
        {
            byte[] dataBarcode8 = BarcodesMaker.GetBarcode("01234567",
                                                          BarcodeType.EAN_8,
                                                          Color.Blue,
                                                          Color.Yellow,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0),
                                                          2,
                                                          false);
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode8)))
            {
                image.Save("barcodeEAN8_2.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeEAN8_2.png", "barcodeEAN8_2.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeEAN13_Colors_NoQuiteZones()
        {
            byte[] dataBarcode13 = BarcodesMaker.GetBarcode("0123456789123",
                                                          BarcodeType.EAN_13,
                                                          Color.Green,
                                                          Color.White,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0),
                                                          2,
                                                          false);
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
            {
                image.Save("barcodeEAN13_2.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeEAN13_2.png", "barcodeEAN13_2.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeGS128_Colors_NoQuiteZones()
        {
            byte[] dataBarcodeGS128 = BarcodesMaker.GetBarcode("012345678912ABCD",
                                                          BarcodeType.GS1_128A,
                                                          Color.Red,
                                                          Color.Gray,
                                                          true,
                                                          MeasureBarcodeUnit.FromPixel(0),
                                                          2, 
                                                          false);
            using (Image image = Image.FromStream(new MemoryStream(dataBarcodeGS128)))
            {
                image.Save("barcodeGS128_2.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_barcodeGS128_2.png", "barcodeGS128_2.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_BarcodeQRCode_Colors_NoQuiteZones()
        {
            byte[] dataQR = BarcodesMaker.GetQRCode("QRCode example",
                                                    QRCodeEncodingMethod.Binary,
                                                    QRCodeErrorCorrection.M,
                                                    8, 
                                                    Color.Black, Color.Green, 
                                                    false);
            using (Image image = Image.FromStream(new MemoryStream(dataQR)))
            {
                image.Save("QRCode_2.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_QRCode_2.png", "QRCode_2.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_Barcode_Rotation_90_clockwise()
        {
            byte[] dataBarcode13 = BarcodesMaker.GetBarcode("0123456789123",
                                                          BarcodeType.EAN_13,
                                                          Color.Black,
                                                          Color.White,
                                                          true,
                                                          0,
                                                          2,
                                                          true,
                                                          BarcodeRotation.Clockwise_90);
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
            {
                image.Save("rotation_90_clockwise.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_rotation_90_clockwise.png", "rotation_90_clockwise.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_Barcode_Rotation_180_clockwise()
        {
            byte[] dataBarcode13 = BarcodesMaker.GetBarcode("123456789012",
                                                          BarcodeType.UPC_A,
                                                          Color.Black,
                                                          Color.White,
                                                          true,
                                                          0,
                                                          2,
                                                          true,
                                                          BarcodeRotation.Clockwise_180);
            using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
            {
                image.Save("rotation_180_clockwise.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            FileComparer.AreEqual("etalon_rotation_180_clockwise.png", "rotation_180_clockwise.png").Should().Be(true);
        }

        [Fact]
        public void QuickStart_Barcode_Rotation_270_clockwise()
        {
        byte[] dataBarcode13 = BarcodesMaker.GetBarcode("ABC0123456789123abc,;.",
                                                        BarcodeType.GS1_128B,
                                                        Color.Black,
                                                        Color.White,
                                                        true,
                                                        0,
                                                        2,
                                                        true,
                                                        BarcodeRotation.Clockwise_270);
        using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
        {
            image.Save("rotation_270_clockwise.png", System.Drawing.Imaging.ImageFormat.Png);
        }

            FileComparer.AreEqual("etalon_rotation_270_clockwise.png", "rotation_270_clockwise.png").Should().Be(true);
        }

    }
}
