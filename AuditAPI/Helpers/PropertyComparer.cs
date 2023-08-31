using AuditsAPI.Dtos;
using System.Reflection;

namespace AuditsAPI.Helpers
{
    public static class PropertyComparer
    {
        public static List<Dictionary<string, Tuple<object, object>>> CompareProperties(List<PropertyAuditDto> objects)
        {
            List<Dictionary<string, Tuple<object, object>>> diffPropertiesList = new List<Dictionary<string, Tuple<object, object>>>();

            if (objects == null || objects.Count < 2)
            {
                throw new ArgumentException("List must contain at least two objects for comparison.");
            }

            Type type = objects[0].GetType();

            PropertyInfo[] properties = type.GetProperties();

            foreach (object obj in objects)
            {
                Dictionary<string, Tuple<object, object>> diffProperties = new Dictionary<string, Tuple<object, object>>();

                foreach (PropertyInfo property in properties)
                {
                    object value1 = property.GetValue(objects[0])!;
                    object value2 = property.GetValue(obj)!;

                    if (!Equals(value1, value2))
                    {
                        diffProperties[property.Name] = Tuple.Create(value1!, value2!);
                    }
                }

                diffPropertiesList.Add(diffProperties);
            }

            return diffPropertiesList;
        }
    }
}
