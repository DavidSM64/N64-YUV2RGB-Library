# N64-YUV2RGB-Library
A multi-language library for converting between YUV and RGB pixels. For use with N64 ROM hacking and homebrew.

This library is licensed under MIT, so feel free to include the source code into your project(s).

## Languages

* C
* C++
* C#
* VB.NET

## Functions

Name | Description
------------ | -------------
YUV2RGB | Decodes a YUV value into a RGB value
RGB2YUV | Encodes 2 RGB pixels into 1 UYVY pair
decodeArray_YUVtoRGB | Decodes an array of YUV data (UYVY pairs) into an RGB data array.
decodeArray_YUVtoRGBA | Decodes an array of YUV data (UYVY pairs) into an RGBA data array.
encodeArray_YUVfromRGB | Encodes an array of RGB data into an YUV (UYVY pairs) data array.
encodeArray_YUVfromRGBA | Encodes an array of RGBA data into an YUV (UYVY pairs) data array.

#### Why have functions for both RGB and RGBA data, when YUV doesn't have an alpha channel?

It's mostly for convience. A program might not deal with RGBA32 at all, or it might use RGBA32 as the standard pixel format. 
You can choose whichever you think is more relevant to your program. 

Note: if you encode data to YUV from RGBA32 the alpha channel will be 
ignored, and if you're decoding YUV into RGBA32 the alpha channel will always be set to 255.

## Syntax

### C

Each function has the prefix `N64YUV_`. Data values are assumed to be `unsigned char`. Every function has a `void` 
return type, so instead you will pass the pointers of the outputs in the parameters. The array functions will automatically
call `malloc()` with the correct output size if you pass in a null pointer for the output array.

For the array functions, you should call them like so:
```C
// Set outputData as a null pointer at first, N64YUV_decodeArray_YUVtoRGB will call malloc() automatically.
unsigned char* outputData = 0;

// length of the output data, once decoded.
int outputLength = 0;

/* 
Decode a YUV array into an RGB array, 
inputYUV is some unsigned char array that contains UYVY pairs.
inputLength is the length of inputYUV in bytes.
*/
N64YUV_decodeArray_YUVtoRGB(inputYUV, inputLength, &outputData, &outputLength);

// Later in the code...
if(outputData != 0)
  free(outputData); // You must free the output data yourself
```

#### Function types
```C
void N64YUV_YUV2RGB(
	unsigned char Y, unsigned char U, unsigned char V,
	unsigned char* R, unsigned char* G, unsigned char* B);
  
void N64YUV_RGB2YUV(
	unsigned char R1, unsigned char G1, unsigned char B1,
	unsigned char R2, unsigned char G2, unsigned char B2,
	unsigned char* U, unsigned char* Y1, unsigned char* V, unsigned char* Y2);
  
void N64YUV_decodeArray_YUVtoRGB(unsigned char* inputYUV, int inputLength, unsigned char** outputRGB, int* outputLength);
void N64YUV_decodeArray_YUVtoRGBA(unsigned char* inputYUV, int inputLength, unsigned char** outputRGBA, int* outputLength);
void N64YUV_encodeArray_YUVfromRGB(unsigned char* inputRGB, int inputLength, unsigned char** outputYUV, int* outputLength);
void N64YUV_encodeArray_YUVfromRGBA(unsigned char* inputRGBA, int inputLength, unsigned char** outputYUV, int* outputLength);
```
---------------------------------------------
### C++

Each function is contained within the `N64YUV` class under the `YUV2RGB` namespace. 
Data values are assumed to be `unsigned char`. 
The pixel conversion functions have a `void` return type, so instead you will pass the pointers of the outputs in the parameters. 
The array functions use `std::vector` for the input array and output return value.
They are called like so:
```C++
// inputYUV is some std::vector<unsigned char> containing UVYV pairs
std::vector<unsigned char> output = YUV2RGB::N64YUV::decodeArray_YUVtoRGB(inputYUV);
```

#### Function types
```C++
namespace YUV2RGB
{
  class N64YUV
  {
    void YUV2RGB(
      unsigned char Y, unsigned char U, unsigned char V,
      unsigned char* R, unsigned char* G, unsigned char* B);

    void RGB2YUV(
      unsigned char R1, unsigned char G1, unsigned char B1,
      unsigned char R2, unsigned char G2, unsigned char B2,
      unsigned char* U, unsigned char* Y1, unsigned char* V, unsigned char* Y2);

    std::vector<unsigned char> decodeArray_YUVtoRGB(std::vector<unsigned char> inputYUV);
    std::vector<unsigned char> decodeArray_YUVtoRGBA(std::vector<unsigned char> inputYUV);
    std::vector<unsigned char> encodeArray_YUVfromRGB(std::vector<unsigned char> inputRGB);
    std::vector<unsigned char> encodeArray_YUVfromRGBA(std::vector<unsigned char> inputRGBA);
  }
}
```
---------------------------------------------
### C#

Each function is contained within the `N64YUV` class under the `YUV2RGB` namespace. 
Data values are assumed to be `byte`. 
The pixel conversion functions have a `void` return type, so instead you will pass the outputs using `out`. 
The array functions use `byte[]` for the input array and output return value.
They are called like so:
```C#
// inputYUV is some byte[] containing UVYV pairs
byte[] output = N64YUV.decodeArray_YUVtoRGBA(inputYUV);
```

#### Function types
```C#
namespace YUV2RGB
{
  class N64YUV
  {
    void RGB2YUV(
      byte R1, byte G1, byte B1,
      byte R2, byte G2, byte B2,
      out byte U, out byte Y1, out byte V, out byte Y2);

    void RGB2YUV(
      byte R1, byte G1, byte B1,
      byte R2, byte G2, byte B2,
      out byte U, out byte Y1, out byte V, out byte Y2);

    byte[] decodeArray_YUVtoRGB(byte[] inputYUV);
    byte[] decodeArray_YUVtoRGBA(byte[] inputYUV);
    byte[] encodeArray_YUVfromRGB(byte[] inputRGB);
    byte[] encodeArray_YUVfromRGBA(byte[] inputRGBA);
  }
}
```
---------------------------------------------
### VB.NET

Each function is contained within the `N64YUV` class. 
Data values are assumed to be `Byte`. 
The pixel conversion functions don't have a return value, 
instead the outputs are passed in the parameters as references. 
The array functions use `Byte()` for the input array and output return value.
They are called like so:
```vb
' inputYUV is some Byte() containing UVYV pairs
Dim output As Byte() = N64YUV.decodeArray_YUVtoRGBA(inputYUV)
```

#### Function types
```vb
class N64YUV
{
  Sub YUV2RGB(
    ByVal Y As Byte, ByVal U As Byte, ByVal V As Byte, 
    ByRef R As Byte, ByRef G As Byte, ByRef B As Byte)

  Sub RGB2YUV(
    ByVal R1 As Byte, ByVal G1 As Byte, ByVal B1 As Byte, 
    ByVal R2 As Byte, ByVal G2 As Byte, ByVal B2 As Byte, 
    ByRef U As Byte, ByRef Y1 As Byte, ByRef V As Byte, ByRef Y2 As Byte)

  Function decodeArray_YUVtoRGB(ByVal inputYUV As Byte()) As Byte()
  Function decodeArray_YUVtoRGBA(ByVal inputYUV As Byte()) As Byte()
  Function encodeArray_YUVfromRGB(ByVal inputRGB As Byte()) As Byte()
  Function encodeArray_YUVfromRGBA(ByVal inputRGBA As Byte()) As Byte()
}

```
