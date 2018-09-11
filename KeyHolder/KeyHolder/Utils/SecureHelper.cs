using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyHolder.Utils
{
    public static class SecureHelper
    {
        private static readonly string PasswordHash = "";
        private static readonly string SaltKey = "";
        private static readonly string ViKey = "";

        // todo: try with bytes
        public static string EncryptString(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(ViKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string DecryptString(string encryptedString)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedString);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(ViKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        //private const int saltSize = 8;

        public static void CreateFileDb(FileInfo targetFile)
        {
            if (!targetFile.Exists)
            {
                var fileStream = targetFile.Create();
                fileStream.Dispose();
                fileStream.Close();
            }
        }

        //public static void WriteEncryptedContent(FileStream fileStream, string password)
        //{
        //    var keyGenerator = new Rfc2898DeriveBytes(password, saltSize);
        //    var rijndael = Rijndael.Create();

        //    // BlockSize, KeySize in bit --> divide by 8
        //    rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
        //    rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

        //    using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
        //    {
        //        // todo: add the json handling here!!!
        //        //using (var writer = new StreamWriter(cryptoStream))
        //        //{
        //        //    writer.WriteLine("DB file for passwords.");
        //        //}
        //    }
        //}

        //public static string Decrypt(FileInfo sourceFile, string password)
        //{
        //    var text = string.Empty;

        //    // read salt
        //    var fileStream = sourceFile.OpenRead();
        //    var salt = new byte[saltSize];
        //    fileStream.Read(salt, 0, saltSize);

        //    // initialize algorithm with salt
        //    var keyGenerator = new Rfc2898DeriveBytes(password, salt);
        //    var rijndael = Rijndael.Create();
        //    rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
        //    rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

        //    // decrypt
        //    using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
        //    {
        //        using (var reader = new StreamReader(cryptoStream))
        //        {
        //            text = reader.ReadToEnd();
        //        }
        //    }

        //    return text;
        //}
    }
}
