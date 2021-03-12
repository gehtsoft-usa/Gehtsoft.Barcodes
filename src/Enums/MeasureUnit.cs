namespace Gehtsoft.Barcodes.Enums
{
    /// <summary>
    /// Specifies a set of measurement units that can be used to specify the value for cutting the barcode height.
    /// </summary>
    public enum MeasureBarcodeType
    {
        /// <summary>
        /// Pixel.
        /// </summary>
        Pixel,
        
        /// <summary>
        /// Percent.
        /// </summary>
        Percent
    }
    
    /// <summary>
    /// Describes the measurement unit and its value.
    /// </summary>
    public class MeasureBarcodeUnit
    {
        /// <summary>
        /// The value of the measurement unit.
        /// </summary>
        public float Value { get; }
        /// <summary>
        /// The type of the measurement unit.
        /// </summary>
        public MeasureBarcodeType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureBarcodeUnit"/> class.
        /// </summary>
        /// <param name="value">The measurement unit value to set.</param>
        /// <param name="type">The measurement unit type to set.</param>
        public MeasureBarcodeUnit(float value, MeasureBarcodeType type = MeasureBarcodeType.Pixel)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureBarcodeUnit"/> class in pixels.
        /// </summary>
        /// <param name="value">The measurement unit value in pixels</param>
        public static implicit operator MeasureBarcodeUnit(float value) => 
            new MeasureBarcodeUnit(value, MeasureBarcodeType.Pixel);

        /// <summary>
        /// Creates MeasureBarcodeUnit with value in pixels
        /// </summary>
        /// <param name="value">Value in pixels</param>
        /// <returns>MeasureBarcodeUnit</returns>
        public static MeasureBarcodeUnit FromPixel(float value)
        {
            return new MeasureBarcodeUnit(value, MeasureBarcodeType.Pixel);
        }

        /// <summary>
        /// Creates MeasureBarcodeUnit with value in percent
        /// </summary>
        /// <param name="value">Value in percent</param>
        /// <returns>MeasureBarcodeUnit</returns>
        public static MeasureBarcodeUnit FromPercent(float value)
        {
            return new MeasureBarcodeUnit(value, MeasureBarcodeType.Percent);
        }
    }
}
