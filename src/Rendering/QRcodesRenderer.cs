using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Gehtsoft.Barcodes.Data;
using Gehtsoft.Barcodes.Encoding;
using Gehtsoft.Barcodes.Enums;
using Gehtsoft.Barcodes.Utils;
using System;

namespace Gehtsoft.Barcodes.Rendering
{
    /// <summary>
    /// Describes the logic of QR codes rendering.
    /// </summary>
    internal static class QRCodesRenderer
    {
        /// <summary>
        /// Renders a QR code on bitmap from the encoded data and returns an array of bytes.
        /// </summary>
        /// <param name="data">The user data for encoding.</param>
        /// <param name="encoding">The QR code encoding.</param>
        /// <param name="levelCorrection">The level of error correction.</param>
        /// <param name="version">The QR code version.</param>
        /// <param name="scaleMultiplier">The pixel scaling of the resulting QR code image.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="quietZone">The size of the quiet zone.</param>
        /// <returns>byte[]</returns>
        internal static byte[] GetQRCodeImage(string data, QRCodeEncodingMethod encoding, QRCodeErrorCorrection levelCorrection, QRCodeVersion version, int scaleMultiplier, Color foregroundColor, Color backgroundColor, int quietZone = 4)
        {
            if (!Enum.IsDefined(typeof(QRCodeVersion), version))
                throw new ArgumentOutOfRangeException();

            // Only the binary method is implemented
            if (encoding != QRCodeEncodingMethod.Binary)
                throw new NotImplementedException();

            int numVersion;

            if (version == QRCodeVersion.Automatic)
                numVersion = (int)QRCodesUtils.GetMinVersionForBinaryMode(data, levelCorrection);
            else
                numVersion = (int)version;

            var numberMask = 4;


            // Get the image data array
            byte[] dataBinary = QRCodesEncoder.EncodeQRCodeData(data, numVersion, levelCorrection);

            // Create a bitmap for the QR code
            var bitmapSource = CreateBitmap(dataBinary, numVersion, numberMask, levelCorrection, 
                                            new SolidBrush(foregroundColor), 
                                            new SolidBrush(backgroundColor), 
                                            quietZone);
            // Pixel scaling

            var bitmap = QRCodesUtils.Scale(bitmapSource, scaleMultiplier);

            // Save the resulting image in the PNG format
            var memStream = new MemoryStream();
            bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
            memStream.Flush();
            var qrData = memStream.ToArray();
            memStream.Close();
            return qrData;
        }

        private static Bitmap CreateBitmap(byte[] dataBinary, int version, int numberMask, QRCodeErrorCorrection levelCorrection, Brush foregroundColor, Brush backgroundColor, int margin)
        {
            var foregroundBrush = (SolidBrush)foregroundColor;
            int sizeCanvas = 21 + 4 * (version - 1);
            int size = sizeCanvas + 2 * margin;
            var image = new Bitmap(size, size);
            var imageMaskQRCode = new Bitmap(size, size); // system zones

            // Create a base system template
            using (Graphics graph = Graphics.FromImage(image))
            {
                using (Graphics graphMaskQRCode = Graphics.FromImage(imageMaskQRCode))
                {
                    graph.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graph.SmoothingMode = SmoothingMode.None;
                    graph.PixelOffsetMode = PixelOffsetMode.Half;
                    graph.PageUnit = GraphicsUnit.Pixel;
                    graph.FillRectangle(backgroundColor, 
                                        new RectangleF(0, 0, image.Width, image.Height));
                    AddTemplateToGraphics(graph, graphMaskQRCode, version, sizeCanvas, 
                                          foregroundColor, backgroundColor, margin);
                    AddLevelDataToMask(graphMaskQRCode, sizeCanvas, margin);
                }
            }

            // Add sync lines to the image
            AddSyncToImage(image, imageMaskQRCode, sizeCanvas,
                           foregroundBrush, margin);

            // Add the version code to the image
            AddVersionDataToImage(image, imageMaskQRCode, version, sizeCanvas, 
                                  foregroundBrush.Color, margin);

            // Add the data to the image
            AddBinaryDataToImage(image, imageMaskQRCode, dataBinary, sizeCanvas, 
                                 foregroundBrush, numberMask, margin);

            // Add the code of the selected correction level and mask
            AddLevelAndMaskCodes(image, numberMask, levelCorrection, sizeCanvas, 
                                 foregroundBrush.Color, margin);
            return image;
        }

