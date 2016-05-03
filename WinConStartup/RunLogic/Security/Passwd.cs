using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace WinConStartup.RunLogic.Security
{
    public class Passwd
    {
        private const string DECFILENAME = "dec.xml";

        private const string PASWFSALT =
            @"9{*)S[tSK@{zq>t,b-^4+VzMSx7Zg[9W{'Y4R;fzqf\4HL;X=Gkee{Fq@)2mtabgM[}A^pqqdR$`j26ac_->S4p5V]%HM*2b_m9";

        private readonly string _sFile;


        public Passwd(string file)
        {
            _sFile = file;
        }

        public bool ComparePass(string user, string passwd)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(_sFile);
            XmlNode xnode = xdoc.SelectSingleNode("root/str/str[@user='" + hashString(user) + "']");
            string passw = "";
            if (xnode?.Attributes != null) passw = xnode.Attributes["passw"].InnerText;
            return hashString(passwd).Equals(passw);
        }

        public void CreateUser(string user, string passwd)
        {
            XmlDocument xdoc = new XmlDocument();
            if (!File.Exists(_sFile))
            {
                XmlElement xel_1 = xdoc.CreateElement("root");
                XmlElement xel_2 = xdoc.CreateElement("str");
                xel_1.AppendChild(xel_2);
                xdoc.AppendChild(xel_1);
            }
            else
            {

                xdoc.Load(_sFile);
            }
            XmlElement nUser = xdoc.CreateElement("str");
            nUser.SetAttribute("user", hashString(user));
            nUser.SetAttribute("passw", hashString(passwd));
            xdoc.SelectSingleNode("/root/str")?.AppendChild(nUser);
            xdoc.Save(_sFile);
        }

        private string hashString(string str)
        {
            if (str == null) str = "";
            SHA512 alg = SHA512.Create();
            byte[] data;
            try
            {
                data = Convert.FromBase64String(str);
            }
            catch (FormatException)
            {
                data = Encoding.UTF8.GetBytes(str);
            }
            return Convert.ToBase64String(alg.ComputeHash(data));
        }

        public string mixBytes(string str)
        {
            SHA512 alg = SHA512.Create();
            return Encoding.UTF8.GetString(alg.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }


        //-----------------------------------------------ENCRYPTION SITE--------------------------------------------------//



        public readonly byte[] Salt =
        {
            0xf6, 0x30, 0x88, 0x06, 0x7e, 0x92, 0x7d, 0x0f, 0xe5, 0xfd, 0x4b, 0x7b, 0xf8,
            0xd6, 0x60, 0xc4, 0x5b, 0x2c, 0x63, 0xc9
        }; // Must be at least eight bytes.  MAKE THIS SALTIER!

        public const int Iterations = 5042; // Recommendation is >= 1000.

        /// <summary>Decrypt a file.</summary>
        /// <remarks>NB: "Padding is invalid and cannot be removed." is the Universal CryptoServices error.  Make sure the password, salt and iterations are correct before getting nervous.</remarks>
        /// <param name="sourceFilename">The full path and name of the file to be decrypted.</param>
        /// <param name="destinationFilename">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the decryption.</param>
        /// <param name="salt">The salt to be applied to the password.</param>
        /// <param name="iterations">The number of iterations Rfc2898DeriveBytes should use before generating the key and initialization vector for the decryption.</param>
        public void DecryptFile(string sourceFilename, string destinationFilename, string password, byte[] salt,
            int iterations)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize/8);
            aes.IV = key.GetBytes(aes.BlockSize/8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

            using (
                FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None))
            {
                using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                {
                    try
                    {
                        using (
                            FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read,
                                FileShare.Read))
                        {
                            source.CopyTo(cryptoStream);
                        }
                    }
                    catch (CryptographicException exception)
                    {
                        if (exception.Message == "Padding is invalid and cannot be removed.")
                            throw new ApplicationException(
                                "Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                        else
                            throw;
                    }
                }
            }
        }

        /// <summary>Encrypt a file.</summary>
        /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
        /// <param name="destinationFilename">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the encryption.</param>
        /// <param name="salt">The salt to be applied to the password.</param>
        /// <param name="iterations">The number of iterations Rfc2898DeriveBytes should use before generating the key and initialization vector for the decryption.</param>
        public void EncryptFile(string sourceFilename, string destinationFilename, string password, byte[] salt,
            int iterations)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize/8);
            aes.IV = key.GetBytes(aes.BlockSize/8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

            using (
                FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None))
            {
                using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                {
                    using (
                        FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read,
                            FileShare.Read))
                    {
                        source.CopyTo(cryptoStream);
                    }
                }
            }
        }
    }
}