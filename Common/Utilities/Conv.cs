using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace ESS.FW.Common.Utilities
{
    [System.Diagnostics.DebuggerStepThrough()]
    public static class Conv
    {
        public static char ConvSBN(char c)
        {
            char result;
            switch (c)
            {
                case '１':
                    result = '1';
                    break;
                case '２':
                    result = '2';
                    break;
                case '３':
                    result = '3';
                    break;
                case '４':
                    result = '4';
                    break;
                case '５':
                    result = '5';
                    break;
                case '６':
                    result = '6';
                    break;
                case '７':
                    result = '7';
                    break;
                case '８':
                    result = '8';
                    break;
                case '９':
                    result = '9';
                    break;
                case '０':
                    result = '0';
                    break;
                default:
                    result = c;
                    break;
            }
            return result;
        }

        public static string ConvSBC(char c)
        {
            //if (c >= 'Ａ' && c <= 'Ｚ')
            //{
            //    int code;
            //    ('Ａ'-'A')+c;
            //}
            //else if (c >= 'ａ' && c <= 'ｚ')
            //{
            //    'ａ'
            //    'a'
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取字符串真正的长度（一个回车占两个字符，一个汉字占两个字符，一个符号或字母占一个长度）
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串真实长度</returns>
        public static int StrLen(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            return Encoding.GetEncoding(936).GetByteCount(str);
        }

        /// <summary>
        /// 转换字符串，如果转换失败，返回<see cref="string.Empty"/>
        /// </summary>
        public static string NS(object value)
        {
            return NS(value, string.Empty);
        }

        /// <summary>
        /// 转换字符串，如果转换失败，返回<see cref="defaultValue"/>
        /// </summary>
        public static string NS(object value, string defaultValue)
        {
            if (value == DBNull.Value)
                return defaultValue;
            if (value == null)
                return defaultValue;
            return value.ToString();
        }

        /// <summary>
        /// 转换任意对象到整型，如果转换失败返回0
        /// </summary>
        public static int NI(object value)
        {
            return NI(value, 0);
        }

        /// <summary>
        /// 转换任意对象到整型，如果转换失败返回defaultValue
        /// </summary>
        public static int NI(object value, int defaultValue)
        {
            if (value == DBNull.Value)
                return defaultValue;
            if (value == null)
                return defaultValue;
            int ret;
            if (int.TryParse(value.ToString(), out ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转换日期对象为字符串，如果日期等于MinValue 则返回空字符串
        /// </summary>
        public static string NDTS(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 转换字符串对象为整型，如果字符串为空，则返回MinValue，转换如果失败则抛出异常
        /// </summary>
        public static DateTime NSDT(string value)
        {
            DateTime ret;
            if (string.IsNullOrEmpty(value))
            {
                return DateTime.MinValue;
            }
            else
            {
                if (DateTime.TryParse(value, out ret))
                {
                    return ret;
                }
                else
                {
                    throw new ArgumentException("非法日期格式");
                }
            }
        }

        /// <summary>
        /// 将任意对象转换成<see cref="System.Int64"/>类型，如果转换失败则返回0
        /// </summary>
        public static long NL(object value)
        {
            return NL(value, 0L);
        }

        /// <summary>
        /// 将任意对象转换为<see cref="System.Int64"/>类型，如果转换失败则返回defaultValue
        /// </summary>
        public static long NL(object value, long defaultValue)
        {
            if (value == DBNull.Value)
                return defaultValue;
            if (value == null)
                return defaultValue;
            long ret;
            if (Int64.TryParse(value.ToString(), out ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将任意对象转换为<see cref="System.Decimal"/>类型，如果转换失败则返回0M
        /// </summary>
        public static decimal NDec(object value)
        {
            return NDec(value, 0M);
        }

        /// <summary>
        /// 将任意对象转换为<see cref="System.Decimal"/>类型，如果转换失败则返回defaultValue
        /// </summary>
        public static decimal NDec(object value, decimal defaultValue)
        {
            if (value == DBNull.Value)
                return defaultValue;
            if (value == null)
                return defaultValue;
            decimal ret;
            if (decimal.TryParse(value.ToString(), out ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将任意对象转换为<see cref="System.DateTime"/>类型，如果转换失败则抛出异常
        /// </summary>
        public static DateTime NDT(object p)
        {
            try
            {
                return DateTime.Parse(p.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("日期转换出错", ex);
            }
        }

        /// <summary>
        /// 将任意对象转换为<see cref="System.DateTime"/>类型，如果转换失败则返回一个默认值
        /// </summary>
        public static DateTime NDT(object p, DateTime defaultValue)
        {
            try
            {
                return DateTime.Parse(p.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
                
                return defaultValue;
            }
        }


        /// <summary>
        /// 将任意对象转换为<see cref="System.DateTime"/>类型，如果转换失败则返回NULL值
        /// </summary>
        public static DateTime? NDTNull(object p)
        {
            try
            {
                return DateTime.Parse(p.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// 判断任意对象是否能转换为<see cref="System.DateTime"/>类型
        /// </summary>
        public static bool IsDate(object value)
        {
            try
            {
                DateTime dt = DateTime.Parse(NS(value));
                if (dt == DateTime.MinValue)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断任意对象是否能转换为<see cref="System.Int64"/>类型
        /// </summary>
        public static bool IsLong(object value)
        {
            if (value == null)
                return false;
            long ret;
            if (long.TryParse(value.ToString(), out ret))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断任意对象是否能转换为<see cref="System.Int32"/>类型
        /// </summary>
        public static bool IsInt(object value)
        {
            if (value == null)
                return false;
            int ret;
            if (int.TryParse(value.ToString(), out ret))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断任意对象是否能转换为<see cref="System.Int16"/>类型
        /// </summary>
        public static bool IsShort(object value)
        {
            if (value == null)
                return false;
            short ret;
            if (short.TryParse(value.ToString(), out ret))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断一个数是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDigital(object value)
        {
            try
            {
                string va = value.ToString();
                string pattern = @"^(-?\d+[.]?\d+)$|^(-?\d+)$";　  
                return Regex.IsMatch(va, pattern);

            }
            catch
            {
                return false;
            }

        }

        public static bool IsDecmail(object value)
        {
            if (value == null)
                return false;
            decimal ret;
            if (decimal.TryParse(value.ToString(), out ret))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static PropertyDescriptor GetPropertyIgnoreCase(PropertyDescriptorCollection propertyDescriptorCollection, string captionMember)
        {
            foreach (PropertyDescriptor pd in propertyDescriptorCollection)
            {
                if (pd.Name.Equals(captionMember, StringComparison.OrdinalIgnoreCase))
                {
                    return pd;
                }
            }
            return null;
        }
    }
}
