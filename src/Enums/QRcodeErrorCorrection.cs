// ReSharper disable InconsistentNaming
namespace Gehtsoft.Barcodes.Enums
{
    /// <summary>
    /// Specifies a set of supported QR code error correction levels.
    /// </summary>
    public enum QRCodeErrorCorrection
    {
        /// <summary>
        /// A low level of error correction - 7% of data bytes can be restored.
        /// </summary>
        L = 0, // must zero
        
        /// <summary>
        /// A medium level of error correction - 15% of data bytes can be restored.
        /// </summary>
        M,
        
        /// <summary>
        /// A quartile level of error correction - 25% of data bytes can be restored.
        /// </summary>
        Q,
        
        /// <summary>
        /// A high level of error correction - 30% of data bytes can be restored.
        /// </summary>
        H
    }
}