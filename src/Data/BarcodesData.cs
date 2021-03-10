using System;
using System.Collections.Generic;
using System.Linq;
using Gehtsoft.Barcodes.Enums;
// ReSharper disable InconsistentNaming

namespace Gehtsoft.Barcodes.Data
{
    internal static class EANData
    {
        internal static string[] L = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        internal static string[] G = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        internal static string[] R = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
        internal static string[] left_part_EAN13 = { "LLLLLL", "LLGLGG", "LLGGLG", "LLGGGL", "LGLLGG", "LGGLLG", "LGGGLL", "LGLGLG", "LGLGGL", "LGGLGL" };
        internal const float width_EAN_13 = 37.29f;
        internal const float height_EAN_13 = 25.93f;
        internal const float stroke_height_EAN_13 = 22.85f;
        internal const float separator_height_EAN_13 = 24.50f;
        internal const float width_EAN_8 = 26.73f;
        internal const float height_EAN_8 = 21.64f;
        internal const float stroke_height_EAN_8 = 18.23f;
        internal const float separator_height_EAN_8 = 18.23f + 1.65f;
        internal const float stroke_width = 0.33f;
        internal const int default_barcode_width_EAN_13 = 113; //(11+3+(7*6)+5+(7*6)+3+7)
        internal const int default_barcode_width_EAN_8 = 81; // (7+3+(7*4)+5+(7*4)+3+7)
        internal const float default_font_size = 10f;
        internal const float default_font_small_size = 8f;
        internal const int dots_per_inch = 600;
        internal const string font_family_name = "Microsoft Sans Serif";
        internal const int left_quite_zone_count_EAN_13 = 11;
        internal const int left_quite_zone_count_EAN_8 = 7;
        internal const int right_quite_zone_count = 7;
    }
    internal static class GS1_128Data
    {
        internal const int QuietZoneMinimumWidth = 24;
        internal const int MinimumBarcodeHeight = 48;
        internal const float default_font_size = 10f;
        internal const int dots_per_inch = 600;
        internal const string font_family_name = "Microsoft Sans Serif";
    }
    internal static class Code128EncodationTable
    {
        private static readonly List<Code128EncodationEntry> Entries = new List<Code128EncodationEntry>(107);

        private static void AddEntry(string value, string a, string encoded, string b = null, string c = null)
        {
            Entries.Add(new Code128EncodationEntry(value, a, b ?? a, c ?? value.PadLeft(2, '0'), encoded));
        }

