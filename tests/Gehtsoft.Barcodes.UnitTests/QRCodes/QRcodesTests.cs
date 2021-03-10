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

namespace Gehtsoft.Barcodes.UnitTests
{
    public sealed class QRCodesTests
    {
        [Theory]
        [InlineData(QRCodeErrorCorrection.L, "ACC40A7C27368A024EFE8747EF491B6D094C4EE1E1C7798FD6A857C977D9E073", "Low level")]
        [InlineData(QRCodeErrorCorrection.M, "AB15C5C89550D796361B85587120CDC0712AF6A9BEF3E77AEC2E9D1634DC0903", "Medium level")]
        [InlineData(QRCodeErrorCorrection.Q, "DAD52BD28B4475D8FCFFEE347242B75197056C2F00DFE3F08EA9A4151375EFFF", "High level")]
        [InlineData(QRCodeErrorCorrection.H, "1E239808ABED8BEBFD745C5849E1C3DE88798150FBEB3DD9236F35F6E6DE4CBB", "Max level")]
        public void TestErrorCorrectionOfQRCode(QRCodeErrorCorrection correctionLevel, string expectedHash,
            string caseName)
        {
            // Method 1
            byte[] data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, 1);
            string hash = CalculateSHA2(data);
            hash.Should().Be(expectedHash);
            
            // Method 2
            data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, correctionLevel, QRCodeVersion.Version1, 1, 
                Color.Black, Color.White);
            hash = CalculateSHA2(data);
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
                string hash = CalculateSHA2(data);
                hash.Should().Be(QRCodesTestData.HashSHA2[version]);
                
                // Method 2
                data = BarcodesMaker.GetQRCode("test", QRCodeEncodingMethod.Binary, QRCodeErrorCorrection.M,
                    (QRCodeVersion) version, 1, Color.Black, Color.White);
                hash = CalculateSHA2(data);
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
        [InlineData("test", "AB15C5C89550D796361B85587120CDC0712AF6A9BEF3E77AEC2E9D1634DC0903", 
                    false, "Minimal QR-code")]
        [InlineData("test 678901234", "3A0BEB3EE0A9FA4E0681F8A8A184128A6E303B1B454E1262568E8FC0F29C5E0B", 
                    false, "QR-code 1")]
        [InlineData("test 6789012345", "F7ED21EA0EFB9DCC813AA45D1A0ED370AF956F2E12725A58045540248C39C66A", 
                    false, "QR-code 2")]
        [InlineData("test test test test test test test test test test ", "FFDB4DE6AF778AEB3B0C691B14B205FC904F6ED0667D3149296ABE0B48AAF47F", 
                    false, "QR-code 2")]
        [InlineData("test", "AB15C5C89550D796361B85587120CDC0712AF6A9BEF3E77AEC2E9D1634DC0903",
                    true, "Minimal QR-code, method 2")]
        [InlineData("test 678901234", "3A0BEB3EE0A9FA4E0681F8A8A184128A6E303B1B454E1262568E8FC0F29C5E0B",
                    true, "QR-code 1, method 2")]
        [InlineData("test 6789012345", "F7ED21EA0EFB9DCC813AA45D1A0ED370AF956F2E12725A58045540248C39C66A",
                    true, "QR-code 2, method 2")]
        [InlineData("test test test test test test test test test test ", "FFDB4DE6AF778AEB3B0C691B14B205FC904F6ED0667D3149296ABE0B48AAF47F",
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
            string hash = CalculateSHA2(data);
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