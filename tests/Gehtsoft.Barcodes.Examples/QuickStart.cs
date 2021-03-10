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
    }
}
