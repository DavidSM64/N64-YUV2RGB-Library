Public Class N64YUV
    Private Shared Function N64YUV_CLAMP(ByVal val As Integer) As Byte
        Return If(val > 255, 255, (If(val < 0, 0, val)))
    End Function

    Public Shared Sub YUV2RGB(ByVal Y As Byte, ByVal U As Byte, ByVal V As Byte, ByRef R As Byte, ByRef G As Byte, ByRef B As Byte)
        R = N64YUV_CLAMP(Math.Round(1.164F * (Y - 16) + 1.596F * (V - 128)))
        G = N64YUV_CLAMP(Math.Round(1.164F * (Y - 16) - 0.813F * (V - 128) - 0.391F * (U - 128)))
        B = N64YUV_CLAMP(Math.Round(1.164F * (Y - 16) + 2.018F * (U - 128)))
    End Sub

    Public Shared Sub RGB2YUV(ByVal R1 As Byte, ByVal G1 As Byte, ByVal B1 As Byte, ByVal R2 As Byte, ByVal G2 As Byte, ByVal B2 As Byte, ByRef U As Byte, ByRef Y1 As Byte, ByRef V As Byte, ByRef Y2 As Byte)
        Y1 = N64YUV_CLAMP(CInt(Math.Round((0.257F * R1) + (0.504F * G1) + (0.098F * B1) + 16)))
        Y2 = N64YUV_CLAMP(CInt(Math.Round((0.257F * R2) + (0.504F * G2) + (0.098F * B2) + 16)))
        Dim V1 As Single = (0.439F * R1) - (0.368F * G1) - (0.071F * B1) + 128
        Dim U1 As Single = -(0.148F * R1) - (0.291F * G1) + (0.439F * B1) + 128
        Dim V2 As Single = (0.439F * R2) - (0.368F * G2) - (0.071F * B2) + 128
        Dim U2 As Single = -(0.148F * R2) - (0.291F * G2) + (0.439F * B2) + 128
        U = N64YUV_CLAMP(Math.Round((U1 + U2) / 2.0F))
        V = N64YUV_CLAMP(Math.Round((V1 + V2) / 2.0F))
    End Sub

    Public Shared Function decodeArray_YUVtoRGB(ByVal inputYUV As Byte()) As Byte()
        Dim inputLength As Integer = inputYUV.Length
        Dim outputRGB As Byte() = New Byte((inputLength * 3) / 2 - 1) {}

        For i As Integer = 0 To inputLength / 4 - 1
            Dim U As Byte = inputYUV(i * 4 + 0)
            Dim Y1 As Byte = inputYUV(i * 4 + 1)
            Dim V As Byte = inputYUV(i * 4 + 2)
            Dim Y2 As Byte = inputYUV(i * 4 + 3)
            Dim R1, G1, B1, R2, G2, B2 As Byte
            YUV2RGB(Y1, U, V, R1, G1, B1)
            YUV2RGB(Y2, U, V, R2, G2, B2)
            outputRGB(i * 6 + 0) = R1
            outputRGB(i * 6 + 1) = G1
            outputRGB(i * 6 + 2) = B1
            outputRGB(i * 6 + 3) = R2
            outputRGB(i * 6 + 4) = G2
            outputRGB(i * 6 + 5) = B2
        Next

        Return outputRGB
    End Function

    Public Shared Function decodeArray_YUVtoRGBA(ByVal inputYUV As Byte()) As Byte()
        Dim inputLength As Integer = inputYUV.Length
        Dim outputRGBA As Byte() = New Byte(inputLength * 2 - 1) {}

        For i As Integer = 0 To inputLength / 4 - 1
            Dim U As Byte = inputYUV(i * 4 + 0)
            Dim Y1 As Byte = inputYUV(i * 4 + 1)
            Dim V As Byte = inputYUV(i * 4 + 2)
            Dim Y2 As Byte = inputYUV(i * 4 + 3)
            Dim R1, G1, B1, R2, G2, B2 As Byte
            YUV2RGB(Y1, U, V, R1, G1, B1)
            YUV2RGB(Y2, U, V, R2, G2, B2)
            outputRGBA(i * 8 + 0) = R1
            outputRGBA(i * 8 + 1) = G1
            outputRGBA(i * 8 + 2) = B1
            outputRGBA(i * 8 + 3) = &HFF
            outputRGBA(i * 8 + 4) = R2
            outputRGBA(i * 8 + 5) = G2
            outputRGBA(i * 8 + 6) = B2
            outputRGBA(i * 8 + 7) = &HFF
        Next

        Return outputRGBA
    End Function

    Public Shared Function encodeArray_YUVfromRGB(ByVal inputRGB As Byte()) As Byte()
        Dim inputLength As Integer = inputRGB.Length
        Dim outputYUV As Byte() = New Byte((inputLength * 2) / 3 - 1) {}

        For i As Integer = 0 To inputLength / 6 - 1
            Dim R1 As Byte = inputRGB(i * 6 + 0)
            Dim G1 As Byte = inputRGB(i * 6 + 1)
            Dim B1 As Byte = inputRGB(i * 6 + 2)
            Dim R2 As Byte = inputRGB(i * 6 + 3)
            Dim G2 As Byte = inputRGB(i * 6 + 4)
            Dim B2 As Byte = inputRGB(i * 6 + 5)
            Dim U, Y1, V, Y2 As Byte
            RGB2YUV(R1, G1, B1, R2, G2, B2, U, Y1, V, Y2)
            outputYUV(i * 4 + 0) = U
            outputYUV(i * 4 + 1) = Y1
            outputYUV(i * 4 + 2) = V
            outputYUV(i * 4 + 3) = Y2
        Next

        Return outputYUV
    End Function

    Public Shared Function encodeArray_YUVfromRGBA(ByVal inputRGBA As Byte()) As Byte()
        Dim inputLength As Integer = inputRGBA.Length
        Dim outputYUV As Byte() = New Byte(inputLength / 2 - 1) {}

        For i As Integer = 0 To inputLength / 8 - 1
            Dim R1 As Byte = inputRGBA(i * 8 + 0)
            Dim G1 As Byte = inputRGBA(i * 8 + 1)
            Dim B1 As Byte = inputRGBA(i * 8 + 2)
            Dim R2 As Byte = inputRGBA(i * 8 + 4)
            Dim G2 As Byte = inputRGBA(i * 8 + 5)
            Dim B2 As Byte = inputRGBA(i * 8 + 6)
            Dim U, Y1, V, Y2 As Byte
            RGB2YUV(R1, G1, B1, R2, G2, B2, U, Y1, V, Y2)
            outputYUV(i * 4 + 0) = U
            outputYUV(i * 4 + 1) = Y1
            outputYUV(i * 4 + 2) = V
            outputYUV(i * 4 + 3) = Y2
        Next

        Return outputYUV
    End Function

End Class
