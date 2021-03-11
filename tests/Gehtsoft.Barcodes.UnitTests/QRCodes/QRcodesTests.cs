using System;
using System.Drawing;
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
using System.Drawing;

namespace Gehtsoft.Barcodes.UnitTests
{
    public sealed class QRCodesTests
    {
        [Theory]
        [InlineData(QRCodeErrorCorrection.L, "7DE8EB5F591400527B7302502D10BAB829E6A5AA8A742E5553CF8A08A541881F", "Low level")]
        [InlineData(QRCodeErrorCorrection.M, "A2D9FCAA7833732ED2C4ABD472F0724683FA5561AFFAC94DA6890E2E691A73ED", "Medium level")]
        [InlineData(QRCodeErrorCorrection.Q, "C10D158F700054569E02F6FA1DFDE27181351D3E9051BFA50078F820905C7D6D", "High level")]
        [InlineData(QRCodeErrorCorrection.H, "55797CA63FC4B3A0B6BD6D0DD4ACFC6CC18CC8D9D2DE885FEC091083D334207E", "Max level")]
        public void TestErrorCorrectionOfQRCode(QRCodeErrorCorrection correctionLevel, string expectedHash,
            string caseName)
        {
            // Method 1
            byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, 1);
            string hash = CalculateImageCanvasHash(data);
            hash.Should().Be(expectedHash);
            
            // Method 2
            data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, QRCodeVersion.Version1, 1, 
                Color.Black, Color.White);
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
                    (QRCodeVersion) version, 1, Color.Black, Color.White);
                hash = CalculateImageCanvasHash(data);
                hash.Should().Be(QRCodesTestData.HashSHA2[version]);
            }
        }

        [Fact]
        public void TestSetupColorsForQRCode()
        {
            var colorForeground = Color.FromArgb(255, 5, 80, 205);
            var colorBackground = Color.FromArgb(255, 200, 255, 254);
            byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, QRCodeErrorCorrection.M, QRCodeVersion.Version1, 
                1, colorForeground, colorBackground);
            
            using (var memStream = new MemoryStream(data))
            using (var bitmap = new Bitmap(memStream))
            {
                bitmap.GetPixel(0, 0).Should().BeEquivalentTo(colorBackground);
                bitmap.GetPixel(4, 4).Should().BeEquivalentTo(colorForeground);
            }
        }

        [Theory]
        [InlineData("test", "A2D9FCAA7833732ED2C4ABD472F0724683FA5561AFFAC94DA6890E2E691A73ED", 
                    false, "Minimal QR-code")]
        [InlineData("test 678901234", "115F6F461FCE021A8F747DD17AFBB06B9A78F4D7987102DD3E37537EAB91972A", 
                    false, "QR-code 1")]
        [InlineData("test 6789012345", "45C0DAEC59A4693A590A4ACB8BAC68E67429F6AE2CD510ED622F6611F890C705", 
                    false, "QR-code 2")]
        [InlineData("test test test test test test test test test test ", "38CC301644350F3CCE2699F6C4F642FAD06D437E9D14B15C00003B4B5D78F39B", 
                    false, "QR-code 2")]
        [InlineData("test", "A2D9FCAA7833732ED2C4ABD472F0724683FA5561AFFAC94DA6890E2E691A73ED",
                    true, "Minimal QR-code, method 2")]
        [InlineData("test 678901234", "115F6F461FCE021A8F747DD17AFBB06B9A78F4D7987102DD3E37537EAB91972A",
                    true, "QR-code 1, method 2")]
        [InlineData("test 6789012345", "45C0DAEC59A4693A590A4ACB8BAC68E67429F6AE2CD510ED622F6611F890C705",
                    true, "QR-code 2, method 2")]
        [InlineData("test test test test test test test test test test ", "38CC301644350F3CCE2699F6C4F642FAD06D437E9D14B15C00003B4B5D78F39B",
                    true, "QR-code 2, method 2")]
        public void TestAutoSelectVersionsOfQRCode(string text, string hashExpected, bool isMethod2, string caseName)
        {
            byte[] data = new byte[0];
            if (isMethod2)
                data = BarcodesMaker.GetQRCode(text, QRCodeEncodingMethod.Binary,
                        QRCodeErrorCorrection.M);
            else
                data = BarcodesMaker.GetQRCode(text, QRCodeEncodingMethod.Binary,
                        QRCodeErrorCorrection.M, 1, Color.Black, Color.White);
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
            var colorForeground = Color.FromArgb(255, 5, 80, 205);
            var colorBackground = Color.FromArgb(255, 200, 255, 254);

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
            using (Bitmap img = new Bitmap(new MemoryStream(dataImage)))
            {
                img.Save("delme.png", System.Drawing.Imaging.ImageFormat.Png);
                byte[] data = new byte[img.Width * img.Height];
                for (int x = 0; x < img.Width; x++)
                    for (int y = 0; y < img.Height; y++)
                        data[x + y * img.Width] = img.GetPixel(x, y).G;
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