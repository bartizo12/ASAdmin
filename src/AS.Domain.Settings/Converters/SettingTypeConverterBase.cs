using AS.Domain.Entities;
using System;
using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Base type converter class for all settings that converts from <typeparamref name="SettingValue"/> entity to related Setting structure
    /// </summary>
    public abstract class SettingTypeConverterBase : TypeConverter
    {
        private const int TotalFieldCount = 15;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(SettingValue))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        protected T GetFieldValue<T>(SettingValue value, string fieldName)
        {
            T fieldValue = default(T);

            for (int i = 1; i <= TotalFieldCount; i++)
            {
                string pFieldName = (string)value.SettingDefinition.GetType()
                    .GetProperty("Field" + i.ToString()).GetValue(value.SettingDefinition, null);

                if (!string.IsNullOrEmpty(pFieldName) &&
                   pFieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                {
                    fieldValue = (T)Convert.ChangeType(value.GetType().GetProperty("Field" + i.ToString()).GetValue(value, null), typeof(T));
                    break;
                }
            }

            return fieldValue;
        }
    }
}