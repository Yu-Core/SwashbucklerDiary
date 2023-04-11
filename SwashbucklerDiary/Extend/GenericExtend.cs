using System.Reflection;

namespace SwashbucklerDiary.Extend
{
#nullable disable
    public static  class GenericExtend
    {
        public static T DeepCopy<T>(this T obj)
        {
            if (obj == null)
            {
                return default;
            }

            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            object copy = Activator.CreateInstance(type);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(obj);
                if (fieldValue == null)
                {
                    continue;
                }

                field.SetValue(copy, DeepCopy(fieldValue));
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite || !property.CanRead)
                {
                    continue;
                }

                object propertyValue = property.GetValue(obj, null);
                if (propertyValue == null)
                {
                    continue;
                }

                property.SetValue(copy, DeepCopy(propertyValue), null);
            }

            return (T)copy;
        }
    }
}
