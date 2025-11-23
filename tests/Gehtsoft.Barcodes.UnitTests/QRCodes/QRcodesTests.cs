using System;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.UserAPI;
using Gehtsoft.Barcodes.Utils;
using Xunit;
// ReSharper disable InconsistentNaming

namespace Gehtsoft.Barcodes.UnitTests
{
    public sealed class QRCodesTests
    {
        [Theory]
        [InlineData(QRCodeErrorCorrection.L, "66FEC15C9DAB7C3E097C0FA3A2DC2E6F9E2DF639DACA5F1F7A2C829231650752", "Low level")]
        [InlineData(QRCodeErrorCorrection.M, "78036895575404CBD890E80DE7CB79AEB3E0A42A1B58B340FE1A208314D1D3A0", "Medium level")]
        [InlineData(QRCodeErrorCorrection.Q, "BF2C368DDB24244AD1F2EADF46228492730CB803CE3EDB3158042F58AF683272", "High level")]
        [InlineData(QRCodeErrorCorrection.H, "EB957277AAC9F2DF8A872E9A7AEFEB41811797A18ADE8B0381579E1C28BDBC1E", "Max level")]
        public void TestErrorCorrectionOfQRCode(QRCodeErrorCorrection correctionLevel, string expectedHash,
            string caseName)
        {
            // Method 1
            byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, 1);
            string hash = CalculateImageCanvasHash(data);
            hash.Should().Be(expectedHash);
            
            // Method 2
            data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, QRCodeVersion.Version1, 1,
                SKColors.Black, SKColors.White);
            hash = CalculateImageCanvasHash(data);
            hash.Should().Be(expectedHash);
            
