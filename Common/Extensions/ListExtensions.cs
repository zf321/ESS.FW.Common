using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace ESS.FW.Common.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Sort35<T>(this System.Collections.Generic.List<T> list, Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("Comparison");
            int length = list.Count;
            if (length > 0)
            {
                IComparer<T> comparer = new FunctorComparer<T>(comparison);

                int index = 0;

                if (list == null)
                    throw new ArgumentNullException("array");
                if (index < 0 || length < 0)
                    throw new ArgumentOutOfRangeException((length < 0 ? "length" : "index") + "ArgumentOutOfRange_NeedNonNegNum");

                //if (length > 1 && ((comparer != null && comparer != Comparer<T>.Default) || !TrySZSort(list.ToArray(), null, index, (index + length) - 1)))
                if(length>1)
                {
                    try
                    {
                        if (comparer == null)
                            comparer = Comparer<T>.Default;
                        T[] keys = list.ToArray<T>();
                        QuickSort(keys, index, index + (length - 1), comparer);
                        return keys.ToList();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        object[] values = new object[3];
                        values[1] = typeof(T).Name;
                        values[2] = comparer;
                        throw new ArgumentException("Arg_BogusIComparer" + values);
                    }
                }
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        private static extern bool TrySZSort(Array keys, Array items, int left, int right);

        private static void QuickSort<T>(T[] keys, int left, int right, IComparer<T> comparer)
        {
            do
            {
                int a = left;
                int b = right;
                int num3 = a + ((b - a) >> 1);
                SwapIfGreaterWithItems(keys, comparer, a, num3);
                SwapIfGreaterWithItems(keys, comparer, a, b);
                SwapIfGreaterWithItems(keys, comparer, num3, b);
                T y = keys[num3];
                do
                {
                    while (comparer.Compare(keys[a], y) < 0)
                    {
                        a++;
                    }
                    while (comparer.Compare(y, keys[b]) < 0)
                    {
                        b--;
                    }
                    if (a > b)
                    {
                        break;
                    }
                    if (a < b)
                    {
                        T local2 = keys[a];
                        keys[a] = keys[b];
                        keys[b] = local2;
                    }
                    a++;
                    b--;
                }
                while (a <= b);
                if ((b - left) <= (right - a))
                {
                    if (left < b)
                    {
                        QuickSort(keys, left, b, comparer);
                    }
                    left = a;
                }
                else
                {
                    if (a < right)
                    {
                        QuickSort(keys, a, right, comparer);
                    }
                    right = b;
                }
            }
            while (left < right);
        }

        private static void SwapIfGreaterWithItems<T>(T[] keys, IComparer<T> comparer, int a, int b)
        {
            if ((a != b) && (comparer.Compare(keys[a], keys[b]) > 0))
            {
                T local = keys[a];
                keys[a] = keys[b];
                keys[b] = local;
            }
        }

        sealed class FunctorComparer<T> : IComparer<T>
        {
            // Fields
            private Comparer<T> c;
            private Comparison<T> comparison;

            // Methods
            public FunctorComparer(Comparison<T> comparison)
            {
                this.c = Comparer<T>.Default;
                this.comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return this.comparison(x, y);
            }
        }


    }
}
