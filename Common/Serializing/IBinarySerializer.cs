#region

using System;
using System.Collections.Generic;

#endregion

namespace ESS.FW.Common.Serializing
{
    public interface IBinarySerializer
    {
        /// <summary>
     ///     Serialize an object to byte array.
     /// </summary>
     /// <param name="obj"></param>
     /// <returns></returns>
        byte[] Serialize(object obj);
        /// <summary>
        ///     Deserialize an object from a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(byte[] data, Type type);
        /// <summary>
        ///     Deserialize a typed object from a byte array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] data) where T : class;
        /// <summary>
        ///     Deserialize a typed object from a byte array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerable<T> Deserialize<T>(byte[][] data) where T : class;
    }
}