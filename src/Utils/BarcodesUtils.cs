using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Enums;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gehtsoft.Barcodes.UnitTests")]
[assembly: InternalsVisibleTo("Gehtsoft.PDFFlowLib.Barcodes")]

namespace Gehtsoft.Barcodes.Utils
{
    internal static class BarcodesUtils
    {
        internal static void ValidateBarcodeData(string data, BarcodeType barcodeType)
        {
            bool isCorrect;
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    isCorrect = IsCorrectEAN_13(data);
                    break;
                case BarcodeType.EAN_8:
                    isCorrect = IsCorrectEAN_8(data);
                    break;
                case BarcodeType.UPC_A:
                    isCorrect = IsCorrectUPC_A(data);
                    break;
                case BarcodeType.GS1_128A:
                    isCorrect = IsCorrectGS1_128A(data);
                    break;
                case BarcodeType.GS1_128B:
                    isCorrect = IsCorrectGS1_128B(data);
                    break;
                case BarcodeType.GS1_128C:
                    isCorrect = IsCorrectGS1_128C(data);
                    break;
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }
            if (!isCorrect)
                throw new System.InvalidOperationException("Barcode input data is incorrect");
        }

        internal static void ValidateInputParameters(MeasureBarcodeUnit heightToCut, int scaleMultiplier)
        {
            if ((heightToCut.Type == MeasureBarcodeType.Percent) && (heightToCut.Value > 99))
            {
                throw new System.InvalidOperationException("Cannot cut more than 99% of barcode height.");
            }
            if ((heightToCut.Type == MeasureBarcodeType.Percent) && (heightToCut.Value < 0))
            {
                throw new System.InvalidOperationException("Height to cut cannot be negative.");
            }
            if (scaleMultiplier < 1)
            {
                throw new System.InvalidOperationException("scaleMultiplier cannot be less than 1.");
            }
        }

        internal static float GetPercentToCut(float height, MeasureBarcodeUnit heightToCut, int scaleMultiplier)
        {
            switch (heightToCut.Type)
            {
                case MeasureBarcodeType.Percent:
                    {
                        if (heightToCut.Value > 99)
                           throw new InvalidOperationException("Cannot cut more than 99% of height.");
                        return heightToCut.Value;
                    }
                case MeasureBarcodeType.Pixel:
                    {
                        float percent = GetPercent(height, heightToCut.Value * scaleMultiplier);
                        if (percent > 99)
                            throw new InvalidOperationException("Cannot cut more than 99% of height.");
                        return percent;
                    }
                default:
                    throw new InvalidOperationException("Parameter cutHeight must be of type Pixel or Percent.");
            }
        }

        internal static float GetPixels(float from, MeasureBarcodeUnit howMuch)
        {
            switch (howMuch.Type)
            {
                case MeasureBarcodeType.Percent:
                    return from*howMuch.Value/100;
                case MeasureBarcodeType.Pixel:
                    return howMuch.Value;                    
                default:
                    throw new System.InvalidOperationException("Parameter cutHeight must be of type Pixel or Percent");
            }
        }

        // Calculate height depending on width and convert to int for all the barcodes: EAN, UPC and GS1-128:
        internal static int GetBarcodeHeightInt(float width, BarcodeType barcodeType)
        {
            return (int)Math.Ceiling(GetBarcodeHeight(width, barcodeType));
        }

        // Calculate height depending on width for all the barcodes: EAN, UPC and GS1-128:
        internal static float GetBarcodeHeight(float width, BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    return EANData.height_EAN_13 * width / EANData.width_EAN_13;
                case BarcodeType.EAN_8:
                    return EANData.height_EAN_8 * width / EANData.width_EAN_8;
                case BarcodeType.UPC_A:
                    return EANData.height_EAN_13 * width / EANData.width_EAN_13;
                case BarcodeType.GS1_128A:
                case BarcodeType.GS1_128B:
                case BarcodeType.GS1_128C:
                    return width * 0.15f;
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }
        }

        internal static int GetStrokeHeight(float width, BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    return (int)Math.Ceiling(EANData.stroke_height_EAN_13 * width / EANData.width_EAN_13);
                case BarcodeType.EAN_8:
                    return (int)Math.Ceiling(EANData.stroke_height_EAN_8 * width / EANData.width_EAN_8);
                case BarcodeType.UPC_A:
                    return (int)Math.Ceiling(EANData.stroke_height_EAN_13 * width / EANData.width_EAN_13);
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }
        }

        internal static float GetStrokeWidth(float width, BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    return (EANData.stroke_width * width / EANData.width_EAN_13);
                case BarcodeType.EAN_8:
                    return (EANData.stroke_width * width / EANData.width_EAN_8);
                case BarcodeType.UPC_A:
                    return (EANData.stroke_width * width / EANData.width_EAN_13);
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }
        }

        internal static float GetPercent(float from, float howMuch)
        {
            return 100 * howMuch / from;
        }

        internal static int GetSeparatorHeight(float width, BarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    return (int)Math.Ceiling(EANData.separator_height_EAN_13 * width / EANData.width_EAN_13);
                case BarcodeType.EAN_8:
                    return (int)Math.Ceiling(EANData.separator_height_EAN_8 * width / EANData.width_EAN_8);
                case BarcodeType.UPC_A:
                    return (int)Math.Ceiling(EANData.separator_height_EAN_13 * width / EANData.width_EAN_13);
                default:
                    throw new System.NotImplementedException("BarcodeType is not supported");
            }
        }

        private static bool IsCorrectEAN_13(string str)
        {
            if ((str is null) || (str.Length != 13))
                return false;
            return IsDigitsOnly(str);
        }
        private static bool IsCorrectEAN_8(string str)
        {
            if ((str is null) || (str.Length != 8))
                return false;
            return IsDigitsOnly(str);
        }
        private static bool IsCorrectUPC_A(string str)
        {
            if ((str is null) || (str.Length != 12))
                return false;
            return IsDigitsOnly(str);
        }
        private static bool IsCorrectGS1_128A(string str)
        {
            return !(str is null) && str.Length > 0 && str.Length <= 48;
        }
        private static bool IsCorrectGS1_128B(string str)
        {
            return !(str is null) && str.Length > 0 && str.Length <= 48;
        }
        private static bool IsCorrectGS1_128C(string str)
        {
            if ((str is null) || (str.Length % 2 == 1))
                return false;
            return IsDigitsOnly(str);
        }
        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
                if (c < '0' || c > '9')
                    return false;
            return true;
        }
    }
}