        private static void AddLevelAndMaskCodes(Bitmap image, int numberMask, QRCodeErrorCorrection levelCorrection, int sizeCanvas, Color foregroundColor, int margin)
        {
            int code = QRCodesData.CodeMaskAndLevels[8 * ((int)levelCorrection) + numberMask];
            for (int i = 0; i < 15; i++)
            {
                if (((code >> i) & 1) == 1)
                {
                    if (i < 6)
                    {
                        SetPixelToBitmap(image, 8, i, margin, foregroundColor);
                        SetPixelToBitmap(image, sizeCanvas - i - 1, 8, margin, foregroundColor);
                    }
                    else if (i < 8)
                    {
                        SetPixelToBitmap(image, 8, i + 1, margin, foregroundColor);
                        SetPixelToBitmap(image, sizeCanvas - i - 1, 8, margin, foregroundColor);
                    }
                    else if (i < 9)
                    {
                        SetPixelToBitmap(image, 8, sizeCanvas - 7, margin, foregroundColor);
                        SetPixelToBitmap(image, 7, 8, margin, foregroundColor);
                    }
                    else
                    {
                        SetPixelToBitmap(image, 5 - (i - 9), 8, margin, foregroundColor);
                        SetPixelToBitmap(image, 8, sizeCanvas - 7 + (i - 8), margin, foregroundColor);
                    }
                }
            }
            SetPixelToBitmap(image, 8, sizeCanvas - 8, margin, foregroundColor);
        }

        private static void AddLevelDataToMask(Graphics graphMaskSystemAreaQRCode, int sizeCanvas, int margin)
        {
            var coord8 = GetPosition(8, margin);
            var coord0 = GetPosition(0, margin);
            var coordCanvasMinus1 = GetPosition(sizeCanvas - 1, margin);
            var coordCanvasMinus8 = GetPosition(sizeCanvas - 8, margin);
            graphMaskSystemAreaQRCode.DrawLine(new Pen(Color.Black), 
                                               coord8, coord0, 
                                               coord8, coord8);
            graphMaskSystemAreaQRCode.DrawLine(new Pen(Color.Black), 
                                               coord0, coord8, 
                                               coord8, coord8);
            graphMaskSystemAreaQRCode.DrawLine(new Pen(Color.Black), 
                                               coordCanvasMinus8, coord8, 
                                               coordCanvasMinus1, coord8);
            graphMaskSystemAreaQRCode.DrawLine(new Pen(Color.Black), 
                                               coord8, coordCanvasMinus8, 
                                               coord8, coordCanvasMinus1);
        }

        private static void AddBinaryDataToImage(Bitmap image, Bitmap imageMask, byte[] dataBinary, int sizeCanvas, SolidBrush foregroundColor, int numberMask, int margin)
        {
            int indexByte = 0;
            int indexBit = 7;
            int stripX = sizeCanvas - 2;
            int stripY = sizeCanvas - 1;
            bool isUp = true;
            int offsetX = 1;
            Color color = foregroundColor.Color;
            bool hasPixel = false;

            while (indexByte < dataBinary.Length)
            {
                var posX = stripX + offsetX;
                var posY = stripY;
                var x = GetPosition(posX, margin);
                var y = GetPosition(posY, margin);

                // If not a system area pixel, then write a bit of data
                if (imageMask.GetPixel(x, y).A == 0)
                {
                    if (((dataBinary[indexByte] >> indexBit) & 1) == 1)
                        hasPixel = true;
                    else
                        hasPixel = false;

                    // Write the mask
                    if (GetMaskValueOfXY(numberMask, posX, posY) == 0)
                        hasPixel = !hasPixel;

                    if (hasPixel)
                        image.SetPixel(x, y, color);

                    indexBit--;
                    if (indexBit < 0)
                    {
                        indexBit = 7;
                        indexByte++;
                    }
                }
                CalcNextCurrentPosition(ref stripX, ref stripY, ref offsetX, ref isUp, 
                                        sizeCanvas);
            }
        }