        static Code128EncodationTable()
        {
            AddEntry("0", " ",  "11011001100");
            AddEntry("1", "!", "11001101100");
            AddEntry("2", "\"", "11001100110");
            AddEntry("3", "#", "10010011000");
            AddEntry("4", "$", "10010001100");
            AddEntry("5", "%", "10001001100");
            AddEntry("6", "&", "10011001000");
            AddEntry("7", "'", "10011000100");
            AddEntry("8", "(", "10001100100");
            AddEntry("9", ")", "11001001000");
            
            AddEntry("10", "*", "11001000100");
            AddEntry("11", "+", "11000100100");
            AddEntry("12", ",", "10110011100");
            AddEntry("13", "-", "10011011100");
            AddEntry("14", ".", "10011001110");
            AddEntry("15", "/", "10111001100");
            AddEntry("16", "0", "10011101100");
            AddEntry("17", "1", "10011100110");
            AddEntry("18", "2", "11001110010");
            AddEntry("19", "3", "11001011100");
            
            AddEntry("20", "4", "11001001110");
            AddEntry("21", "5", "11011100100");
            AddEntry("22", "6", "11001110100");
            AddEntry("23", "7", "11101101110");
            AddEntry("24", "8", "11101001100");
            AddEntry("25", "9", "11100101100");
            AddEntry("26", ":", "11100100110");
            AddEntry("27", ";", "11101100100");
            AddEntry("28", "<", "11100110100");
            AddEntry("29", "=", "11100110010");
            
            AddEntry("30", ">", "11011011000");
            AddEntry("31", "?", "11011000110");
            AddEntry("32", "@", "11000110110");
            AddEntry("33", "A", "10100011000");
            AddEntry("34", "B", "10001011000");
            AddEntry("35", "C", "10001000110");
            AddEntry("36", "D", "10110001000");
            AddEntry("37", "E", "10001101000");
            AddEntry("38", "F", "10001100010");
            AddEntry("39", "G", "11010001000");
            
            AddEntry("40", "H", "11000101000");
            AddEntry("41", "I", "11000100010");
            AddEntry("42", "J", "10110111000");
            AddEntry("43", "K", "10110001110");
            AddEntry("44", "L", "10001101110");
            AddEntry("45", "M", "10111011000");
            AddEntry("46", "N", "10111000110");
            AddEntry("47", "O", "10001110110");
            AddEntry("48", "P", "11101110110");
            AddEntry("49", "Q", "11010001110");
            
            AddEntry("50", "R", "11000101110");
            AddEntry("51", "S", "11011101000");
            AddEntry("52", "T", "11011100010");
            AddEntry("53", "U", "11011101110");
            AddEntry("54", "V", "11101011000");
            AddEntry("55", "W", "11101000110");
            AddEntry("56", "X", "11100010110");
            AddEntry("57", "Y", "11101101000");
            AddEntry("58", "Z", "11101100010");
            AddEntry("59", "[", "11100011010");
            
            AddEntry("60", "\\", "11101111010");
            AddEntry("61", "]", "11001000010");
            AddEntry("62", "^", "11110001010");
            AddEntry("63", "_", "10100110000");
            AddEntry("64", "\0", "10100001100", "`");
            AddEntry("65", Convert.ToChar(1).ToString(), "10010110000", "a");
            AddEntry("66", Convert.ToChar(2).ToString(), "10010000110", "b");
            AddEntry("67", Convert.ToChar(3).ToString(), "10000101100", "c");
            AddEntry("68", Convert.ToChar(4).ToString(), "10000100110", "d");
            AddEntry("69", Convert.ToChar(5).ToString(), "10110010000", "e");
            
            AddEntry("70", Convert.ToChar(6).ToString(), "10110000100", "f");
            AddEntry("71", Convert.ToChar(7).ToString(), "10011010000", "g");
            AddEntry("72", Convert.ToChar(8).ToString(), "10011000010", "h");
            AddEntry("73", Convert.ToChar(9).ToString(), "10000110100", "i");
            AddEntry("74", Convert.ToChar(10).ToString(), "10000110010", "j");
            AddEntry("75", Convert.ToChar(11).ToString(), "11000010010", "k");
            AddEntry("76", Convert.ToChar(12).ToString(), "11001010000", "l");
            AddEntry("77", Convert.ToChar(13).ToString(), "11110111010", "m");
            AddEntry("78", Convert.ToChar(14).ToString(), "11000010100", "n");
            AddEntry("79", Convert.ToChar(15).ToString(), "10001111010", "o");
            
            AddEntry("80", Convert.ToChar(16).ToString(), "10100111100", "p");
            AddEntry("81", Convert.ToChar(17).ToString(), "10010111100", "q");
            AddEntry("82", Convert.ToChar(18).ToString(), "10010011110", "r");
            AddEntry("83", Convert.ToChar(19).ToString(), "10111100100", "s");
            AddEntry("84", Convert.ToChar(20).ToString(), "10011110100", "t");
            AddEntry("85", Convert.ToChar(21).ToString(), "10011110010", "u");
            AddEntry("86", Convert.ToChar(22).ToString(), "11110100100", "v");
            AddEntry("87", Convert.ToChar(23).ToString(), "11110010100", "w");
            AddEntry("88", Convert.ToChar(24).ToString(), "11110010010", "x");
            AddEntry("89", Convert.ToChar(25).ToString(), "11011011110", "y");
            
            AddEntry("90", Convert.ToChar(26).ToString(), "11011110110", "z");
            AddEntry("91", Convert.ToChar(27).ToString(), "11110110110", "{");
            AddEntry("92", Convert.ToChar(28).ToString(), "10101111000", "|");
            AddEntry("93", Convert.ToChar(29).ToString(), "10100011110", "}");
            AddEntry("94", Convert.ToChar(30).ToString(), "10001011110", "~");
            AddEntry("95", Convert.ToChar(31).ToString(), "10111101000", Convert.ToChar(127).ToString());
            AddEntry("96", Convert.ToChar(202).ToString(), "10111100010", Convert.ToChar(202).ToString());
            AddEntry("97", Convert.ToChar(201).ToString(), "11110101000", Convert.ToChar(201).ToString());
            AddEntry("98", "SHIFT", "11110100010");
            AddEntry("99", "CODE_C", "10111011110");
            
            AddEntry("100", "CODE_B", "10111101110", Convert.ToChar(203).ToString(), "CODE_B");
            AddEntry("101", Convert.ToChar(203).ToString(), "11101011110", "CODE_A", "CODE_A");
            AddEntry("102", Convert.ToChar(200).ToString(), "11110101110", c: Convert.ToChar(200).ToString());
            AddEntry("103", "START_A", "11010000100", c: "START_A");
            AddEntry("104", "START_B", "11010010000", c: "START_B");
            AddEntry("105", "START_C", "11010011100", c: "START_C");
            
            AddEntry("", "STOP", "1100011101011", c: "STOP");
            
            // 200 is FNC1
            // 201 is FNC2
            // 202 is FNC3
            // 203 is FNC4
        }

