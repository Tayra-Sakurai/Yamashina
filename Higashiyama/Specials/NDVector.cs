using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Higashiyama.Specials
{
    internal partial class NDVector<TNumber> : IList<TNumber>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>, INumberBase<TNumber>, IRootFunctions<TNumber>
    {
        private TNumber[] data = [];

        public TNumber this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }

        public int IndexOf(TNumber value)
        {
            List<TNumber> doubles = [.. data ];
            return doubles.IndexOf(value);
        }

        public void Insert(int index, TNumber value)
        {
            List<TNumber> doubles = [.. data];
            doubles.Insert(index, value);
            data = [.. doubles ];
        }

        public void RemoveAt(int index)
        {
            List<TNumber> doubles = [.. data];
            doubles.RemoveAt(index);
            data = [.. doubles];
        }

        public bool Contains(TNumber value)
        {
            return data.Contains(value);
        }

        public void Add(TNumber value)
        {
            List<TNumber> list = [.. data];
            list.Add(value);
            data = [.. list];
        }

        public void Clear()
        {
            data = [];
        }

        public void CopyTo(TNumber[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        public bool Remove(TNumber value)
        {
            List<TNumber> doubles = [.. data];
            bool result = doubles.Remove(value);
            data = [.. doubles];
            return result;
        }

        public int Count => data.Length;

        public bool IsReadOnly => false;

        IEnumerator<TNumber> IEnumerable<TNumber>.GetEnumerator()
        {
            List<TNumber> doubles = [.. data];
            IEnumerable<TNumber> doubles1 = doubles;
            return doubles1.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public TNumber Norm => TNumber.Sqrt(this * this);

        public static TNumber operator *(NDVector<TNumber> v1, NDVector<TNumber> v2)
        {
            if (v1.Count != v2.Count)
                throw new ArgumentException("Dimentions must be same.");
            TNumber result = TNumber.Zero;
            for (int i = 0; i < v1.Count; i++)
                result += v1[i] * v2[i];
            return result;
        }

        public static NDVector<TNumber> operator *(TNumber coefficient, NDVector<TNumber> v)
        {
            List<TNumber> doubles = [];
            foreach (TNumber d in v)
                doubles.Add(d *  coefficient);
            NDVector<TNumber> result = [.. doubles];
            return result;
        }

        public static NDVector<TNumber> operator +(NDVector<TNumber> v1, NDVector<TNumber> v2)
        {
            if (v1.Count != v2.Count)
                throw new ArgumentException("Dimentioins of vectors must be same.");
            List<TNumber> doubles = [];
            for (int i = 0;i < v1.Count;i++)
                doubles.Add(v1[i] + v2[i]);
            NDVector<TNumber> result = [.. doubles];
            return result;
        }

        public static NDVector<TNumber> operator -(NDVector<TNumber> v)
        {
            List<TNumber> doubles = [.. v];
            for (int i = 0; i < doubles.Count; i++)
                doubles[i] = -doubles[i];
            NDVector<TNumber> result = [.. doubles];
            return result;
        }

        public static NDVector<TNumber> operator -(NDVector<TNumber> v1, NDVector<TNumber> v2)
        {
            return v1 + (-v2);
        }
    }
}
