#ifndef N64YUVLIB
#define N64YUVLIB
#include <stdlib.h>
#include <math.h>

#define N64YUV_CLAMP(val) (val > 255 ? 255 : (val < 0 ? 0 : val))

/** Decodes a YUV value into a RGB value */
void N64YUV_YUV2RGB(
	unsigned char Y, unsigned char U, unsigned char V,
	unsigned char* R, unsigned char* G, unsigned char* B);

/** Encodes 2 RGB pixels into 1 UYVY pair */
void N64YUV_RGB2YUV(
	unsigned char R1, unsigned char G1, unsigned char B1,
	unsigned char R2, unsigned char G2, unsigned char B2,
	unsigned char* U, unsigned char* Y1, unsigned char* V, unsigned char* Y2);

void N64YUV_decodeArray_YUVtoRGB(unsigned char* inputYUV, int inputLength, 
	unsigned char** outputRGB, int* outputLength);
void N64YUV_decodeArray_YUVtoRGBA(unsigned char* inputYUV, int inputLength, 
	unsigned char** outputRGBA, int* outputLength);
void N64YUV_encodeArray_YUVfromRGB(unsigned char* inputRGB, int inputLength, 
	unsigned char** outputYUV, int* outputLength);
void N64YUV_encodeArray_YUVfromRGBA(unsigned char* inputRGBA, int inputLength, 
	unsigned char** outputYUV, int* outputLength);

#endif