using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESS.FW.Common.Utilities
{
    public class DesTools
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public const string ENCRYPT = "plsqldev";



        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString)
        {

            try
            {

                byte[] rgbKey = Encoding.UTF8.GetBytes(ENCRYPT.Substring(0, 8));

                byte[] rgbIV = Keys;

                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);

                cStream.Write(inputByteArray, 0, inputByteArray.Length);

                cStream.FlushFinalBlock();

                return Convert.ToBase64String(mStream.ToArray());

            }

            catch
            {
                throw;
            }
            return encryptString;
        }


        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <returns></returns>
        public static string DecryptDES(string decryptString)
        {

            try
            {


                byte[] rgbKey = Encoding.UTF8.GetBytes(ENCRYPT.Substring(0, 8));

                byte[] rgbIV = Keys;

                byte[] inputByteArray = Convert.FromBase64String(decryptString);

                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);

                cStream.Write(inputByteArray, 0, inputByteArray.Length);

                cStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(mStream.ToArray());

            }

            catch
            {
                throw;

                return decryptString;

            }

        }



        /// <summary>
        /// 加密标记，防止反复加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString, string mark)
        {
            int len = mark.Length;
            string temp = encryptString.Substring(0, len);

            if (mark == temp)
            {
                return encryptString;
            }
            else
            {
                return mark + EncryptDES(encryptString);
            }
        }
        /// <summary>
        /// 解密标记
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string DecryptDES(string encryptString, string mark)
        {
            string sub = encryptString.Substring(mark.Length, encryptString.Length - mark.Length);
            return DecryptDES(sub);

        }


        public static string SerializeText(object obj)
        {
            Type t = obj.GetType();

            //对象 序列化 成 xml .
            StringBuilder sb = new StringBuilder();
            using (StringWriter tw = new StringWriter(sb))
            {
                XmlSerializer sz1 = new XmlSerializer(t);
                sz1.Serialize(tw, obj);
            }

            return sb.ToString();

        }

        /// <summary>
        /// 将字符保存到文件中。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="s"></param>
        public static void SaveToFile(string fileName, string s)
        {
            using (FileStream tempStream = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(tempStream))
                {
                    writer.WriteLine(s);
                }
            }
        }
    }
}
