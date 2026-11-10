using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Application.Common.Utility
{
    public class Encryption
    {
        public string Encrypt_final(string text)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(text);
            byte[] passwordBytes = Encoding.UTF8.GetBytes("Password");

            // Hash the password with SHA256
            passwordBytes = SHA256.HashData(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string encryptedResult = Convert.ToBase64String(bytesEncrypted);
            return encryptedResult;
        }

        public string Decrypt_final(string text)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(text);
            byte[] passwordBytesdecrypt = Encoding.UTF8.GetBytes("Password");
            passwordBytesdecrypt = SHA256.HashData(passwordBytesdecrypt);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytesdecrypt);

            string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);
            return decryptedResult;
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[]? encryptedBytes = null;
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    byte[] key = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256, 32); // 256 bits = 32 bytes
                    byte[] iv = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256, 16); // 128 bits = 16 bytes

                    AES.Key = key;
                    AES.IV = iv;
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[]? decryptedBytes = null;
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    byte[] key = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256, 32); // 256 bits = 32 bytes
                    byte[] iv = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256, 16); // 128 bits = 16 bytes

                    AES.Key = key;
                    AES.IV = iv;
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
