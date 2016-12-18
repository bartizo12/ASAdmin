namespace AS.Domain.Entities
{
    public static class SettingValueExtensions
    {
        private static readonly int FieldCount = 15;

        public static void Set(this SettingValue settingValue, SettingDefinition settingDef, string fieldName, string value)
        {
            for (int i = 1; i <= FieldCount; i++)
            {
                string name = (string)settingDef.GetType().GetProperty("Field" + i.ToString()).GetValue(settingDef, null);

                if (fieldName == name)
                {
                    settingValue.GetType().GetProperty("Field" + i.ToString()).SetValue(settingValue, value);
                    return;
                }
            }
        }

        public static string Get(this SettingValue settingValue, SettingDefinition settingDef, string fieldName)
        {
            for (int i = 1; i <= FieldCount; i++)
            {
                string name = (string)settingDef.GetType().GetProperty("Field" + i.ToString()).GetValue(settingDef, null);

                if (fieldName == name)
                {
                    return (string)settingValue.GetType().GetProperty("Field" + i.ToString()).GetValue(settingValue);
                }
            }
            return null;
        }
    }
}