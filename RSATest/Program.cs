using System;
using System.Text;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.IO;

namespace RSATest
{
    class Program
    {
        static void Main(string[] args)
        {
            var cryptoServiceProvider = new RSACryptoServiceProvider(2048);
            var privateKey = cryptoServiceProvider.ExportParameters(true);
            var publickey = cryptoServiceProvider.ExportParameters(false);

            string publicKeyString = Getkeystring(publickey);
            string privateKeyString = Getkeystring(privateKey);

            /*Console.WriteLine("PublicKey: {0}", publicKeyString);
            Console.WriteLine("PrivateKey: {0}", privateKeyString);*/
            Console.WriteLine("Encryption and Decryption Program!");
            Console.Write("Text to encrypt: ");
            string textTOencrypt = Console.ReadLine();
            Console.WriteLine("Text to Encrypt: {0}", textTOencrypt);
            string encryptedTEXT = Encrypt(textTOencrypt, publicKeyString);
            Console.WriteLine("Encrypted text: {0}", encryptedTEXT);
            string decryptedTEXT = Decrypt(encryptedTEXT, privateKeyString);
            Console.WriteLine("Decrypted text: {0}", decryptedTEXT);
            TOFile(encryptedTEXT, decryptedTEXT);
        }

        public static string Getkeystring(RSAParameters rSAParameters)
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, rSAParameters);
            return stringWriter.ToString();
        }

        public static string Encrypt(string textToEncrypt, string publicKeyString)
        {
            var byteToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(publicKeyString.ToString());
                    var encryptedDATA = rsa.Encrypt(byteToEncrypt, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedDATA);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
                
            }
        }

        public static string Decrypt(string textTOdecrypt, string privatekeyString)
        {
            var byteToDecrypt = Encoding.UTF8.GetBytes(textTOdecrypt);

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(privatekeyString);
                    var resultByte = Convert.FromBase64String(textTOdecrypt);
                    var decryptedByte = rsa.Decrypt(resultByte, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedByte);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        private static string Generateteststring()
        {
            Guid opportinityID = Guid.NewGuid();
            Guid sysuserID = Guid.NewGuid();
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("opportunityid={0}", opportinityID.ToString());
            sb.AppendFormat("&systemuserid={0}", sysuserID.ToString());
            sb.AppendFormat("&currenttime={0}", currentTime);

            return sb.ToString();
        }

        public static void TOFile(string encrypted, string decrypted)
        {
            FileStream fileStream;
            StreamWriter streamWriter;
            TextWriter textWriter = Console.Out;
            string path = "./Encrypted.txt";
            //string appendText = Environment.NewLine + "Encrypted :" + encrypted + Environment.NewLine + "Decrypted: " + decrypted +Environment.NewLine + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string pathBackup = "./Encrypted.txt.bac";
            if (File.Exists(path))
            {
                File.Delete(pathBackup);
                File.Move(path, pathBackup);
                File.Delete(path);
            }
            try
            {
                fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                streamWriter = new StreamWriter(fileStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't open Test.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(streamWriter);
            Console.WriteLine("Encrypted: {0}", encrypted);
            Console.WriteLine("Decrypted: {0}", decrypted);
            Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            Console.SetOut(textWriter);
            streamWriter.Close();
            fileStream.Close();
            Console.WriteLine("Done!");
        }
    }
}
