using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ESS.FW.Common.Utilities
{
    public class ByteConverter
    {
        /// <summary>
        /// 将DataSet格式化成字节数组byte[]
        /// </summary>
        /// <param name="dsOriginal">DataSet对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatData(DataSet dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 将DataSet格式化成字节数组byte[]，并且已经经过压缩
        /// </summary>
        /// <param name="dsOriginal">DataSet对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatDataCompress(DataSet dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return Compress(binaryDataResult);
        }

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            GZipStream stream = new GZipStream(ms, CompressionMode.Decompress, true);
            byte[] buffer = new byte[1024];
            MemoryStream temp = new MemoryStream();
            int read = stream.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                temp.Write(buffer, 0, read);
                read = stream.Read(buffer, 0, buffer.Length);
            }
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            stream.Close();
            stream.Dispose();
            ms.Close();
            ms.Dispose();
            bData = temp.ToArray();
            temp.Close();
            temp.Dispose();
            return bData;
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            GZipStream stream = new GZipStream(ms, CompressionMode.Compress, true);
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            //并且解压缩方法stream.Read(buffer, 0, buffer.Length)时会返回0
            bData = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return bData;
        }

        /// <summary>
        /// 将字节数组反序列化成DataSet对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet RetrieveDataSet(byte[] binaryData)
        {
            DataSet ds = null;
            MemoryStream memStream = new MemoryStream(binaryData, true);
            //byte[] bs = memStream.GetBuffer();

            // memStream.Write(bs, 0, bs.Length);
            //memStream.Seek(0, SeekOrigin.Begin);

            IFormatter brFormatter = new BinaryFormatter();
            ds = (DataSet)brFormatter.Deserialize(memStream);
            return ds;
        }

        /// <summary>
        /// 将字节数组反解压后序列化成DataSet对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet RetrieveDataSetDecompress(byte[] binaryData)
        {
            DataSet dsOriginal = null;
            MemoryStream memStream = new MemoryStream(Decompress(binaryData));
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            dsOriginal = (DataSet)obj;
            return dsOriginal;
        }

        /// <summary>
        /// 将object格式化成字节数组byte[]
        /// </summary>
        /// <param name="dsOriginal">object对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatData(object dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 将objec格式化成字节数组byte[]，并压缩
        /// </summary>
        /// <param name="dsOriginal">object对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryFormatDataCompress(object dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return Compress(binaryDataResult);
        }

        /// <summary>
        /// 将字节数组反序列化成object对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>object对象</returns>
        public static object RetrieveObject(byte[] binaryData)
        {
            MemoryStream memStream = new MemoryStream(binaryData);
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// 将字节数组解压后反序列化成object对象
        /// </summary>
        /// <param name="binaryData">字节数组</param>
        /// <returns>object对象</returns>
        public static object RetrieveObjectDecompress(byte[] binaryData)
        {
            MemoryStream memStream = new MemoryStream(Decompress(binaryData));
            IFormatter brFormatter = new BinaryFormatter();
            Object obj = brFormatter.Deserialize(memStream);
            return obj;
        }

        ///// <summary>
        ///// 解密配置文件并读入到xmldoc中
        ///// </summary>
        //public static XmlNode DecryptConfigFile(string filePath)
        //{
        //    FileStream fs = new FileStream(filePath, FileMode.Open);
        //    XmlDocument m_XmlDoc = new XmlDocument();
        //    BinaryFormatter formatter = null;
        //    try
        //    {
        //        formatter = new BinaryFormatter();
        //        //   Deserialize   the   hashtable   from   the   file   and
        //        //   assign   the   reference   to   the   local   variable.
        //        m_XmlDoc.LoadXml(KCEncrypt.Decrypt((string)formatter.Deserialize(fs)));
        //        return m_XmlDoc.DocumentElement;
        //    }
        //    catch (SerializationException e)
        //    {
        //        Console.WriteLine("Failed   to   deserialize.   Reason:   " + e.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        fs.Close();
        //        fs = null;
        //    }
        //}

        ///// <summary>
        ///// 加密密钥后再对文件字符进行加密
        ///// </summary>
        //public static void EncryptConfigFile(string filePath, string str)
        //{
        //    FileStream fs = new FileStream(filePath, FileMode.Create);
        //    BinaryFormatter formatter = new BinaryFormatter();

        //    try
        //    {
        //        formatter.Serialize(fs, KCEncrypt.Encrypt(str));
        //    }
        //    catch (SerializationException e)
        //    {
        //        Console.WriteLine("Failed   to   serialize.   Reason:   " + e.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        fs.Close();
        //        fs = null;
        //    }
        //}
    }
}
