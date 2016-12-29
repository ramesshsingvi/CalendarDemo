using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SchedulerCustomEditor.Models
{
    public class EncryptDecrypt
    {
        private static string DefaultKey = "FacebookAppStrongKey";
        public static string EncryptString(string Password)
        {
            string Value = DefaultKey;

            if (string.IsNullOrEmpty(Value))
                return "";
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(Value);

            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                               0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(clearBytes, 0, clearBytes.Length);

            cs.Close();

            byte[] encryptedData = ms.ToArray();

            return Convert.ToBase64String(encryptedData);
        }

        public static string DecryptString(string Password)
        {
            string Value = DefaultKey;
            try
            {
                if (string.IsNullOrEmpty(Value))
                    return "";
                byte[] cipherBytes = Convert.FromBase64String(Value);

                MemoryStream ms = new MemoryStream();
                Rijndael alg = Rijndael.Create();

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
                               0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(cipherBytes, 0, cipherBytes.Length);

                cs.Close();

                byte[] decryptedData = ms.ToArray();

                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            catch (FormatException fex)
            {
                if (fex.Message == "Invalid length for a Base-64 char array.")
                    return Value;
            }
            return Value;
        }
    }
}