        public static byte[] GetStopCharacter() =>
            Entries[Entries.Count - 1].Encoded;

        public static byte[] GetStartCharacter(BarcodeType codeSet, out int startCharacterValue)
        {
            switch (codeSet)
            {
                case BarcodeType.GS1_128A:
                    startCharacterValue = 103;
                    return GetEncodedByValue("103");
                
                case BarcodeType.GS1_128B:
                    startCharacterValue = 104;
                    return GetEncodedByValue("104");
                
                case BarcodeType.GS1_128C:
                    startCharacterValue = 105;
                    return GetEncodedByValue("105");
                
                default:
                    throw new NotSupportedException();
            }
        }

        public static byte[] GetFnc1(out int fnc1CharacterValue)
        {
            fnc1CharacterValue = 102;
            return GetEncodedByValue("102");
        }

        public static byte[] GetEncodedByValue(string value) =>
            Entries.First(e => e.Value == value).Encoded;

        public static int[] GetBarcodeDataValues(string barcodeDataString, BarcodeType codeSet, out byte[] encoded)
        {
            if (codeSet == BarcodeType.GS1_128A || codeSet == BarcodeType.GS1_128B)
            {
                encoded = new byte[barcodeDataString.Length * 11];
                int[] barcodeDataValues = new int[barcodeDataString.Length];
                for (int i = 0; i < barcodeDataString.Length; i++)
                {
                    string character = barcodeDataString[i].ToString();
                    Code128EncodationEntry entry = codeSet == BarcodeType.GS1_128A
                        ? Entries.First(e => e.A == character)
                        : Entries.First(e => e.B == character);
                    int characterValue = int.Parse(entry.Value);
                    barcodeDataValues[i] = characterValue;
                    entry.Encoded.CopyTo(encoded, i * 11);
                }

                return barcodeDataValues;
            }
            else
            {
                int[] barcodeDataValues = new int[barcodeDataString.Length / 2];
                encoded = new byte[barcodeDataValues.Length * 11];
                for (int i = 0, j = 0; i < barcodeDataString.Length; i += 2, j++)
                {
                    string characterC = barcodeDataString.Substring(i, 2);
                    var entry = Entries.First(e => e.C == characterC);
                    var characterCValue = int.Parse(entry.Value);
                    barcodeDataValues[j] = characterCValue;
                    entry.Encoded.CopyTo(encoded, j * 11);
                }

                return barcodeDataValues;
            }
        }
    }
    
    internal sealed class Code128EncodationEntry
    {
        public Code128EncodationEntry(string value, string a, string b, string c, string encoded)
        {
            Value = value;
            A = a;
            B = b;
            C = c;
            byte[] encodedBytes = new byte[encoded.Length];
            for (int i = 0; i < encoded.Length; i++)
            {
                encodedBytes[i] = (byte) (encoded[i] - '0');
            }
            Encoded = encodedBytes;
        }
        
        public string Value { get; }
        public string A { get; }
        public string B { get; }
        public string C { get; }
        public byte[] Encoded { get; }
    }
}