using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Utils;

namespace Gehtsoft.Barcodes.Encoding
{
    internal static class BarcodesEncoder
    {
        /// <summary>
        /// Generates an EAN/UPC barcode data array that can be used for barcode drawing. 
        /// Supports UPC-A, EAN-13, and EAN-8 standarts
        /// </summary>
        /// <param name="data">The input data to be encoded to a barcode data array.</param>
        /// <param name="barcodeType">The barcode type. A value from the BarcodeType enum.</param>
        /// <returns></returns>

        internal static byte[] EncodeBarcodeDataEAN_UPC(string data, BarcodeType barcodeType)
        {
            int resultLength;
            string leftPart;
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    resultLength = 84;
                    int firstDigit = (int)char.GetNumericValue(data[0]);
                    leftPart = EANData.left_part_EAN13[firstDigit];
                    break;
                case BarcodeType.UPC_A:
                    resultLength = 84;
                    leftPart = "LLLLLL";
                    data = "0" + data;
                    break;
                case BarcodeType.EAN_8:
                    resultLength = 56;
                    leftPart = "LLLL";
                    data = "0" + data;
                    break;
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }

            byte[] encodedData = new byte[resultLength];

            int curDigit;
            // Loop through the left part of the input data:
            for (int i = 0; i < leftPart.Length; i++)
            {
                curDigit = (int)char.GetNumericValue(data[i + 1]);
                if (leftPart[i] == 'G')
                {
                    // Loop through the 7-bit mask of the current digit:
                    for (int j = 0; j < 7; j++)
                    {
                        encodedData[i * 7 + j] = (byte)char.GetNumericValue(EANData.G[curDigit][j]);
                    }
                }
                if (leftPart[i] == 'L')
                {
                    // Loop through the 7-bit mask of the current digit:
                    for (int j = 0; j < 7; j++)
                    {
                        encodedData[i * 7 + j] = (byte)char.GetNumericValue(EANData.L[curDigit][j]);
                    }
                }
            }
            // Loop through the right part of the input data:
            for (int i = leftPart.Length; i < 2 * leftPart.Length; i++)
            {
                curDigit = (int)char.GetNumericValue(data[i + 1]);
                // Loop through the 7-bit mask of the current digit:
                for (int j = 0; j < 7; j++)
                {
                    encodedData[i * 7 + j] = (byte)char.GetNumericValue(EANData.R[curDigit][j]);
                }
            }

            return encodedData;
        }

        /////// Code-128 ////////

        /// <summary>
        /// Generates a GS1-128 barcode data array that can be used for barcode drawing.
        /// </summary>
        /// <param name="barcodeType">You can generate a barcode using the code sets A, B or C.
        ///
        /// Code Set A includes all of the standard upper case alphanumeric characters and punctuation
        /// characters together with the symbology elements (e.g., characters with ASCII values from 00 to 95),
        /// and seven special characters.
        ///
        /// Code Set B includes all of the standard upper case alphanumeric characters and punctuation
        /// characters together with the lowercase alphabetic characters (e.g., ASCII characters 32 to 127
        /// inclusive), and seven special characters.
        ///
        /// Code Set C includes the set of 100 digit pairs from 00 to 99 inclusive, as well as three special
        /// characters. This allows numeric data to be encoded as two data digits per symbol character.</param>
        /// <param name="data">The input string to be encoded to a barcode data array.</param>
        /// <returns>Barcode data array</returns>
        public static byte[] EncodeBarcodeDataGS1_128(string data, BarcodeType barcodeType)
        {
            // Get "system" required characters
            byte[] stopCharacter = Code128EncodationTable.GetStopCharacter();
            byte[] fnc1Character = Code128EncodationTable.GetFnc1(out int fnc1CharacterValue);
            byte[] startCharacter =
                Code128EncodationTable.GetStartCharacter(barcodeType, out int startCharacterValue);

            int checksum = 0;

            int[] barcodeDataValues = Code128EncodationTable.GetBarcodeDataValues(data, barcodeType,
                out byte[] encoded);
            int[] checksumData = new int[barcodeDataValues.Length + 2];
            checksumData[0] = startCharacterValue;
            checksumData[1] = fnc1CharacterValue;
            barcodeDataValues.CopyTo(checksumData, 2);
            for (int i = 0; i < checksumData.Length; i++)
            {
                // For the 'Start B' character, the weight is 1 (as well as for A and B). Otherwise, the weight is equal to the 'i' variable.
                checksum += checksumData[i] * (i == 0 ? 1 : i);
            }

            int dividedChecksum = checksum % 103;
            byte[] dividedChecksumData = Code128EncodationTable.GetEncodedByValue(dividedChecksum.ToString());

            byte[] encodedData = new byte[startCharacter.Length + fnc1Character.Length + stopCharacter.Length +
                dividedChecksumData.Length + encoded.Length];
            startCharacter.CopyTo(encodedData, 0);
            fnc1Character.CopyTo(encodedData, 11);
            encoded.CopyTo(encodedData, 22);
            dividedChecksumData.CopyTo(encodedData, 22 + encoded.Length);
            stopCharacter.CopyTo(encodedData, encodedData.Length - 1 - stopCharacter.Length);

            return encodedData;
        }
    }
}
