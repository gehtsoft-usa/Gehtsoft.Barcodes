using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gehtsoft.Barcodes.Enums;
using static System.Math;
using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Utils;
// ReSharper disable InconsistentNaming

namespace Gehtsoft.Barcodes.Encoding
{
    /// <summary>
    /// Describes the QR codes encoding logic.
    /// </summary>
    internal static class QRCodesEncoder
    {
        /// <summary>
        /// Creates a QR code in the binary encoding.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="version">The QR code version.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <returns>byte[]</returns>        
        public static byte[] EncodeQRCodeData(string data, int version, QRCodeErrorCorrection levelCorrection)
        {
            BitArray arrData;
            UTF8Encoding utf8 = new UTF8Encoding();
            var arrBytes = utf8.GetBytes(data);
            byte codeMethod = MethodCode(QRCodeEncodingMethod.Binary);
            int lengthStandard = QRCodesData.QRCodeMaxLengths[version - 1][(int)levelCorrection];
            arrData = new BitArray(lengthStandard);
            int bitsSaved = 0;
            // Write the header
            if (version < 10)
            {
                SaveByteToBitArray(ref arrData, codeMethod, 4);
                SaveByteToBitArray(ref arrData, (byte)arrBytes.Length);
                bitsSaved += 12;
            }
            else
            {
                SaveByteToBitArray(ref arrData, codeMethod, 4);
                SaveByteToBitArray(ref arrData, (byte)((arrBytes.Length >> 8) & 0xFF));
                SaveByteToBitArray(ref arrData, (byte)(arrBytes.Length & 0xFF));
                bitsSaved += 20;
            }
            // No space for data error
            if ((arrBytes.Length << 3) + bitsSaved + (bitsSaved % 8) > lengthStandard)
                throw new InvalidOperationException();
            // Write the data
            foreach (var byteData in arrBytes)
                SaveByteToBitArray(ref arrData, byteData);
            // Align to full bytes size
            bitsSaved += arrBytes.Length << 3;
            int bitsTail = bitsSaved % 8;
            arrData = QRCodesUtils.LeftShift(arrData, bitsTail);
            AlignToFullSize(ref arrData, bitsSaved + bitsTail, lengthStandard);

            int countBlocks = QRCodesData.QRCodeBlockCounts[version - 1][(int)levelCorrection];
            var lengthBlocks = GetLengthOfBlocks(lengthStandard, countBlocks);
            // Split the data into blocks 
            var dataBlocks = GetBlocks(lengthBlocks, arrData);
            var countCorrection = QRCodesData.QRCodeCorrectionCounts[version - 1][(int)levelCorrection];
            byte[] kCorrection = QRCodesData.QRCodeCorrectionK[(byte)countCorrection];
            // Add correction codes to the blocks and return results
            return AddCorrectionCodeToBlocks(dataBlocks, countCorrection, kCorrection);
        }

        private static byte[] AddCorrectionCodeToBlocks(byte[][] dataBlocks, int countCorrection, byte[] kCorrection)
        {
            List<byte> results = new List<byte>();
            byte[][] datasCorrection = new byte[dataBlocks.Length][];
            // Calculate the correction code for the blocks
            for (int i = 0; i < dataBlocks.Length; i++)
                datasCorrection[i] = GetCorrectionForBlocks(dataBlocks[i], 
                                                            countCorrection, kCorrection);
            // Write the data blocks to results
            for (int iByte = 0; iByte < dataBlocks[dataBlocks.Length - 1].Length; iByte++)
            {
                for (int iBlock = 0; iBlock < dataBlocks.Length; iBlock++)
                {
                    if (iByte < dataBlocks[iBlock].Length)
                        results.Add(dataBlocks[iBlock][iByte]);
                }
            }
            // Write the correction codes to results
            for (int iByte = 0; iByte < countCorrection; iByte++)
            {
                for (int iBlock = 0; iBlock < dataBlocks.Length; iBlock++)
                {
                    results.Add(datasCorrection[iBlock][iByte]);
                }
            }

            return results.ToArray();
        }

        private static byte[] GetCorrectionForBlocks(byte[] block, int countCorrection, byte[] kCorrection)
        {
            byte[] dataCorrection = new byte[countCorrection];
            List<byte> dataCurrect = new List<byte>(block);

            for (int i = 0; i < countCorrection - block.Length; i++)
                dataCurrect.Add(0);
            
            for(int i = 0; i < block.Length; i++)
            {
                var A = dataCurrect[0];
                dataCurrect.RemoveAt(0);
                dataCurrect.Add(0);
                if (A != 0)
                {
                    byte B = QRCodesData.InvertedGaluaFieldData[A];
                    for(int iCorrection = 0; iCorrection < countCorrection; iCorrection++)
                    {
                        var k = (byte)((kCorrection[iCorrection] + B) % 255);
                        var fieldData = QRCodesData.TableGaluaFieldData[k];
                        dataCurrect[iCorrection] ^= fieldData;
                    }
                }
            }

            for (int iCorrection = 0; iCorrection < countCorrection; iCorrection++)
                dataCorrection[iCorrection] = dataCurrect[iCorrection];

            return dataCorrection;
        }

        private static byte[][] GetBlocks(int[] lengthBlocks, BitArray arrData)
        {
            int indexCurrentArrDataByte = arrData.Length - 1;
            byte[][] dataBlocks = new byte[lengthBlocks.Length][];
            for (int i = 0; i < lengthBlocks.Length; i++)
            {
                byte[] block = new byte[lengthBlocks[i]];
                for (int indexByte = 0; indexByte < block.Length; indexByte++)
                {
                    block[indexByte] = GetByteFromBitArray(arrData, indexCurrentArrDataByte);
                    indexCurrentArrDataByte -= 8;
                }
                dataBlocks[i] = block;
            }
            return dataBlocks;
        }

        private static byte GetByteFromBitArray(BitArray arrData, int indexByte)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result <<= 1;
                result |= (byte) (arrData[indexByte - i] ? 1 : 0);
            }
            return result;
        }

        private static int[] GetLengthOfBlocks(int lengthStandard, int countBlocks)
        {
            int size = DivRem(lengthStandard >> 3, countBlocks, out int remainder);
            int[] result = new int[countBlocks];

            for (int i = 0; i < countBlocks; i++)
            {
                if (countBlocks - i <= remainder)
                    result[i] = size + 1;
                else
                    result[i] = size;
            }
            return result;
        }

        private static void AlignToFullSize(ref BitArray arr, int bitsTotal, int leghthStandard)
        {
            // Standard tail mask for QR codes
            byte tail1 = 0b11101100;
            byte tail2 = 0b00010001;

            while (bitsTotal < leghthStandard)
            {
                SaveByteToBitArray(ref arr, (bitsTotal >> 3) % 2 == 0 ? tail1 : tail2);
                bitsTotal += 8;
            }
        }

        private static void SaveByteToBitArray(ref BitArray arr, byte data, int countBits = 8)
        {
            byte mask = (byte) (1 << (countBits - 1));
            for (int i = 0; i < countBits; i++)
            {
                arr = QRCodesUtils.LeftShift(arr, 1);
                arr[0] = (data & mask) != 0 ? true : false;
                data <<= 1;
            }
        }

        private static byte MethodCode(QRCodeEncodingMethod encodingMethod)
        {
            switch (encodingMethod)
            {
                case QRCodeEncodingMethod.Binary:
                    return 0b0100;
            }
            // Only the binary method is implemented
            throw new NotImplementedException(encodingMethod.ToString());
        }
       
    }
}
