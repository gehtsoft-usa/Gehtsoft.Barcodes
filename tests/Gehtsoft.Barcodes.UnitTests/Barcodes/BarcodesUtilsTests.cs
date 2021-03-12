using System;
using FluentAssertions;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Utils;
using Xunit;

namespace Gehtsoft.Barcodes.UnitTests
{
    public class BarcodesUtilsTests
    {
        [Theory]
        [InlineData(BarcodeType.GS1_128A, "GEHTSOFT-USA-LLC 12345", false)]
        [InlineData(BarcodeType.GS1_128A, "(555) 555-1234", false)]
        [InlineData(BarcodeType.GS1_128A, "2GPThWXXEo8CxtgKUQ8oLvSobRKkPv7wzpkHquZqUauvPaT77p", true)] // more than 48 elements
        [InlineData(BarcodeType.GS1_128B, "Some data to encode", false)]
        [InlineData(BarcodeType.GS1_128B, "Hello world 123!", false)]
        [InlineData(BarcodeType.GS1_128C, "123456", false)]
        [InlineData(BarcodeType.GS1_128C, "12345", true)] // count of characters is not even
        [InlineData(BarcodeType.GS1_128C, "12345678901234567890", false)]
        public void Test_IsCorrectGS1_128(BarcodeType type, string inputData, bool shouldThrow)
        {
            void Validate() => BarcodesUtils.ValidateBarcodeData(inputData, type);

            if (shouldThrow)
                ((Action) Validate).Should().ThrowExactly<InvalidOperationException>();
            else
                ((Action) Validate).Should().NotThrow();
        }
    }
}