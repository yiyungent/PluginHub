using PluginHub.Configuration;
using PluginHub.Domain.Configuration;
using PluginHub.Services.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public void DeleteSetting<T, TPropType>(T settings) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public IList<Setting> GetAllSettings()
        {
            throw new NotImplementedException();
        }

        public Setting GetSetting(string key)
        {
            throw new NotImplementedException();
        }

        public Setting GetSettingById(int settingId)
        {
            throw new NotImplementedException();
        }

        public T GetSettingByKey<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            // 提取插件设置名作为 插件表名
            string pluginTableName = typeof(T).Name;
            // 简化--直接使用 txt 文件模拟数据库表
            string filePath = HttpContext.Current.Server.MapPath("~/Tables/" + pluginTableName + ".txt");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            string[] contents = File.ReadAllLines(filePath);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var line in contents)
            {
                string[] lineKeyVal = line.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                keyValuePairs.Add(lineKeyVal[0], lineKeyVal[1]);
            }

            T rtnObj = Activator.CreateInstance<T>();
            // 获取该类的所有属性
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                property.SetValue(rtnObj, keyValuePairs[property.Name]);
            }

            return rtnObj;
        }

        public void SaveSetting<T>(T settings) where T : ISettings, new()
        {
            // 提取插件设置名作为 插件表名
            string pluginTableName = typeof(T).Name;
            // 简化--直接使用 txt 文件模拟数据库表
            string filePath = HttpContext.Current.Server.MapPath("~/Tables/" + pluginTableName + ".txt");
            IList<string> contents = new List<string>();
            // 获取该类的所有属性
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                contents.Add(property.Name + ":" + property.GetValue(settings).ToString());
            }
            File.WriteAllLines(filePath, contents, System.Text.Encoding.UTF8);
        }

        public void SetSetting<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool SettingExists<T, TPropType>(T settings) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }
    }
}