        private static void CalcNextCurrentPosition(ref int stripX, ref int stripY, ref int offsetX, ref bool isUp, int sizeCanvas)
        {
            if (offsetX > 0)
                offsetX = 0;
            else
            {
                offsetX = 1;
                stripY += isUp ? -1 : 1;
                if (stripY < 0)
                {
 
                    // then move from top to bottom
                    isUp = !isUp;
                    stripY = 0;
                    stripX -= 2;
                    if (stripX == 5)
                        stripX -= 1;
                }
                else if (stripY >= sizeCanvas)
                {

                    // then move from bottom to top
                    isUp = !isUp;
                    stripY = sizeCanvas - 1;
                    stripX -= 2;
                    if (stripX == 5)
                        stripX -= 1;
                }
            }
        }

        private static int GetMaskValueOfXY(int numberMask, int x, int y)
        {

            // Mask to improve the QR code reading
            switch(numberMask)
            {
                case 0:
                    return (x + y) % 2;
                case 1:
                    return y % 2;
                case 2:
                    return x % 3;
                case 3:
                    return (x + y) % 3;
                case 4:
                    return (x / 3 + y / 2) % 2;
                case 5:
                    return (x * y) % 2 + (x * y) % 3;
                case 6:
                    return ((x * y) % 2 + (x * y) % 3) % 2;
                case 7:
                    return ((x * y) % 3 + (x + y) % 2) % 2;
            }
            throw new NotImplementedException();
        }

        private static void AddVersionDataToImage(Bitmap image, Bitmap imageMasque, int version, int sizeCanvas, Color foregroundColor, int margin)
        {
            if (version >= 7)
            {
                byte line1 = QRCodesData.VersionData[version - 7][0];
                byte line2 = QRCodesData.VersionData[version - 7][1];
                byte line3 = QRCodesData.VersionData[version - 7][2];

                for (int i = 0; i < 6; i++)
                {
                    if ((line1 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 11, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 11, margin, foregroundColor);
                    }
                    if ((line2 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 10, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 10, margin, foregroundColor);
                    }
                    if ((line3 & 1) == 1)
                    {
                        SetPixelToBitmap(image, sizeCanvas - 9, 5 - i, margin, foregroundColor);
                        SetPixelToBitmap(image, 5 - i, sizeCanvas - 9, margin, foregroundColor);
                    }
                    SetPixelToBitmap(imageMasque, sizeCanvas - 11, 5 - i, margin, Color.Black);
                    SetPixelToBitmap(imageMasque, 5 - i, sizeCanvas - 11, margin, Color.Black);
                    SetPixelToBitmap(imageMasque, sizeCanvas - 10, 5 - i, margin, Color.Black);
                    SetPixelToBitmap(imageMasque, 5 - i, sizeCanvas - 10, margin, Color.Black);
                    SetPixelToBitmap(imageMasque, sizeCanvas - 9, 5 - i, margin, Color.Black);
                    SetPixelToBitmap(imageMasque, 5 - i, sizeCanvas - 9, margin, Color.Black);
                    line1 >>= 1;
                    line2 >>= 1;
                    line3 >>= 1;
                }
            }
        }

        private static void AddSyncToImage(Bitmap image, Bitmap imageMasque, int sizeCanvas, SolidBrush foregroundColor, int margin)
        {
            for (int t = 7; t <= sizeCanvas - 7; t+=1)
            {
                if (t % 2 == 0)
                {
                    SetPixelToBitmap(image, t, 6, margin, foregroundColor.Color);
                    SetPixelToBitmap(image, 6, t, margin, foregroundColor.Color);                    
                }
                SetPixelToBitmap(imageMasque, t, 6, margin, foregroundColor.Color);
                SetPixelToBitmap(imageMasque, 6, t, margin, foregroundColor.Color);
            }
        }

