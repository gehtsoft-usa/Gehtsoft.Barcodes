# Gehtsoft.Barcodes

[![NuGet](https://img.shields.io/nuget/v/Gehtsoft.PDFFlowLib.svg)](https://www.nuget.org/packages/Gehtsoft.PDFFlowLib/)

**Gehtsoft.Barcodes** is a cross-platform C# library for generation of different types of barcodes and QR codes. The goal of the **Gehtsoft.Barcodes** library is to provide .NET application developers with a solution that allows them to create barcodes (including QR codes) quickly.

You can easily add barcodes to your application using the convenient API methods.

The following platforms are supported:

* .NET Framework 4.5.1 and later
* .NET Standard 2.0

Main features
=============

Currently the library supports generation of the following types of barcodes and QR codes: 
- EAN8
- EAN13
- GS1-128A
- GS1-128B
- GS1-128C
- UPC_A
- QR codes

Additionally, using the link in the [More Information](More-information) section, you can get our other proprietary library for generating PDF documents with convenient integration. Using the **GS PDFFlow** library, you can easily add barcodes to simple and complex documents.

Getting started
===============

The easiest way to get started is to install [the available NuGet package](https://www.nuget.org/packages/Gehtsoft.PDFFlowLib/).

Quick start
-----------

You need to connect links to the project:
``` c#
using Gehtsoft.Barcodes.UserAPI;
using Gehtsoft.Barcodes.Enums;
```
In these examples, we use standard modules for saving images.
``` c#
using System.IO;
using System.Drawing;
```
You can use any graphics library that can accept PNG data.
``` c#
public class QuickStart
{
    private static void Main(string[] args)
    {
        byte[] dataBarcode8 = BarcodesMaker.GetBarcode("01234567",
                                                       BarcodeType.EAN_8,
                                                       Color.Black,
                                                       Color.White,
                                                       true,
                                                       MeasureBarcodeUnit.FromPixel(0));
        using (Image image = Image.FromStream(new MemoryStream(dataBarcode8)))
        {
            image.Save("barcodeEAN8.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        byte[] dataBarcode13 = BarcodesMaker.GetBarcode("0123456789123",
                                                       BarcodeType.EAN_13,
                                                       Color.Black,
                                                       Color.White,
                                                       true,
                                                       MeasureBarcodeUnit.FromPixel(0));
        using (Image image = Image.FromStream(new MemoryStream(dataBarcode13)))
        {
            image.Save("barcodeEAN13.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        byte[] dataBarcodeGS128 = BarcodesMaker.GetBarcode("012345678912ABCD",
                                                                  BarcodeType.GS1_128A,
                                                                  Color.Black,
                                                                  Color.White,
                                                                  true,
                                                                  MeasureBarcodeUnit.FromPixel(0));
        using (Image image = Image.FromStream(new MemoryStream(dataBarcodeGS128)))
        {
            image.Save("barcodeGS128.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        byte[] dataQR = BarcodesMaker.GetQRCode("QRCode example", 
                                                QRCodeEncodingMethod.Binary, 
                                                QRCodeErrorCorrection.M,
                                                8);
        using (Image image = Image.FromStream(new MemoryStream(dataQR)))
        {
            image.Save("QRCode.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
```
We have successfully saved the barcode images: EAN8, EAN13, GS1-128, QR Code. 

Parameters
----------

List of parameters of user API methods:  
- data - the input data for the barcode. 
- barcodeType - the barcode type.  
- lineColor - the color of barcode lines.  
- backColor - the background color.  
- showDataLabel - defines whether the input data is printed under the barcode lines.  
- heightToCut - the height in pixels or in percent to be cut from the top of the barcode lines to reduce the standard height.  
- barcodeWidthMultiplier - the pixel scaling of the resulting barcode image.  
- encoding - the QR code encoding.  
- levelCorrection - the level of error correction.  
- scaleMultiplier - the pixel scaling of the resulting QR code image.  
- foregroundColor - the QR code color.  
- backgroundColor - the background color.  
- hasQuietZone - defines whether the QR code has a quiet zone.

More information
================

For more information about **Gehtsoft.Barcodes**, follow these links: 

* If you need to use barcodes or QR codes in PDF documents, we offer our own [Gehtsoft PDFFlow library](https://www.nuget.org/packages/Gehtsoft.PDFFlowlib) for efficient, quick and easy creation of PDF documents of any complexity. [Gehtsoft.PDFFlowLib](https://www.nuget.org/packages/Gehtsoft.PDFFlowLib/) is an excellent C# library that allows developers to easily generate complex documents for real business applications. [PDFFlow](https://www.nuget.org/packages/Gehtsoft.PDFFlowLib/) utilizes the **Gehtsoft.Barcodes** library to generate barcodes and QR codes and offers many additional formatting and layouting options. 

Contacts
========

To report issues with the library or request changes or features, please open an issue at <https://github.com/gehtsoft-usa/Gehtsoft.Barcodes/issues>.

We welcome contributions to the project. Please contact us at <contact@gehtsoftusa.com> if you would like to include your work in the library.

License
=======

The library is shared under GNU LGPL license and is free to use. Please consult the GNU LGPL license for details on proper usage of the library.

THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE MATERIALS OR THE USE OR OTHER DEALINGS IN THE MATERIALS.

