namespace PoC;

public class ByteArrayUtils
{
    
    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }
    
    
    public static byte[] XorIntAndByteArray(int intValue, byte[] byteArray)
    {
        byte[] intBytes = BitConverter.GetBytes(intValue);
        Array.Reverse(intBytes);
        byte[] resultBytes = new byte[byteArray.Length];

        for (int i = 0; i < byteArray.Length; i++)
        {
            resultBytes[i] = (byte)(byteArray[i] ^ intBytes[i % 4]);
        }

        return resultBytes;
    }
}