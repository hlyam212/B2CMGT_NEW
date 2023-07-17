using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace CommonHelper
{
    public static class ModelComparer
    {
        public static ModelComparer<T> Create<T>(IEnumerable<T> oldModels, IEnumerable<T> newModels, params Expression<Func<T, object>>[] compareMembers)
        {
            return new ModelComparer<T>(oldModels, newModels, compareMembers);
        }
    }

    public sealed class ModelComparer<T>
    {
        private IEnumerable<T> _Insert = null;
        private IEnumerable<T> _Update = null;
        private IEnumerable<T> _Delete = null;
        private IEnumerable<T> _Equal = null;

        private String[] _IgnorePropertyName = new[] {
            "iupdate", "dupdate", "icreate", "dcreate"};

        public IEnumerable<T> Insert
        {
            get { return this._Insert; }
        }
        public IEnumerable<T> Update
        {
            get { return this._Update; }
        }
        public IEnumerable<T> Delete
        {
            get { return this._Delete; }
        }
        public IEnumerable<T> Equal
        {
            get { return this._Equal; }
        }

        /// <summary>model的比較器</summary>
        /// <param name="oldModel">原model</param>
        /// <param name="newModel">新model</param>
        /// <param name="memberExpressions">比對的欄位</param>
        public ModelComparer(IEnumerable<T> oldModels, IEnumerable<T> newModels, params Expression<Func<T, object>>[] memberExpressions) : this(oldModels, newModels, GetMembers(memberExpressions))
        {
        }

        internal ModelComparer(IEnumerable<T> oldModels, IEnumerable<T> newModels, IEnumerable<MemberInfo> members)
        {

            if (members == null || !members.Any()) throw new InvalidOperationException("必須指定比對成員");
            if (members.Count() > 7) throw new InvalidOperationException("比對成員最多只能有7個");

            if ((oldModels == null) && (newModels == null))
            {
                throw new ArgumentNullException("oldModels & newModels");
            }

            if ((oldModels == null) && (newModels != null))
            {
                this._Insert = newModels;
                this._Update = newModels.Take(0);
                this._Delete = newModels.Take(0);
                this._Equal = newModels.Take(0);
                return;
            }

            if ((oldModels != null) && (newModels == null))
            {
                this._Delete = oldModels;
                this._Insert = oldModels.Take(0);
                this._Update = oldModels.Take(0);
                this._Equal = oldModels.Take(0);
                return;
            }

            var model = Expression.Parameter(typeof(T), "model");
            var memberTypes = new List<Type>();
            var constructorArgs = new List<Expression>();
            foreach (var n in members)
            {
                if (n.MemberType == MemberTypes.Field)
                {
                    var field = (FieldInfo)n;
                    memberTypes.Add(field.FieldType);
                    constructorArgs.Add(Expression.Field(model, field));
                }
                else
                {
                    var property = (PropertyInfo)n;
                    memberTypes.Add(property.PropertyType);
                    constructorArgs.Add(Expression.Property(model, property));
                }
            }
            var memberTypeArray = memberTypes.ToArray();
            Type tupleType = null;

            if (memberTypeArray.Length.Equals(1))
            {
                tupleType = typeof(Tuple<>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(2))
            {
                tupleType = typeof(Tuple<,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(3))
            {
                tupleType = typeof(Tuple<,,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(4))
            {
                tupleType = typeof(Tuple<,,,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(5))
            {
                tupleType = typeof(Tuple<,,,,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(6))
            {
                tupleType = typeof(Tuple<,,,,,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(7))
            {
                tupleType = typeof(Tuple<,,,,,,>).MakeGenericType(memberTypeArray);
            }
            else if (memberTypeArray.Length.Equals(8))
            {
                tupleType = typeof(Tuple<,,,,,,,>).MakeGenericType(memberTypeArray);
            }

            var tupleConstructor = tupleType.GetConstructor(memberTypeArray);
            var tuple = Expression.New(tupleConstructor, constructorArgs);
            var bodys = new List<Expression>();
            var labelTarget = Expression.Label(typeof(object));
            bodys.Add(Expression.Return(labelTarget, tuple));
            bodys.Add(Expression.Label(labelTarget, Expression.Constant(String.Empty)));
            var block = Expression.Block(bodys);
            var keySelector = Expression.Lambda<Func<T, object>>(block, new[] { model }).Compile();

            this._Insert = newModels.GroupJoin(oldModels, keySelector, keySelector,
                (n, o) => new { n, o }).Where(no => no.o.Any() == false).Select(no => no.n);

            this._Delete = oldModels.GroupJoin(newModels, keySelector, keySelector,
                (n, o) => new { n, o }).Where(no => no.o.Any() == false).Select(no => no.n);

            var joinModel = newModels.Join(oldModels, keySelector, keySelector, (n, o) => new { n, o });

            this._Update = joinModel.Take(0).Select(u => u.n);

            var pdList = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor pdItem in pdList)
            {
                if (this._IgnorePropertyName.Any(ig => String.Compare(ig, pdItem.Name, true) == 0))
                {
                    continue;
                }
                var compare = joinModel.Where(c => Convert.ToString(pdItem.GetValue(c.n)) != Convert.ToString(pdItem.GetValue(c.o)));
                if (compare.Any())
                {
                    var difference = compare.Select(c => c.n).GroupJoin(this._Update, keySelector, keySelector,
                        (c, u) => new { c, u }).Where(cu => cu.u.Any() == false);
                    if (difference.Any())
                    {
                        this._Update = this._Update.Concat(difference.Select(d => d.c));
                    }
                }
            }
            this._Equal = joinModel.Select(j => j.n).GroupJoin(this._Update, keySelector, keySelector,
                (j, u) => new { j, u }).Where(ju => ju.u.Any() == false).Select(ju => ju.j);
        }

        private static MemberInfo GetAccessedMember(LambdaExpression lambda)
        {
            MemberExpression memberExpression = lambda.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member;
            }
            UnaryExpression unaryExpression = lambda.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression != null)
                {
                    return memberExpression.Member;
                }
                MethodCallExpression methodCall = unaryExpression.Operand as MethodCallExpression;
                if (methodCall != null)
                {
                    return methodCall.Method;
                }
            }
            return null;
        }

        private static IEnumerable<MemberInfo> GetMembers(ICollection<Expression<Func<T, object>>> memberExpressions)
        {
            IEnumerable<MemberInfo> result = null;
            if (memberExpressions == null || memberExpressions.Count <= 0)
            {
                return result;
            }
            if (memberExpressions.Any(m => m == null))
            {
                return result;
            }
            try
            {
                result = memberExpressions.Select(x => GetAccessedMember(x));
            }
            catch
            {
            }
            return result;
        }
    }
}
