using System.Security.Cryptography;
using System.Text;

namespace analysistools.api.Services
{
    /// <summary>
    /// Encryption services encrypt and decrypt data, using a secret key, it is a good practice to store it outside the project.
    /// </summary>
    public static class EncryptionService
    {
        //TODO: Save the secret on environment variable or use external vault library
        private static string secretKey = "Continental1";

        //While an app specific salt is not the best practice for
        //password based encryption, it's probably safe enough as long as
        //it is truly uncommon.
        private static byte[] _salt = new byte[] { 1, 0, 1, 1, 0, 1, 0, 0 };

        public static string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder stringBuilder = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) stringBuilder.AppendFormat("{0:x2}", stream[i]);
            return stringBuilder.ToString();
        }

        public static string EncryptString(string PlainText)
        {
            if (string.IsNullOrEmpty(PlainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("secretKey");

            string encryptedString = null;                       // Encrypted string to return
            RijndaelManaged aesAlgorithm = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(secretKey, _salt);

                // Create a RijndaelManaged object
                aesAlgorithm = new RijndaelManaged();
                aesAlgorithm.Key = key.GetBytes(aesAlgorithm.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlgorithm.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlgorithm.IV, 0, aesAlgorithm.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(PlainText);
                        }
                    }
                    encryptedString = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlgorithm != null)
                    aesAlgorithm.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return encryptedString;
        }

        public static string DecryptString(string CipherText)
        {
            if (string.IsNullOrEmpty(CipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("secretKey");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlgorithm = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(secretKey, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(CipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlgorithm = new RijndaelManaged();
                    aesAlgorithm.Key = key.GetBytes(aesAlgorithm.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlgorithm.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlgorithm != null)
                    aesAlgorithm.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
