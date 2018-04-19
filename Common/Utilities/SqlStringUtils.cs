using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;

namespace ESS.FW.Common.Utilities
{
    /*----------------------------------------------------------------
   // Copyright (C) 2011 九州通集团有限公司 版权所有
   // 
   // 文件名：StringUtils.cs
   // 文件功能描述：String工具类
   // 
   // 创建标识：魏刚20110124
   // 
   // 修改标识：
   // 修改描述：
   // 
   // 修改标识：
   // 修改描述：
   //----------------------------------------------------------------*/
    public class SqlStringUtils
    {
        /// <summary>
        /// 将sql脚本中的"&amp;"替换成":"
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static string ReplaceAndToColon(string sqlString)
        {
            // 去掉左右两边的字符，加上一个分号（最后的一个分号会执行 sqlList.Add(sql)，如下）
            //String sql = RemoveSqlComments(sqlString).Trim('\r', '\n', ';', ' ') + ";";

            // SQL开始的位置
            //s int startIdx = 0;
            bool isFind = true; // 根据单引号进行切换（单引号里面的为字符串）

            char[] c = sqlString.ToCharArray();

            string newSqlString;

            for (int i = 0; i < c.Length; i++)
            {
                // 如果第一次发现了单引号，则认为后面的分号不是分隔符，而是字符串中的一部分。
                // 如果第二次发现了单引号，则认为后面的分号是分隔符。
                // 如果第三次发现了单引号，以此类推，则认为后面的分号不是分隔符.....
                if (c[i] == '\'')
                {
                    isFind = !isFind;
                }

                if (isFind && c[i] == '&')
                {
                    int j = 1;

                    if ((c[i + j] <= 'Z' && c[i + j] >= 'A') || (c[i + j] <= 'z' && c[i + j] >= 'a') || c[i + j] == '_' || (c[i + j] <= '9' && c[i + j] >= '0'))
                    {
                        if ((c[i + j] != '_' && (c[i + j] > '9' || c[i + j] < '0')))
                        {
                            c[i] = ':';
                        }
                    }
                }
                // 如果发现了分号，该分号不是字符串中的一部分，则分割。
                if (isFind && c[i] == ';')
                {
                    newSqlString = new string(c);
                    return newSqlString;
                }
            }
            newSqlString = new string(c);
            return newSqlString;
        }

        /// <summary>
        /// 获取SQL中变量参数的名称
        /// </summary>
        /// <param name="sqlString">SQL脚本</param>
        /// <param name="includeAndChar">是否包含与号参数检测（oracle参数写法一般为:branchid，也支持参数&branchid的写法）</param>
        /// <returns>返回小写参数名称列表</returns>
        public static List<String> BuildParameters(String sqlString, bool includeAndChar)
        {
            // 去掉左右两边的字符，加上一个分号（最后的一个分号会执行 sqlList.Add(sql)，如下）
            String sqls = RemoveSqlComments(sqlString).Trim('\r', '\n', ';', ' ') + ";";

            List<String> paramList = new List<string>();
            String sql = null;
            // SQL开始的位置
            //s int startIdx = 0;
            bool isFind = true; // 根据单引号进行切换（单引号里面的为字符串）


            for (int i = 0; i < sqls.Length; i++)
            {
                // 如果第一次发现了单引号，则认为后面的分号不是分隔符，而是字符串中的一部分。
                // 如果第二次发现了单引号，则认为后面的分号是分隔符。
                // 如果第三次发现了单引号，以此类推，则认为后面的分号不是分隔符.....
                if (sqls[i] == '\'')
                {
                    isFind = !isFind;
                }

                if (isFind && (sqls[i] == ':' || (sqls[i] == '&' && includeAndChar)))
                {
                    int j = 1;
                    while ((sqls[i + j] <= 'Z' && sqls[i + j] >= 'A') || (sqls[i + j] <= 'z' && sqls[i + j] >= 'a') || sqls[i + j] == '_' || (sqls[i + j] <= '9' && sqls[i + j] >= '0'))
                    {
                        if (j == 1 && (sqls[i + j] == '_' || (sqls[i + j] <= '9' && sqls[i + j] >= '0')))
                        {
                            break;
                        }
                        j++;
                    }

                    if (j >= 2)
                    {
                        string pLower = sqls.Substring(i + 1, j - 1).ToLower();
                        if (!paramList.Contains(pLower))
                            paramList.Add(pLower);
                    }

                }
                // 如果发现了分号，该分号不是字符串中的一部分，则分割。
                if (isFind && sqls[i] == ';')
                {
                    return paramList;
                }
            }
            return paramList;
        }
        /// <summary>
        /// SQL语句分割函数（SQL语句一般都是使用分号;进行分割）。
        /// 该函数避免了对字符串中的分号进行分割。
        /// 比如: select ';1' from table;select ';2' from table
        /// 该函数只分割为两个SQL select ';1' from table 和 select ';2' from table
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static String[] SqlSplit(String sqlString)
        {
            // 去掉左右两边的字符，加上一个分号（最后的一个分号会执行 sqlList.Add(sql)，如下）
            String sqls = RemoveSqlComments(sqlString).Trim('\r', '\n', ';', ' ') + ";";

            List<String> sqlList = new List<string>();
            String sql = null;
            // SQL开始的位置
            int startIdx = 0;
            bool isFind = true;

            for (int i = 0; i < sqls.Length; i++)
            {
                // 如果第一次发现了单引号，则认为后面的分号不是分隔符，而是字符串中的一部分。
                // 如果第二次发现了单引号，则认为后面的分号是分隔符。
                // 如果第三次发现了单引号，以此类推，则认为后面的分号不是分隔符.....
                if (sqls[i] == '\'')
                {
                    isFind = !isFind;
                }
                // 如果发现了分号，该分号不是字符串中的一部分，则分割。
                if (isFind && sqls[i] == ';')
                {
                    // 截取SQL
                    sql = sqls.Substring(startIdx, i - startIdx).Trim('\r', '\n');
                    if (!String.IsNullOrEmpty(sql))
                        sqlList.Add(sql);

                    // 重新定位
                    startIdx = i + 1;
                }
            }
            return sqlList.ToArray();
        }

