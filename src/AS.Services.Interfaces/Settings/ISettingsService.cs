using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for settings service that Creates/Updates/Deletes  settings definitions and values
    /// </summary>
    public interface ISettingsService : IService
    {
        //Setting Definition Methods
        void AddSettingDefinition(SettingDefinition SettingDefinition);

        void UpdateSettingDefinition(SettingDefinition SettingDefinition);

        SettingDefinition GetSettingDefinitionById(int id);

        SettingDefinition GetSettingDefinitionByName(string name);

        void DeleteSettingDefinitionById(int id);

        void DeleteSettingDefinitionByName(string name);

        IList<SettingDefinition> SelectAllSettingDefinitions();

        //Setting Value Methods
        void AddSettingValue(SettingValue SettingValue);

        void UpdateSettingValue(SettingValue SettingValue);

        void DeleteSettingValueById(int id);

        SettingValue GetSettingValueById(int id);

        IList<SettingValue> SelectSettingValues(string ordering, int SettingDefinitionId);
    }
}