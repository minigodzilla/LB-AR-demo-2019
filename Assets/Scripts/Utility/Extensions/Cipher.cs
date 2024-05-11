using System.IO;
using System;
using System.Security.Cryptography;

public static class Cipher
{
    // Key and IV generated using Aes.Create()
    private static readonly byte[] key = new byte[] { 218, 175, 152, 138, 241, 133, 182, 159, 73, 94, 38, 46, 178, 120, 186, 207, 130, 92, 224, 0, 205, 5, 151, 180, 114, 25, 76, 63, 15, 142, 112, 80 };
    private static readonly byte[] iv = new byte[] { 4, 41, 111, 147, 85, 43, 199, 127, 69, 64, 187, 65, 153, 82, 101, 198 };

    public static byte[] Encrypt(string plainText) => EncryptStringToBytes(plainText, key, iv);
    public static string Decrypt(byte[] cipherText) => DecryptStringFromBytes(cipherText, key, iv);

    // Copied from: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netframework-4.8
    private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException(nameof(plainText));
        if (Key == null || Key.Length <= 0) throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));
        byte[] encrypted;

        // Create an Aes object with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    // Copied from: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netframework-4.8
    private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException(nameof(cipherText));
        if (Key == null || Key.Length <= 0) throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));

        // Declare the string used to hold the decrypted text.
        string plaintext = null;

        // Create an Aes object with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }

        return plaintext;
    }
}