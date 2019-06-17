using PluginHub.Configuration;
using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace PluginHub.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class EngineContext
    {
        #region Utilities

        /// <summary>
        /// 创建引擎实例，若配置了自定义引擎，则反射创建自定义引擎实例，否则创建默认引擎<see cref="PluginHubEngine"/>实例
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>New engine instance</returns>
        protected static IEngine CreateEngineInstance(PluginHubConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                var engineType = Type.GetType(config.EngineType);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + config.EngineType + "' could not be found. Please check the configuration at /configuration/nop/engine[@engineType] or check for missing assemblies.");
                if (!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'Nop.Core.Infrastructure.IEngine' and cannot be configured in /configuration/nop/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }

            return new PluginHubEngine();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            // 若存储的引擎单例为 null 或 强制重新创建
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                // 通过 ConfigurationManager.GetSection("NopConfig")  获取的配置实例 除非 web.config 更新，否则单例，始终返回的同一实例
                var config = ConfigurationManager.GetSection("NopConfig") as PluginHubConfig;
                Singleton<IEngine>.Instance = CreateEngineInstance(config);
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// 替换当前引擎单例 为 自定义引擎
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
