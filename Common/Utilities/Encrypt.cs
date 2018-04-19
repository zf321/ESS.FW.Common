using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESS.FW.Common.Utilities
{
    public class Encrypt
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public const string ENCRYPT = "jiuzhoutongeyao";

        /// <summary>
        /// 序列化对象二进制文件
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fileName"></param>
        public static void Serializable(object o, string fileName)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            IFormatter Formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Formatter.Serialize(stream, o);
            stream.Close();

        }

        /// <summary>
        /// 序列化对象到byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(Object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            IFormatter bf = new BinaryFormatter();
            bf.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            byte[] content = new byte[((int)memoryStream.Length) + 1];
            memoryStream.Read(content, 0, content.Length);
            return content;


        }

        public static MemoryStream ConvertToMemoryStream(Object obj)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlSerializer ser = new XmlSerializer(obj.GetType());

            //IFormatter bf = new BinaryFormatter();
            ser.Serialize(memoryStream, obj);

            return memoryStream;
        }

        /// <summary>
        /// 序列化对象到XML 文件
        /// </summary>
        /// <param name="objectToConvert"></param>
        /// <param name="path"></param>
        public static void SerializeXML(object objectToConvert, string path)
        {
            if (objectToConvert != null)
            {
                Type t = objectToConvert.GetType();

                XmlSerializer ser = new XmlSerializer(t);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    ser.Serialize(writer, objectToConvert);
                    writer.Close();
                }
            }

        }

        public static void SerializeXML(Type type, Type[] extraTypeList, object objectToConvert, string path)
        {
            if (objectToConvert != null)
            {
                //Type t = objectToConvert.GetType();
                //复制原文件作为备份。
                int pos = path.LastIndexOf(@"\");//获取最后一个 文件分隔符的位置。
                string dirname = path.Substring(0, pos + 1);
                string filename = path.Substring(pos + 1, path.Length - pos - 1);

                string tmpFileName = string.Format(@"{0}{1}_{2}", dirname, filename, DateTime.Now.ToFileTime());

                if (File.Exists(path))
                {
                    File.Copy(path, tmpFileName);
                }

                XmlSerializer ser = new XmlSerializer(type, extraTypeList);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    ser.Serialize(writer, objectToConvert);
                    writer.Close();
                }

                if (File.Exists(tmpFileName))
                {
                    //删除用作备份的的文件。
                    File.Delete(tmpFileName);
                }
            }

        }

        public static object DeserializeXml(Type type, Type[] extraTypeList, string fileName)
        {
            if (!File.Exists(fileName)) return null;
            XmlSerializer ser = new XmlSerializer(type, extraTypeList);
            object o = null;
            try
            {
                using (StreamReader read = new StreamReader(fileName))
                {
                    o = ser.Deserialize(read);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return o;
        }

        /// <summary>
        /// 反序列化XML 到对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static object DeserializeXml(string fileName, Type tp)
        {
            if (!File.Exists(fileName)) return null;
            XmlSerializer ser = new XmlSerializer(tp);
            object o = null;
            try
            {
                using (StreamReader read = new StreamReader(fileName))
                {
                    o = ser.Deserialize(read);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return o;
        }

        /// <summary>
        /// 反序列二进制文件到对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static object Deserialize(String fileName, Type ob)
        {
            if (!File.Exists(fileName)) return null;
            object instance = ob.Assembly.CreateInstance(ob.FullName);


            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.None);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                instance = Deserialize(fs, ob);
                return instance;
            }
            catch
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public static object Deserialize(Stream stream, Type ob)
        {
            object obj = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;

        }

        /// <summary>
        /// 将序列化的xml文件反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(string fileName)
        {
            return ((T)DeserializeXml(fileName, typeof(T)));
        }

        public static T DeserializeXml<T>(StreamReader ms)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            T o;
            try
            {
                o = (T)ser.Deserialize(ms);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return o;
        }

        /// <summary>
        /// 将二进制文件序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T Deserialize<T>(String fileName)
        {
            return ((T)Deserialize(fileName, typeof(T)));
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
                return "";

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

        /// <summary>
        /// 加密标记，防止反复加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString, string mark)
        {
            if (string.IsNullOrEmpty(encryptString))
                return "";

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
        /// 解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <returns></returns>
        public static string DecryptDES(string decryptString)
        {
            if (string.IsNullOrEmpty(decryptString))
                return "";

            byte[] rgbKey = Encoding.UTF8.GetBytes(ENCRYPT.Substring(0, 8));

            byte[] rgbIv = Keys;

            byte[] inputByteArray = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, dcsp.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);

            cStream.Write(inputByteArray, 0, inputByteArray.Length);

            cStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(mStream.ToArray());

        }

        /// <summary>
        /// 解密标记
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string DecryptDES(string encryptString, string mark)
        {
            if (string.IsNullOrEmpty(encryptString))
                return "";

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
