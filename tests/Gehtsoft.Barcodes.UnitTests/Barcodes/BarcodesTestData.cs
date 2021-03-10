﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gehtsoft.Barcodes.UnitTests.Barcodes
{
    internal static class BarcodesTestData
    {
        internal static string inputTestStringEAN_8 = "12345678";
        internal static byte[] encodedTestDataEAN_8 = 
            {
                0, 0, 1, 1, 0, 0, 1,
                0, 0, 1, 0, 0, 1, 1,
                0, 1, 1, 1, 1, 0, 1,
                0, 1, 0, 0, 0, 1, 1,
                1, 0, 0, 1, 1, 1, 0,
                1, 0, 1, 0, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0,
                1, 0, 0, 1, 0, 0, 0
            };

        internal static string inputTestStringEAN_13 = "1234567890123";
        internal static byte[] encodedTestDataEAN_13 =
            {
                0, 0, 1, 0, 0, 1, 1,
                0, 1, 1, 1, 1, 0, 1,
                0, 0, 1, 1, 1, 0, 1,
                0, 1, 1, 0, 0, 0, 1,
                0, 0, 0, 0, 1, 0, 1,
                0, 0, 1, 0, 0, 0, 1,
                1, 0, 0, 1, 0, 0, 0,
                1, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 0, 1, 0,
                1, 1, 0, 0, 1, 1, 0,
                1, 1, 0, 1, 1, 0, 0,
                1, 0, 0, 0, 0, 1, 0
            };

        internal static string inputTestStringUPC_A = "123456789012";
        internal static byte[] encodedTestDataUPC_A =
            {
                0, 0, 1, 1, 0, 0, 1,
                0, 0, 1, 0, 0, 1, 1,
                0, 1, 1, 1, 1, 0, 1,
                0, 1, 0, 0, 0, 1, 1,
                0, 1, 1, 0, 0, 0, 1,
                0, 1, 0, 1, 1, 1, 1,
                1, 0, 0, 0, 1, 0, 0,
                1, 0, 0, 1, 0, 0, 0,
                1, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 0, 1, 0,
                1, 1, 0, 0, 1, 1, 0,
                1, 1, 0, 1, 1, 0, 0
            };

        internal static string inputTestStringGS1_128A = "1234567890ABC1234567890,;.";
        internal static byte[] encodedTestDataGS1_128A =
            {
                1, 1, 0, 1, 0, 0, 0,
                0, 1, 0, 0, 1, 1, 1,
                1, 0, 1, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 1,
                0, 0, 1, 1, 0, 1, 1,
                0, 0, 1, 1, 1, 0, 0,
                1, 0, 1, 1, 0, 0, 1,
                0, 1, 1, 1, 0, 0, 1,
                1, 0, 0, 1, 0, 0, 1,
                1, 1, 0, 1, 1, 0, 1,
                1, 1, 0, 0, 1, 0, 0,
                1, 1, 0, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 1,
                0, 1, 1, 0, 1, 1, 1,
                0, 1, 1, 1, 0, 1, 0,
                0, 1, 1, 0, 0, 1, 1,
                1, 0, 0, 1, 0, 1, 1,
                0, 0, 1, 0, 0, 1, 1,
                1, 0, 1, 1, 0, 0, 1,
                0, 1, 0, 0, 0, 1, 1,
                0, 0, 0, 1, 0, 0, 0,
                1, 0, 1, 1, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0,
                0, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 0, 1, 1,
                0, 1, 1, 0, 0, 1, 1,
                1, 0, 0, 1, 0, 1, 1,
                0, 0, 1, 0, 1, 1, 1,
                0, 0, 1, 1, 0, 0, 1,
                0, 0, 1, 1, 1, 0, 1,
                1, 0, 1, 1, 1, 0, 0,
                1, 0, 0, 1, 1, 0, 0,
                1, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 1, 1, 0,
                1, 1, 1, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 0,
                0, 1, 1, 1, 0, 0, 1,
                0, 1, 1, 0, 0, 1, 0,
                0, 1, 1, 1, 0, 1, 1,
                0, 0, 1, 0, 1, 1, 0,
                0, 1, 1, 1, 0, 0, 1,
                1, 1, 0, 1, 1, 0, 0,
                1, 0, 0, 1, 0, 0, 1,
                1, 0, 0, 1, 1, 1, 0,
                1, 1, 0, 0, 0, 0, 1,
                0, 1, 0, 1, 1, 0, 0,
                0, 1, 1, 1, 0, 1, 0,
                1, 1, 0
            };

        internal static string inputTestStringGS1_128B = "1234567890ABC1234567890abc";
        internal static byte[] encodedTestDataGS1_128B =
            {
                1, 1, 0, 1, 0, 0, 1,
                0, 0, 0, 0, 1, 1, 1,
                1, 0, 1, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 1,
                0, 0, 1, 1, 0, 1, 1,
                0, 0, 1, 1, 1, 0, 0,
                1, 0, 1, 1, 0, 0, 1,
                0, 1, 1, 1, 0, 0, 1,
                1, 0, 0, 1, 0, 0, 1,
                1, 1, 0, 1, 1, 0, 1,
                1, 1, 0, 0, 1, 0, 0,
                1, 1, 0, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 1,
                0, 1, 1, 0, 1, 1, 1,
                0, 1, 1, 1, 0, 1, 0,
                0, 1, 1, 0, 0, 1, 1,
                1, 0, 0, 1, 0, 1, 1,
                0, 0, 1, 0, 0, 1, 1,
                1, 0, 1, 1, 0, 0, 1,
                0, 1, 0, 0, 0, 1, 1,
                0, 0, 0, 1, 0, 0, 0,
                1, 0, 1, 1, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0,
                0, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 0, 1, 1,
                0, 1, 1, 0, 0, 1, 1,
                1, 0, 0, 1, 0, 1, 1,
                0, 0, 1, 0, 1, 1, 1,
                0, 0, 1, 1, 0, 0, 1,
                0, 0, 1, 1, 1, 0, 1,
                1, 0, 1, 1, 1, 0, 0,
                1, 0, 0, 1, 1, 0, 0,
                1, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 0, 1, 1, 0,
                1, 1, 1, 0, 1, 1, 1,
                0, 1, 0, 0, 1, 1, 0,
                0, 1, 1, 1, 0, 0, 1,
                0, 1, 1, 0, 0, 1, 0,
                0, 1, 1, 1, 0, 1, 1,
                0, 0, 1, 0, 0, 1, 0,
                1, 1, 0, 0, 0, 0, 1,
                0, 0, 1, 0, 0, 0, 0,
                1, 1, 0, 1, 0, 0, 0,
                0, 1, 0, 1, 1, 0, 0,
                1, 0, 0, 0, 1, 1, 0,
                0, 0, 1, 1, 1, 0, 0,
                0, 1, 1, 1, 0, 1, 0,
                1, 1, 0
            };

        internal static string inputTestStringGS1_128C = "12345678901234567890";
        internal static byte[] encodedTestDataGS1_128C =
            {
                1, 1, 0, 1, 0, 0, 1,
                1, 1, 0, 0, 1, 1, 1,
                1, 0, 1, 0, 1, 1, 1,
                0, 1, 0, 1, 1, 0, 0,
                1, 1, 1, 0, 0, 1, 0,
                0, 0, 1, 0, 1, 1, 0,
                0, 0, 1, 1, 1, 0, 0,
                0, 1, 0, 1, 1, 0, 1,
                1, 0, 0, 0, 0, 1, 0,
                1, 0, 0, 1, 1, 0, 1,
                1, 1, 1, 0, 1, 1, 0,
                1, 0, 1, 1, 0, 0, 1,
                1, 1, 0, 0, 1, 0, 0,
                0, 1, 0, 1, 1, 0, 0,
                0, 1, 1, 1, 0, 0, 0,
                1, 0, 1, 1, 0, 1, 1,
                0, 0, 0, 0, 1, 0, 1,
                0, 0, 1, 1, 0, 1, 1,
                1, 1, 0, 1, 1, 0, 1,
                0, 1, 1, 1, 1, 0, 1,
                1, 1, 1, 1, 0, 0, 0,
                1, 1, 1, 0, 1, 0, 1,
                1, 0
            };
    }
}