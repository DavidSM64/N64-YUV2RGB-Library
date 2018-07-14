using System;

namespace YUV2RGB
{
    class N64YUV
    {
        private static byte N64YUV_CLAMP(int val) {
            return (byte)(val > 255 ? 255 : (val < 0 ? 0 : val));
        }

        /** Decodes a YUV value into a RGB value */
        public static void YUV2RGB(
            byte Y, byte U, byte V,
            out byte R, out byte G, out byte B)
        {
            R = N64YUV_CLAMP((int)Math.Round(1.164f * (Y - 16) + 1.596f * (V - 128)));
            G = N64YUV_CLAMP((int)Math.Round(1.164f * (Y - 16) - 0.813f * (V - 128) - 0.391f * (U - 128)));
            B = N64YUV_CLAMP((int)Math.Round(1.164f * (Y - 16) + 2.018f * (U - 128)));
        }

        /** Encodes 2 RGB pixels into 1 UYVY pair */
        public static void RGB2YUV(
            byte R1, byte G1, byte B1,
            byte R2, byte G2, byte B2,
            out byte U, out byte Y1, out byte V, out byte Y2)
        {
            Y1 = N64YUV_CLAMP((int)Math.Round((0.257f * R1) + (0.504f * G1) + (0.098f * B1) + 16));
            Y2 = N64YUV_CLAMP((int)Math.Round((0.257f * R2) + (0.504f * G2) + (0.098f * B2) + 16));

            float V1 = (0.439f * R1) - (0.368f * G1) - (0.071f * B1) + 128;
            float U1 = -(0.148f * R1) - (0.291f * G1) + (0.439f * B1) + 128;
            float V2 = (0.439f * R2) - (0.368f * G2) - (0.071f * B2) + 128;
            float U2 = -(0.148f * R2) - (0.291f * G2) + (0.439f * B2) + 128;

            U = N64YUV_CLAMP((int)Math.Round((U1 + U2) / 2.0f));
            V = N64YUV_CLAMP((int)Math.Round((V1 + V2) / 2.0f));
        }

        public static byte[] decodeArray_YUVtoRGB(byte[] inputYUV)
        {
            int inputLength = inputYUV.Length;
            byte[] outputRGB = new byte[(inputLength * 3) / 2];

            for (int i = 0; i < inputLength / 4; i++)
            {
                byte U = inputYUV[i * 4 + 0];
                byte Y1 = inputYUV[i * 4 + 1];
                byte V = inputYUV[i * 4 + 2];
                byte Y2 = inputYUV[i * 4 + 3];

                byte R1, G1, B1, R2, G2, B2;

                YUV2RGB(Y1, U, V, out R1, out G1, out B1);
                YUV2RGB(Y2, U, V, out R2, out G2, out B2);

                outputRGB[i * 6 + 0] = R1;
                outputRGB[i * 6 + 1] = G1;
                outputRGB[i * 6 + 2] = B1;
                outputRGB[i * 6 + 3] = R2;
                outputRGB[i * 6 + 4] = G2;
                outputRGB[i * 6 + 5] = B2;
            }

            return outputRGB;
        }

        public static byte[] decodeArray_YUVtoRGBA(byte[] inputYUV)
        {
            int inputLength = inputYUV.Length;
            byte[] outputRGBA = new byte[inputLength * 2];

            for (int i = 0; i < inputLength / 4; i++)
            {
                byte U = inputYUV[i * 4 + 0];
                byte Y1 = inputYUV[i * 4 + 1];
                byte V = inputYUV[i * 4 + 2];
                byte Y2 = inputYUV[i * 4 + 3];
                byte R1, G1, B1, R2, G2, B2;

                YUV2RGB(Y1, U, V, out R1, out G1, out B1);
                YUV2RGB(Y2, U, V, out R2, out G2, out B2);

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

        public static byte[] encodeArray_YUVfromRGB(byte[] inputRGB)
        {
            int inputLength = inputRGB.Length;
            byte[] outputYUV = new byte[(inputLength * 2) / 3];

            for (int i = 0; i < inputLength / 6; i++)
            {

                byte R1 = inputRGB[i * 6 + 0];
                byte G1 = inputRGB[i * 6 + 1];
                byte B1 = inputRGB[i * 6 + 2];
                byte R2 = inputRGB[i * 6 + 3];
                byte G2 = inputRGB[i * 6 + 4];
                byte B2 = inputRGB[i * 6 + 5];

                byte U, Y1, V, Y2;

                RGB2YUV(R1, G1, B1, R2, G2, B2, out U, out Y1, out V, out Y2);

                outputYUV[i * 4 + 0] = U;
                outputYUV[i * 4 + 1] = Y1;
                outputYUV[i * 4 + 2] = V;
                outputYUV[i * 4 + 3] = Y2;
            }

            return outputYUV;
        }

        public static byte[] encodeArray_YUVfromRGBA(byte[] inputRGBA)
        {
            int inputLength = inputRGBA.Length;
            byte[] outputYUV = new byte[inputLength / 2];

            for (int i = 0; i < inputLength / 8; i++)
            {

                // Skip alpha bytes, since it's not needed.
                byte R1 = inputRGBA[i * 8 + 0];
                byte G1 = inputRGBA[i * 8 + 1];
                byte B1 = inputRGBA[i * 8 + 2];
                byte R2 = inputRGBA[i * 8 + 4];
                byte G2 = inputRGBA[i * 8 + 5];
                byte B2 = inputRGBA[i * 8 + 6];

                byte U, Y1, V, Y2;

                RGB2YUV(R1, G1, B1, R2, G2, B2, out U, out Y1, out V, out Y2);

                outputYUV[i * 4 + 0] = U;
                outputYUV[i * 4 + 1] = Y1;
                outputYUV[i * 4 + 2] = V;
                outputYUV[i * 4 + 3] = Y2;
            }

            return outputYUV;
        }
    };
}
