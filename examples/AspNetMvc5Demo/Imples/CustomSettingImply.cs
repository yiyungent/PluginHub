using PluginHub.Configuration;
using PluginHub.Domain.Configuration;
using PluginHub.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AspNetMvc5Demo.Imples
{
    public class CustomSettingImply : ISettingService
    {
        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting<T>() where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public IList<Setting> GetAllSettings()
        {
            throw new NotImplementedException();
        }

        public Setting GetSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            throw new NotImplementedException();
        }

        public Setting GetSettingById(int settingId)
        {
            throw new NotImplementedException();
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            throw new NotImplementedException();
        }

        public T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }
    }
}