using System;
using System.Collections.Generic;
using System.Reflection;

public static class ObjectComparer
{
    public static Dictionary<string, Tuple<object, object>> CompareObjects(object obj1, object obj2)
    {
        Dictionary<string, Tuple<object, object>> diffProperties = new Dictionary<string, Tuple<object, object>>();

        Type type = obj1.GetType();

        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object value1 = property.GetValue(obj1)!;
            object value2 = property.GetValue(obj2)!;

            if (property.Name == "LastUpdateAt" || property.Name == "LastUpdateByUser" || !Equals(value1, value2))
            {
                diffProperties[property.Name] = Tuple.Create(value1!, value2!);
            }
            
        }

        return diffProperties;
    }
}
