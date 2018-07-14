#ifndef N64YUVLIB
#define N64YUVLIB
#include <cstdlib>
#include <cmath>
#include <vector>

namespace YUV2RGB
{
	#define N64YUV_CLAMP(val) (val > 255 ? 255 : (val < 0 ? 0 : val))

	class N64YUV 
	{
	public:

		/** Decodes a YUV value into a RGB value */
		static inline void YUV2RGB(
			unsigned char Y, unsigned char U, unsigned char V,
			unsigned char* R, unsigned char* G, unsigned char* B) 
		{
			*R = N64YUV_CLAMP((int)roundf(1.164f * (Y - 16) + 1.596f * (V - 128)));
			*G = N64YUV_CLAMP((int)roundf(1.164f * (Y - 16) - 0.813f * (V - 128) - 0.391f * (U - 128)));
			*B = N64YUV_CLAMP((int)roundf(1.164f * (Y - 16) + 2.018f * (U - 128)));
		}

		/** Encodes 2 RGB pixels into 1 UYVY pair */
		static inline void RGB2YUV(
			unsigned char R1, unsigned char G1, unsigned char B1,
			unsigned char R2, unsigned char G2, unsigned char B2,
			unsigned char* U, unsigned char* Y1, unsigned char* V, unsigned char* Y2) 
		{
			*Y1 = N64YUV_CLAMP((int)roundf((0.257f * R1) + (0.504f * G1) + (0.098f * B1) + 16));
			*Y2 = N64YUV_CLAMP((int)roundf((0.257f * R2) + (0.504f * G2) + (0.098f * B2) + 16));

			float V1 = (0.439f * R1) - (0.368f * G1) - (0.071f * B1) + 128;
			float U1 = -(0.148f * R1) - (0.291f * G1) + (0.439f * B1) + 128;
			float V2 = (0.439f * R2) - (0.368f * G2) - (0.071f * B2) + 128;
			float U2 = -(0.148f * R2) - (0.291f * G2) + (0.439f * B2) + 128;

			*U = N64YUV_CLAMP((int)roundf((U1 + U2) / 2.0f));
			*V = N64YUV_CLAMP((int)roundf((V1 + V2) / 2.0f));
		}

		static inline std::vector<unsigned char> decodeArray_YUVtoRGB(std::vector<unsigned char> inputYUV) 
		{
			int inputLength = inputYUV.size();
			std::vector<unsigned char> outputRGB = std::vector<unsigned char>((inputLength * 3) / 2);

			for (int i = 0; i < inputLength / 4; i++) {
				int U  = inputYUV[i * 4 + 0];
				int Y1 = inputYUV[i * 4 + 1];
				int V  = inputYUV[i * 4 + 2];
				int Y2 = inputYUV[i * 4 + 3];

				unsigned char R1, G1, B1, R2, G2, B2;

				YUV2RGB(Y1, U, V, &R1, &G1, &B1);
				YUV2RGB(Y2, U, V, &R2, &G2, &B2);

				outputRGB[i * 6 + 0] = R1;
				outputRGB[i * 6 + 1] = G1;
				outputRGB[i * 6 + 2] = B1;
				outputRGB[i * 6 + 3] = R2;
				outputRGB[i * 6 + 4] = G2;
				outputRGB[i * 6 + 5] = B2;
			}

			return outputRGB;
		}

		static inline std::vector<unsigned char> decodeArray_YUVtoRGBA(std::vector<unsigned char> inputYUV)
		{
			int inputLength = inputYUV.size();
			std::vector<unsigned char> outputRGBA = std::vector<unsigned char>(inputLength * 2);

			for (int i = 0; i < inputLength / 4; i++) {
				unsigned char U  = inputYUV[i * 4 + 0];
				unsigned char Y1 = inputYUV[i * 4 + 1];
				unsigned char V  = inputYUV[i * 4 + 2];
				unsigned char Y2 = inputYUV[i * 4 + 3];
				unsigned char R1, G1, B1, R2, G2, B2;

				YUV2RGB(Y1, U, V, &R1, &G1, &B1);
				YUV2RGB(Y2, U, V, &R2, &G2, &B2);

				outputRGBA[i * 8 + 0] = R1;
				outputRGBA[i * 8 + 1] = G1;
				outputRGBA[i * 8 + 2] = B1;
				outputRGBA[i * 8 + 3] = 0xFF;
				outputRGBA[i * 8 + 4] = R2;
				outputRGBA[i * 8 + 5] = G2;
				outputRGBA[i * 8 + 6] = B2;
				outputRGBA[i * 8 + 7] = 0xFF;
			}

			return outputRGBA;
		}

		static inline std::vector<unsigned char> encodeArray_YUVfromRGB(std::vector<unsigned char> inputRGB)
		{
			int inputLength = inputRGB.size();
			std::vector<unsigned char> outputYUV = std::vector<unsigned char>((inputLength * 2) / 3);

			for (int i = 0; i < inputLength / 6; i++) {

				unsigned char R1 = inputRGB[i * 6 + 0];
				unsigned char G1 = inputRGB[i * 6 + 1];
				unsigned char B1 = inputRGB[i * 6 + 2];
				unsigned char R2 = inputRGB[i * 6 + 3];
				unsigned char G2 = inputRGB[i * 6 + 4];
				unsigned char B2 = inputRGB[i * 6 + 5];

				unsigned char U, Y1, V, Y2;

				RGB2YUV(R1, G1, B1, R2, G2, B2, &U, &Y1, &V, &Y2);

				outputYUV[i * 4 + 0] = U;
				outputYUV[i * 4 + 1] = Y1;
				outputYUV[i * 4 + 2] = V;
				outputYUV[i * 4 + 3] = Y2;
			}

			return outputYUV;
		}

		static inline std::vector<unsigned char> encodeArray_YUVfromRGBA(std::vector<unsigned char> inputRGBA)
		{
			int inputLength = inputRGBA.size();
			std::vector<unsigned char> outputYUV = std::vector<unsigned char>(inputLength / 2);

			for (int i = 0; i < inputLength / 8; i++) {

				// Skip alpha bytes, since it's not needed.
				unsigned char R1 = inputRGBA[i * 8 + 0];
				unsigned char G1 = inputRGBA[i * 8 + 1];
				unsigned char B1 = inputRGBA[i * 8 + 2];
				unsigned char R2 = inputRGBA[i * 8 + 4];
				unsigned char G2 = inputRGBA[i * 8 + 5];
				unsigned char B2 = inputRGBA[i * 8 + 6];

				unsigned char U, Y1, V, Y2;

				RGB2YUV(R1, G1, B1, R2, G2, B2, &U, &Y1, &V, &Y2);

				outputYUV[i * 4 + 0] = U;
				outputYUV[i * 4 + 1] = Y1;
				outputYUV[i * 4 + 2] = V;
				outputYUV[i * 4 + 3] = Y2;
			}

			return outputYUV;
		}
	};
}

#endif