            Assert.True(true, caseName);
        }

        [Fact]
        public void TestVersionsOfQRCode()
        {
            for (int version = 1; version <= 40; version++)
            {
                // Method 1
                byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary,
                    QRCodeErrorCorrection.M, (QRCodeVersion) version);
                string hash = CalculateImageCanvasHash(data);
                hash.Should().Be(QRCodesTestData.HashSHA2[version]);

                // Method 2
                data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, QRCodeErrorCorrection.M,
                    (QRCodeVersion) version, 1, SKColors.Black, SKColors.White);
                hash = CalculateImageCanvasHash(data);
                hash.Should().Be(QRCodesTestData.HashSHA2[version]);
            }
        }

        [Fact]
        public void TestSetupColorsForQRCode()
        {
            var colorForeground = new SKColor(5, 80, 205);
            var colorBackground = new SKColor(200, 255, 254);
            byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, QRCodeErrorCorrection.M, QRCodeVersion.Version1,
                1, colorForeground, colorBackground);

            using (var memStream = new MemoryStream(data))
            using (var bitmap = SKBitmap.Decode(memStream))
            {
                bitmap.GetPixel(0, 0).Should().BeEquivalentTo(colorBackground);
                bitmap.GetPixel(4, 4).Should().BeEquivalentTo(colorForeground);
            }
        }

        [Theory]
        [InlineData("test", "78036895575404CBD890E80DE7CB79AEB3E0A42A1B58B340FE1A208314D1D3A0",
                    false, "Minimal QR-code")]
        [InlineData("test 678901234", "B8231E3BBC43E9186659B45FCE0EFC687B01DDC8F4A50110E6D30F4DC87A4D8C",
                    false, "QR-code 1")]
        [InlineData("test 6789012345", "60410737236ABFAEB7EB7F1413836FEB41BD3B5037DDC2145E00462BB245CB49",
                    false, "QR-code 2")]
        [InlineData("test test test test test test test test test test ", "29946C2C612029705EA875E2307CBD492262E9FD57B76F3338D5A0EA0CC85182",
                    false, "QR-code 2")]
        [InlineData("test", "78036895575404CBD890E80DE7CB79AEB3E0A42A1B58B340FE1A208314D1D3A0",
                    true, "Minimal QR-code, method 2")]
        [InlineData("test 678901234", "B8231E3BBC43E9186659B45FCE0EFC687B01DDC8F4A50110E6D30F4DC87A4D8C",
                    true, "QR-code 1, method 2")]
        [InlineData("test 6789012345", "60410737236ABFAEB7EB7F1413836FEB41BD3B5037DDC2145E00462BB245CB49",
                    true, "QR-code 2, method 2")]
        [InlineData("test test test test test test test test test test ", "29946C2C612029705EA875E2307CBD492262E9FD57B76F3338D5A0EA0CC85182",
                    true, "QR-code 2, method 2")]
        public void TestAutoSelectVersionsOfQRCode(string text, string hashExpected, bool isMethod2, string caseName)
        {
            byte[] data = new byte[0];
            if (isMethod2)
                data = BarcodesMaker.GetQRCode(text, QRCodeEncodingMethod.Binary,
                        QRCodeErrorCorrection.M);
            else
                data = BarcodesMaker.GetQRCode(text, QRCodeEncodingMethod.Binary,
                        QRCodeErrorCorrection.M, 1, SKColors.Black, SKColors.White);
            string hash = CalculateImageCanvasHash(data);
            hash.Should().Be(hashExpected);
            Assert.True(true, caseName);
        }

        [Theory]
        [InlineData("test", QRCodeEncodingMethod.Binary, 1, 1, 1, false, false, "Simple example")]
        [InlineData("test", QRCodeEncodingMethod.Binary, 0, -1, 1, false, true, "bad version")]
        [InlineData("test", QRCodeEncodingMethod.Binary, -1, 1, 1, true, false, "bad correction")]
        [InlineData("scale", QRCodeEncodingMethod.Binary, 1, 1, 0, false, true, "bad scale")]
        [InlineData("", QRCodeEncodingMethod.Binary, 0, 1, 1, true, false, "no text")]
        [InlineData("", QRCodeEncodingMethod.Binary, 0, 1, 1, true, false, "null text")]
        public void TestInputParamsValidation(string text, QRCodeEncodingMethod encoding, int correction, int version,
            int scaleMultiplier, bool shouldThrowArgumentException, bool shouldThrowArgumentOutOfRangeException, string caseName)
        {
            var colorForeground = new SKColor(5, 80, 205);
            var colorBackground = new SKColor(200, 255, 254);

            if (shouldThrowArgumentException)
                ((Action) call).Should().ThrowExactly<ArgumentException>();
            else if (shouldThrowArgumentOutOfRangeException)
                ((Action) call).Should().ThrowExactly<ArgumentOutOfRangeException>();
            else
                call();
            
            Assert.True(true, caseName);

            void call()
            {
                BarcodesMaker.GetQRCode(text, encoding, (QRCodeErrorCorrection) correction, (QRCodeVersion) version, scaleMultiplier,
                    colorForeground, colorBackground);
            }
        }

        [Fact]
        public void TestGetMinVersionOfQRCodeForBinaryMode()
        {
            QRCodesUtils.GetMinVersionForBinaryMode("test", QRCodeErrorCorrection.M)
                .Should().Be(1);
            QRCodesUtils.GetMinVersionForBinaryMode(string.Concat(Enumerable.Repeat("test ", 12)),
                    QRCodeErrorCorrection.Q)
                .Should().Be(5);
            QRCodesUtils.GetMinVersionForBinaryMode(string.Concat(Enumerable.Repeat("test ", 12)),
                    QRCodeErrorCorrection.H)
                .Should().Be(7);
            Assert.Throws<InvalidOperationException>(() =>
                QRCodesUtils.GetMinVersionForBinaryMode(string.Concat(Enumerable.Repeat("test ", 10000)),
                    QRCodeErrorCorrection.L));
        }

        [Theory]
        [InlineData("test", 0, false, "Simple example")]
        [InlineData("test", -1, true, "bad correction")]
        [InlineData("", 1, true, "no text")]
        [InlineData(null, 1, true, "null text")]
        public void TestInputParamsValidation_GetMinVersionOfQRCodeForBinaryMode(string text, int correction,
            bool shouldThrowArgumentException, string caseName)
        {
            if (shouldThrowArgumentException)
                ((Action) call).Should().ThrowExactly<ArgumentException>();
            else
                call();
            
            Assert.True(true, caseName);

            void call()
            {
                QRCodesUtils.GetMinVersionForBinaryMode(text, (QRCodeErrorCorrection) correction);
            }
        }

        /// <summary>
        /// Get hash from bitmap canvas
        /// </summary>
        /// <param name="dataImage">Byte array of white/black monochrome image</param>
        /// <returns>string</returns>
        private string CalculateImageCanvasHash(byte[] dataImage)
        {
            using (SKBitmap img = SKBitmap.Decode(dataImage))
            {
                File.WriteAllBytes("delme.png", dataImage);
                byte[] data = new byte[img.Width * img.Height];
                for (int x = 0; x < img.Width; x++)
                    for (int y = 0; y < img.Height; y++)
                        data[x + y * img.Width] = img.GetPixel(x, y).Green;
                return CalculateSHA2(data);
            }
        }

        private static string CalculateSHA2(byte[] buffer)
        {
            var strHash = new StringBuilder();
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(buffer);
                foreach (var b in hash)
                    strHash.Append(b.ToString("X2"));
            }
            return strHash.ToString();
        }
    }
}