using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ScreenActivator.Buisness
{
    public static class Encryption
    {
        private const string SECRETKEY = "b14ca5898a4e4133scvb2ea2315a1916";

        public static string Encrypt(string str)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(SECRETKEY);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(str);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static string Decrypt(string str)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(str);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(SECRETKEY);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public static string StringToHex(string stringvalue)
        {
            Encoding ec = System.Text.Encoding.Unicode;
            Byte[] stringBytes = ec.GetBytes(stringvalue);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return "S" + sbBytes.ToString();
        }
        public static string HexToString(string hexvalue)
        {
            hexvalue = hexvalue.Substring(1);
            Encoding ec = System.Text.Encoding.Unicode;
            int CharsLength = hexvalue.Length;
            byte[] bytesarray = new byte[CharsLength / 2];
            for (int i = 0; i < CharsLength; i += 2)
            {
                bytesarray[i / 2] = Convert.ToByte(hexvalue.Substring(i, 2), 16);
            }
            return ec.GetString(bytesarray);
        }
    }
}
