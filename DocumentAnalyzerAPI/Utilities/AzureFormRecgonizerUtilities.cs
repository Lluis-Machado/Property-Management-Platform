using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.Utilities
{
    public static class AzureFormRecgonizerUtilities
    {

        public static T? MapFieldValue<T>(IReadOnlyDictionary<string, DocumentField> documentFields, string fieldName)
        {
            if (!documentFields.TryGetValue(fieldName, out DocumentField documentField)) return default(T);

            return GetFieldValue<T>(documentField);
        }

        private static T GetFieldValue<T>(DocumentField documentField)
        {
            switch (documentField.FieldType)
            {
                case DocumentFieldType.String:
                    return documentField.Value.AsString() is T strValue ? strValue : default;
                case DocumentFieldType.Date:
                    return documentField.Value.AsDate() is T dateValue ? dateValue : default;
                case DocumentFieldType.Double:
                    return documentField.Value.AsDouble() is T doubleValue ? doubleValue : default;
                case DocumentFieldType.Currency:
                    if (documentField.Value.AsCurrency().Amount is T currencyValue)
                    {
                        return currencyValue;
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        // Resturn currency symbol
                        return (T)(object)documentField.Value.AsCurrency().Symbol;
                    }
                    else
                    {
                        return default;
                    }
                default:
                    return default;
            }
        }
    }
}
