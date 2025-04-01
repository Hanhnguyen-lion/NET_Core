using System.Data;
using Newtonsoft.Json.Linq;

namespace MyApi.Helper{
    public static class CommonHelper{

        public static T? Get<T>(this IDataReader reader, string key, T? defaultValue = default(T))
        {
            var val = reader[key];
            return To<T>(val, defaultValue);
        }        
        public static T? To<T>(this object obj, T? defaultValue = default)
        {
            if (obj is T)
                 return (T)obj;

            if (obj == null)
                return defaultValue;

            if (obj.Equals(DBNull.Value))
                return defaultValue;

            if ((obj is JValue) && ((JValue)obj).Value == null)
                return defaultValue;

            var type = typeof(T);
            if (type.IsEnum)
            {
                if (obj is string s_obj)
                {
                    return (T)Enum.Parse(typeof(T), s_obj);
                } else
                    return (T)Convert.ChangeType(obj, typeof(int));
            }

            if (type == typeof(string))
            {
                return (T?)(object?)obj.ToString();
            }

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return (T)Convert.ChangeType(obj, nullableType);

            if (obj is JToken j_obj)
            {
                return j_obj.ToObject<T>();
            }

            if (obj is System.Collections.BitArray ba)
            {
                return ba[0].To<T>();
            }

            return (T)Convert.ChangeType(obj, type);
        }
    }
}