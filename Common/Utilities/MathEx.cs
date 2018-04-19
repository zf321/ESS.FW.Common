using System;

namespace ESS.FW.Common.Utilities
{
    /*----------------------------------------------------------------
// Copyright (C) 2011 九州通集团有限公司
// 版权所有。 
//
// 文件名：MathEx.cs
// 文件功能描述：函数的扩展。。
     * 
     * // 
// 创建标识：朱江

//----------------------------------------------------------------*/

    public static class MathEx
    {
        //根据配置可以调置调整四舍五入的方式 为 四舍五入 还是 银行家传入。
        public static decimal Round(decimal d, int i)
        {
            return Math.Round(d, i, MidpointRounding.AwayFromZero);
        }

    }
}