        private static void AddTemplateToGraphics(Graphics graph, Graphics graphMaskQRCode, int version, int sizeCanvas, Brush foregroundColor, Brush backgroundColor, int margin)
        {
            AddFindingSquareToGraphics(graph, graphMaskQRCode, 
                                       new Point(GetPosition(3, margin), 
                                                 GetPosition(3, margin)), 
                                       foregroundColor, 
                                       backgroundColor);
            AddFindingSquareToGraphics(graph, graphMaskQRCode, 
                                       new Point(GetPosition(3, margin), 
                                                 GetPosition(sizeCanvas - 4, margin)), 
                                       foregroundColor, 
                                       backgroundColor);
            AddFindingSquareToGraphics(graph, graphMaskQRCode, 
                                       new Point(GetPosition(sizeCanvas - 4, margin), 
                                                 GetPosition(3, margin)), 
                                       foregroundColor, 
                                       backgroundColor);

            foreach(int x in QRCodesData.CoordinatesOfAlignSquares[version - 1])
                foreach(int y in QRCodesData.CoordinatesOfAlignSquares[version - 1])
                {
                    if (!(x < 8 && y < 8 || x < 8 && y > sizeCanvas - 8 ||
                        x > sizeCanvas - 8 && y < 8))
                    {
                        AddAlignSquareToGraphics(graph, graphMaskQRCode,
                                                 new Point(GetPosition(x, margin),
                                                           GetPosition(y, margin)),
                                                 foregroundColor,
                                                 backgroundColor);
                    }
                }
        }

        private static void AddFindingSquareToGraphics(System.Drawing.Graphics graph, System.Drawing.Graphics graphMask, Point position, Brush foregroundColor, Brush backgroundColor)
        {
            graph.FillRectangle(foregroundColor, 
                                new Rectangle(new Point(position.X - 3, position.Y - 3),
                                              new Size(7, 7)));
            graph.FillRectangle(backgroundColor, 
                                new Rectangle(new Point(position.X - 2, position.Y - 2),
                                              new Size(5, 5)));
            graph.FillRectangle(foregroundColor, 
                                new Rectangle(new Point(position.X - 1, position.Y - 1), 
                                              new Size(3, 3)));
            graphMask.FillRectangle(new SolidBrush(Color.Black), 
                                    new Rectangle(new Point(position.X - 4, position.Y - 4), 
                                                  new Size(9, 9)));
        }

        private static void AddAlignSquareToGraphics(System.Drawing.Graphics graph, System.Drawing.Graphics graphMask, Point position, Brush foregroundColor, Brush backgroundColor)
        {
            graph.FillRectangle(foregroundColor, 
                                new Rectangle(new Point(position.X - 2, position.Y - 2),
                                              new Size(5, 5)));
            graph.FillRectangle(backgroundColor, 
                                new Rectangle(new Point(position.X - 1, position.Y - 1),
                                              new Size(3, 3)));
            graph.FillRectangle(foregroundColor, 
                                new Rectangle(new Point(position.X, position.Y),
                                              new Size(1, 1)));
            graphMask.FillRectangle(new SolidBrush(Color.Black), 
                                    new Rectangle(new Point(position.X - 2, position.Y - 2), 
                                                  new Size(5, 5)));
        }

        /// <summary>
        /// Gets the absolute position from the QR code canvas.
        /// </summary>
        /// <param name="coord">The coordinate.</param>
        /// <param name="margin">The quiet zone.</param>
        /// <returns>int</returns>   
        private static int GetPosition(int coord, int margin) => margin + coord;

        /// <summary>
        /// Sets pixel to the QR code bitmap.
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="foregroundColor">The QR code color.</param>
        /// <param name="margin">The quiet zone.</param>
        /// <returns>int</returns>
        private static void SetPixelToBitmap(Bitmap image, int x, int y, int margin, Color foregroundColor)
        {
            image.SetPixel(GetPosition(x, margin),
                           GetPosition(y, margin),
                           foregroundColor);
        }
    }
}
