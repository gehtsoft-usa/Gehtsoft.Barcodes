using Gehtsoft.Barcodes.Encoding;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Rendering;
using Gehtsoft.Barcodes.Utils;

namespace Gehtsoft.Barcodes.UserAPI
{
    public static partial class BarcodesMaker
    {
        /// <summary>
        /// Generates a barcode image from the input data and returns the image as an array of bytes.
        /// </summary>
        /// <param name="data">The input data for the barcode.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="lineColor">The color of barcode lines.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="showDataLabel">Defines whether the input data is printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines to reduce the standard height.</param>     
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="hasQuietZones">Defines whether the QR code has quiet zones.</param>
        /// <returns>Array of bytes</returns>     
        public static byte[] GetBarcode(string data, BarcodeType barcodeType, System.Drawing.Color lineColor, System.Drawing.Color backColor, bool showDataLabel, MeasureBarcodeUnit heightToCut, int scaleMultiplier = 2, bool hasQuietZones = true)
        {
            // Validate the data depending on barcodeType:
            BarcodesUtils.ValidateBarcodeData(data, barcodeType);

            // Validate the input parameters:          
            BarcodesUtils.ValidateInputParameters(heightToCut, scaleMultiplier);

            if (barcodeType == BarcodeType.EAN_8 || barcodeType == BarcodeType.EAN_13 || barcodeType == BarcodeType.UPC_A)
            {             
                byte[] imgData = GetBarcodeEAN_UPC(data, barcodeType, lineColor, backColor, showDataLabel, heightToCut, scaleMultiplier, hasQuietZones);
                return imgData;
            }
            else
            {               
                byte[] imgData = GetBarcodeGS1_128(data, barcodeType, lineColor, backColor, showDataLabel, heightToCut, scaleMultiplier, hasQuietZones);
                return imgData;
            }
        }

        /// <summary>
        /// Generates an EAN-8, EAN-13, or UPC-A barcode image from the input data and returns the image as an array of bytes.
        /// </summary>
        /// <param name="data">The input data for the barcode.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="lineColor">The color of barcode lines.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="showDataLabel">Defines whether the input data is printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines to reduce the standard height.</param>      
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="hasQuietZones">Defines whether the QR code has quiet zones.</param>
        /// <returns>Array of bytes</returns>
        internal static byte[] GetBarcodeEAN_UPC(string data, BarcodeType barcodeType, System.Drawing.Color lineColor, System.Drawing.Color backColor, bool showDataLabel, MeasureBarcodeUnit heightToCut, int scaleMultiplier = 2, bool hasQuietZones = true)
        {
            // Process the data and encode it:
            byte[] encodedData = BarcodesEncoder.EncodeBarcodeDataEAN_UPC(data, barcodeType);

            // Create an image using encodedData and save the image to a byte array:  
            byte[] imgData = BarcodesRenderer.GetBarcodeImageEAN_UPC(encodedData, data, barcodeType, showDataLabel, heightToCut, scaleMultiplier, lineColor, backColor, hasQuietZones);
            return imgData;
        }

        /// <summary>
        /// Generates a GS1-128 barcode image from the input data and returns the image as an array of bytes.
        /// </summary>
        /// <param name="data">The input data for the barcode.</param>
        /// <param name="barcodeType">The barcode type.</param>
        /// <param name="lineColor">The color of barcode lines.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="showDataLabel">Defines whether the input data is printed under the barcode lines.</param>
        /// <param name="heightToCut">The height in pixels or in percent to be cut from the top of the barcode lines to reduce the standard height.</param>
        /// <param name="scaleMultiplier">The multiplier of the barcode width for better text rendering.</param>
        /// <param name="hasQuietZones">Defines whether the QR code has quiet zones.</param>
        /// <returns>Array of bytes</returns>

        internal static byte[] GetBarcodeGS1_128(string data, BarcodeType barcodeType, System.Drawing.Color lineColor, System.Drawing.Color backColor, bool showDataLabel, MeasureBarcodeUnit heightToCut, int scaleMultiplier = 2, bool hasQuietZones = true)
        {
            // Process the data and encode it:
            byte[] encodedData = BarcodesEncoder.EncodeBarcodeDataGS1_128(data, barcodeType);
            
            // Create an image using encodedData and save the image to a byte array:
            byte[] imgData = BarcodesRenderer.GetBarcodeImageGS1_128(encodedData, data, heightToCut, showDataLabel, scaleMultiplier, strokeColor: lineColor, backColor: backColor, hasQuietZones);
            return imgData;
        }
    }
}
