// ReSharper disable InconsistentNaming
namespace Gehtsoft.Barcodes.Enums
{
    /// <summary>
    /// Specifies a set of supported types of barcodes that can be drawn using AddBarcode... methods.
    /// </summary>
    public enum BarcodeType
    {
        /// <summary>
        /// The 8-digit EAN-8 barcode (European Article Number) derived from the longer EAN-13 code.
        /// </summary>
        EAN_8,
        
        /// <summary>
        /// The 13-digit EAN-13 barcode (originally European Article Number, but now renamed International Article Number).
        /// </summary>
        EAN_13,
        
        /// <summary>
        /// The 12-digit UPC-A barcode (Universal Product Code) widely used in the United States, Canada, Europe, Australia, New Zealand, and other countries.
        /// </summary>
        UPC_A,
        
        /// <summary>
        /// GS1-128 (also known as UCC-128 or EAN-128) that uses the Code 128 barcode specification with Code Set A.
        ///
        /// Code Set A includes digits, uppercase letters, and control characters, such as tab and new-line.
        /// </summary>
        GS1_128A,
        
        /// <summary>
        /// GS1-128 (also known as UCC-128 or EAN-128) that uses the Code 128 barcode specification with Code Set B.
        ///
        /// Code Set B includes digits, uppercase and lowercase letters, and some additional characters.
        /// </summary>
        GS1_128B,
        
        /// <summary>
        /// GS1-128 (also known as UCC-128 or EAN-128) that uses the Code 128 barcode specification with Code Set C.
        ///
        /// Code Set C includes digits only. It must have an even number of digits.
        /// </summary>
        GS1_128C
    }
}
