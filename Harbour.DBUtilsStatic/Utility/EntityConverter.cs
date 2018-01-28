using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Harbour.DBUtilsStatic
{
    /// <summary>
    /// 实体转换类（默认有缓存）
    /// </summary>
    public static class EntityConverter
    {
        /// <summary>
        /// 将DataRow转为List
        /// </summary>
        /// <typeparam name="T">实体类（必须有默认构造参数）</typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns></returns>
        public static T ToEntity<T>(DataRow dr) where T : new()
        {
            if (dr == null)
                return default(T);

            T t = new T();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (dr.Table.Columns.Contains(prop.Name))
                {
                    GetSetter<T>(prop)(t, dr[prop.Name]);
                }
            }
            return t;
        }
        /// <summary>
        /// 将IDataReader转换为实体
        /// </summary>
        /// <typeparam name="T">实体类（必须有默认构造参数）</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <returns></returns>
        public static T ToEntity<T>(IDataReader dr) where T : new()
        {
            T t = default(T);
            if (dr.Read())
            {
                t = new T();
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    GetSetter<T>(prop)(t, dr[prop.Name]);
                }
            }
            return t;
        }

        /// <summary>
        /// 将DataTable转为List
        /// </summary>
        /// <typeparam name="T">实体类（必须有默认构造参数）</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> ToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    if (dr.Table.Columns.Contains(prop.Name))
                    {
                        GetSetter<T>(prop)(t, dr[prop.Name]);
                    }
                }
                list.Add(t);
            }

            return list;
        }
        /// <summary>
        /// 将IDataReader转为实体
        /// </summary>
        /// <typeparam name="T">实体类（必须有默认构造参数）</typeparam>
        /// <param name="dr">IDataReader</param>
        /// <returns></returns>
        public static List<T> ToList<T>(IDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            while (dr.Read())
            {
                T t = new T();
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    GetSetter<T>(prop)(t, dr[prop.Name]);
                }
                list.Add(t);
            }
            return list;
        }


        static readonly Dictionary<string, object> _retActDic = new Dictionary<string, object>();
        private static Action<T, object> GetSetter<T>(PropertyInfo property)
        {
            Type type = typeof(T);
            string key = type.AssemblyQualifiedName + "_set_" + property.Name;

            object retAct;
            if (!_retActDic.TryGetValue(key, out retAct))
            {
                lock (key)
                {
                    if (!_retActDic.TryGetValue(key, out retAct))
                    {
                        //创建 对实体 属性赋值的expression
                        ParameterExpression parameter = Expression.Parameter(type, "t");
                        ParameterExpression value = Expression.Parameter(typeof(object), "propertyValue");
                        MethodInfo setter = type.GetMethod("set_" + property.Name);
                        MethodCallExpression call = Expression.Call(parameter, setter, Expression.Convert(value, property.PropertyType));
                        var lambda = Expression.Lambda<Action<T, object>>(call, parameter, value);
                        retAct = lambda.Compile();
                        _retActDic.Add(key, retAct);
                    }
                }
            }
            return retAct as Action<T, object>;
        }
    }
}
