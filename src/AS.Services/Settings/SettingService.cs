using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Data;
using AS.Infrastructure.Web;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace AS.Services
{
    /// <summary>
    /// Creates/Updates/Deletes  settings definitions and values
    /// </summary>
    public class SettingService : ISettingsService
    {
        private readonly IDbContext _dbContext;
        private readonly IStorageManager<Configuration> _configurationStorageManager;
        private readonly IEncryptionProvider _encryptionProvider;

        public SettingService(IDbContext dbContext,
            IStorageManager<Configuration> configurationStorageManager
            , IEncryptionProvider encryptionProvider)
        {
            this._dbContext = dbContext;
            this._configurationStorageManager = configurationStorageManager;
            this._encryptionProvider = encryptionProvider;
        }

        public void AddSettingDefinition(SettingDefinition SettingDefinition)
        {
            _dbContext.Set<SettingDefinition>().Add(SettingDefinition);
            _dbContext.SaveChanges();
        }

        public void AddSettingValue(SettingValue settingValue)
        {
            SettingDefinition settingDef = this.GetSettingDefinitionById(settingValue.SettingDefinitionID);
            settingValue = Encrypt(settingValue, settingDef);
            _dbContext.Set<SettingValue>().Add(settingValue);
            _dbContext.SaveChanges();
        }

        public void UpdateSettingDefinition(SettingDefinition settingDefinition)
        {
            _dbContext.Entry(settingDefinition).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void UpdateSettingValue(SettingValue settingValue)
        {
            SettingDefinition settingDef = this.GetSettingDefinitionById(settingValue.SettingDefinitionID);

            for (int i = 1; i <= 15; i++)
            {
                bool isRequired = (bool)settingDef.GetType().GetProperty("FieldRequired" + i.ToString()).GetValue(settingDef, null);
                string value = (string)settingValue.GetType().GetProperty("Field" + i.ToString()).GetValue(settingValue, null);

                if (isRequired && string.IsNullOrEmpty(value))
                    throw new ASException(ResMan.GetString("Setting_InvalidSettingValue"));
            }
            settingValue = Encrypt(settingValue, settingDef);
            _dbContext.Entry(settingValue).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public SettingDefinition GetSettingDefinitionById(int id)
        {
            return _dbContext.Set<SettingDefinition>()
                .SingleOrDefault(sdef => sdef.Id == id);
        }

        public SettingDefinition GetSettingDefinitionByName(string name)
        {
            return _dbContext.Set<SettingDefinition>()
                .SingleOrDefault(sdef => sdef.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DeleteSettingDefinitionById(int id)
        {
            this._dbContext.RemoveById<SettingDefinition, int>(id);
        }

        public void DeleteSettingValueById(int id)
        {
            this._dbContext.RemoveById<SettingValue, int>(id);
        }

        public SettingValue GetSettingValueById(int id)
        {
            SettingValue settingVal = _dbContext.Set<SettingValue>()
                                                .AsNoTracking()
                                                .Include("SettingDefinition")
                                                .SingleOrDefault(sVal => sVal.Id == id);

            return Decrypt(settingVal, settingVal.SettingDefinition);
        }

        public IList<SettingDefinition> SelectAllSettingDefinitions()
        {
            return _dbContext.Set<SettingDefinition>()
                .OrderBy(p => p.Name)
                .ToList();
        }

        public IList<SettingValue> SelectSettingValues(string ordering, int settingDefinitionId)
        {
            var query = _dbContext.Set<SettingValue>().AsNoTracking() as IQueryable<SettingValue>;
            query = query.Where(sv => sv.SettingDefinitionID == settingDefinitionId);

            if (!string.IsNullOrEmpty(ordering))
                query = query.OrderBy(ordering);

            SettingDefinition settingDef = this.GetSettingDefinitionById(settingDefinitionId);
            IList<SettingValue> list = query.ToList();
            list.AsParallel().ForAll((a) => a = Decrypt(a, settingDef));

            return query.ToList();
        }

        public void DeleteSettingDefinitionByName(string name)
        {
            _dbContext.Set<SettingDefinition>().RemoveRange(_dbContext.Set<SettingDefinition>().Where(sdef => sdef.Name == name));
        }

        /// <summary>
        /// Decrypts password fields.
        /// </summary>
        /// <param name="settingValue">Value</param>
        /// <param name="settingDefiniton">Definition</param>
        /// <returns>Decrypted setting value</returns>
        private SettingValue Decrypt(SettingValue settingValue, SettingDefinition settingDefiniton)
        {
            if (settingValue == null || settingDefiniton == null)
                return settingValue;

            for (int i = 1; i <= 15; i++)
            {
                FormInputType inputType = (FormInputType)settingDefiniton.GetType().GetProperty("FieldInputType" + i.ToString()).GetValue(settingDefiniton, null);

                if (inputType == FormInputType.Password)
                {
                    string value = (string)settingValue.GetType().GetProperty("Field" + i.ToString()).GetValue(settingValue, null);
                    value = _encryptionProvider.Decrypt(value, _configurationStorageManager.Read().First().SymmetricKey);
                    settingValue.GetType().GetProperty("Field" + i.ToString()).SetValue(settingValue, value);
                }
            }

            return settingValue;
        }

        /// <summary>
        /// Encrypts password fields of the setting value to store it securely
        /// </summary>
        /// <param name="settingValue">Value</param>
        /// <param name="settingDefiniton">Definition</param>
        /// <returns>Encrypted application setting value</returns>
        private SettingValue Encrypt(SettingValue settingValue, SettingDefinition settingDefiniton)
        {
            if (settingValue == null || settingDefiniton == null)
                return settingValue;

            for (int i = 1; i <= 15; i++)
            {
                FormInputType inputType = (FormInputType)settingDefiniton.GetType().GetProperty("FieldInputType" + i.ToString()).GetValue(settingDefiniton, null);

                if (inputType == FormInputType.Password)
                {
                    string value = (string)settingValue.GetType().GetProperty("Field" + i.ToString()).GetValue(settingValue, null);
                    value = _encryptionProvider.Encrypt(value, _configurationStorageManager.Read().First().SymmetricKey);
                    settingValue.GetType().GetProperty("Field" + i.ToString()).SetValue(settingValue, value);
                }
            }
            return settingValue;
        }
    }
}