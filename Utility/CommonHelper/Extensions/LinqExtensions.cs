using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace CommonHelper
{
    /// <summary>
    /// 針對LINQ的相關延伸處理
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// LINQ轉成DataTable型態
        /// </summary>
        public static DataTable LinqToDataTable<T>(this IEnumerable<T> dataList)
        {
            DataTable dtReturn = new DataTable();
            if (dataList == null) 
            {
                return dtReturn;
            }

            // column names 
            PropertyInfo[]? oProps = null;
            foreach (T data in dataList)
            {
                if (data == null) continue;

                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = data.GetType().GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && 
                            (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(data, null) == null ? DBNull.Value : pi.GetValue
                    (data, null);
                }

                dtReturn.Rows.Add(dr);
            }

            return dtReturn;
        }

        /// <summary>
        /// 判斷兩個物件(object)的內容(& Value)是否完全一樣
        /// var matchUsers = Users.Where(isEqual((User u) => u.PhoneNumber == phoneNumber));
        /// </summary>
        public static Expression<Func<TItem,bool>> isEqual<TItem, TProp>(Expression<Func<TItem, TProp>> propAccessor, TProp? other) where TItem :class where TProp : class
        {
            var propElement = propAccessor.Parameters.Single();
            BinaryExpression? conditionalExpression = null;
            foreach(var prop in typeof(TProp).GetProperties())
            {
                BinaryExpression equalExpression;
                object? otherValue = null;
                if (otherValue != null)
                {
                    otherValue = prop.GetValue(other);
                }

                Type propType = prop.PropertyType;
                MemberExpression leftExpression = MakeMemberAccess(propAccessor.Body, prop);
                Expression rightExpression = Convert(Constant(otherValue), propType);
                if (propType.IsPrimitive)
                {
                    equalExpression = Equal(leftExpression, rightExpression);
                }
                else
                {
                    equalExpression = MakeBinary(
                        ExpressionType.Equal, 
                        leftExpression, 
                        rightExpression, 
                        false, 
                        prop.PropertyType.GetMethod("op_Equality"));
                }

                if (conditionalExpression == null)
                {
                    conditionalExpression = equalExpression;
                }
                else
                {
                    conditionalExpression = AndAlso(conditionalExpression, equalExpression);
                }
            }
            
            if (conditionalExpression == null)
            {
                throw new ArgumentException("There should be at least one property.");
            }
            return Lambda<Func<TItem, bool>>(conditionalExpression, propElement);
        }

        /// <summary>
        /// 原Linq的Zip是透過 index 來選擇兩個 ICollection 的 item 進行結合。而 index 的上限，取決於兩個 ICollection 長度較短的那一個集合長度。
        /// MyZip則是依據兩個ICollection中，Index較多的ICollection為主，進行兩個 ICollection 的 item 結合。
        /// </summary>
        public static IEnumerable<TResult> UnitedZip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> firstIterator = first.GetEnumerator())
            using (IEnumerator<TSecond> secondIterator = second.GetEnumerator())
            {
                if (first.Count() > second.Count())
                {
                    while (firstIterator.MoveNext())
                    {
                        while (secondIterator.MoveNext())
                        {
                            yield return resultSelector(firstIterator.Current, secondIterator.Current);
                        }

                        secondIterator.Reset();
                    }
                }
                else if (first.Count() < second.Count())
                {
                    while (secondIterator.MoveNext())
                    {
                        while (firstIterator.MoveNext())
                        {
                            yield return resultSelector(firstIterator.Current, secondIterator.Current);
                        }

                        firstIterator.Reset();
                    }
                }
                else
                {
                    while (firstIterator.MoveNext() && secondIterator.MoveNext())
                    {
                        yield return resultSelector(firstIterator.Current, secondIterator.Current);
                    }
                }
            }
        }

        public static List<T> AddList<T>(this T addItem)
        {
            List<T> itemList = new List<T>();
            itemList.Add(addItem);

            return itemList;
        }
        public static List<T> AddList<T>(this T addItem, T oneMoreItem)
        {
            List<T> itemList = new List<T>();
            itemList.Add(addItem);
            itemList.Add(oneMoreItem);

            return itemList;
        }
    }
}