        /// <summary>
        /// 移出SQL注释
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static String RemoveSqlComments(String sql)
        {
            Regex regCmtsFilter1 = new Regex(@"--(.*)");
            Regex regCmtsFilter2 = new Regex(@"\/\*((.)*?)\*\/", RegexOptions.Singleline);

            sql = regCmtsFilter1.Replace(sql, " ");
            sql = regCmtsFilter2.Replace(sql, " ");

            return sql;
        }

        /// <summary>
        /// 根据sqlString语句构建参数到命令对象。
        /// </summary>
        /// <param name="sqlString"></param>
        [Obsolete("该函数存在问题，请使用BuildParameters(String sqlString, bool includeAndChar)")]
        public static List<String> BuildParameters(string sqlString)
        {
            List<String> paramList = new List<String>();
            Regex regex = new Regex(@":([\w]+)");
            //去注释
            String str = RemoveSqlComments(sqlString);

            MatchCollection matches = regex.Matches(str);

            if (matches != null)
            {
                foreach (Match match in matches)
                {

                    String paramName = match.Groups[1].Value.ToLower();

                    if (!paramList.Contains(paramName))
                        paramList.Add(paramName);
                }
            }

            return paramList;
        }


        /// <summary>
        /// 差集操作（由所有属于A且不属于B的元素组成的集合）
        /// </summary>
        public static IEnumerable<string> Except(ICollection<String> listA, ICollection<String> listB)
        {
            Hashtable h1 = new Hashtable();
            foreach (string v in listB)
            {
                h1.Add(v, true);
            }
            if (h1.Count == 0)
            {
                foreach (string v in listA)
                {
                    yield return v;
                }
            }
            else
            {
                foreach (string v in listA)
                {
                    if (!h1.ContainsKey(v))
                    {
                        yield return v;
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// 交集操作
        /// </summary>
        public static IEnumerable<string> Intersect(params ICollection<String>[] args)
        {
            if (args.Length == 1)
            {
                return args[0];
            }
            if (args.Length == 2)
            {
                return new List<string>(pIntersect(args[0], args[1]));
            }
            List<string> ret = new List<string>(args[0]);
            for (int i = 1; i < args.Length; i++)
            {
                ret = new List<string>(pIntersect(ret, args[i]));
            }
            return ret;
        }

        static IEnumerable<string> pIntersect(ICollection<String> arg1, ICollection<String> arg2)
        {
            Hashtable h1 = new Hashtable();
            foreach (string v in arg1)
            {
                h1.Add(v, true);
            }
            foreach (string v in arg2)
            {
                if (h1.ContainsKey(v))
                {
                    yield return v;
                }
            }
            yield break;
        }

        /// <summary>
        /// 并集操作
        /// </summary>
        public static IEnumerable<string> Union(params ICollection<String>[] args)
        {
            Hashtable h = new Hashtable();
            foreach (IEnumerable enumator in args)
            {
                foreach (string element in enumator)
                {
                    if (!h.ContainsKey(element))
                    {
                        h[element] = true;
                        yield return element;
                    }
                }
            }
            yield break;
        }
    }
}
