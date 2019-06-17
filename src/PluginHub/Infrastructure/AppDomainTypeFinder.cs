using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PluginHub.Infrastructure
{
    /// <summary>
    /// A class that finds types needed by Nop by looping assemblies in the 
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Fields

        private bool ignoreReflectionErrors = true;
        private bool loadAppDomainAssemblies = true;
        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();

        #endregion

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>Gets or sets wether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns are applied when loading these assemblies.</summary>
        public bool LoadAppDomainAssemblies
        {
            get { return loadAppDomainAssemblies; }
            set { loadAppDomainAssemblies = value; }
        }

        /// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames
        {
            get { return assemblyNames; }
            set { assemblyNames = value; }
        }

        /// <summary>
        /// 黑名单：不需要（再次）加载的程序集
        /// Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        /// <summary>
        /// 白名单，需要加载的程序集
        /// Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies arn't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return assemblyRestrictToLoadingPattern; }
            set { assemblyRestrictToLoadingPattern = value; }
        }

        #endregion

        #region Methods

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// 在指定的程序集列表中 找类型（能够实现指定类型<see cref="assignTypeFrom"/>或继承指定类型的类）
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                // 遍历程序集列表
                foreach (var a in assemblies)
                {
                    //  该程序集 内 所有的 类型
                    Type[] types = null;
                    try
                    {
                        types = a.GetTypes();
                    }
                    catch
                    {
                        //Entity Framework 6 doesn't allow getting types (throws an exception)
                        if (!ignoreReflectionErrors)
                        {
                            throw;
                        }
                    }
                    if (types != null)
                    {
                        // 遍历当前程序集中的 所有类型
                        foreach (var t in types)
                        {
                            // 1.若当前类型的实例可以 替换 指定类型的实例（里氏替换） / 实现接口
                            // 2.指定类型是一个普通类 => true 且 当前类型 继承于 指定类型 => true
                            // 1,2 都 满足才满足
                            if (assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                            {
                                // 当前类型非接口
                                if (!t.IsInterface)
                                {
                                    // 是否只要实现类（非抽象）
                                    if (onlyConcreteClasses)
                                    {
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            result.Add(t);
                                        }
                                    }
                                    else
                                    {
                                        result.Add(t);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        /// <summary>
        /// 返回 所有需要加载的程序集
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies that should be loaded by the Nop factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// 将 app已加载程序集列表 中所有 不在黑名单 且 在白名单的 程序集 添加到 实参
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// 将 <see cref="AssemblyNames"/> 中的 所有程序集 添加到 实参
        /// Adds specificly configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string assemblyName in AssemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        /// 检查一个程序集 是否需要被加载,需要返回True
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The name of the assembly to check.
        /// </param>
        /// <returns>
        /// True if the assembly should be loaded into Nop.
        /// </returns>
        public virtual bool Matches(string assemblyFullName)
        {
            // 不在黑名单内 且 在白名单内的 程序集
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// 检查一个程序集 是否 需要被 加载，不需要 返回 True
        /// 满足 pattern 则 不需要被加载
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly name to match.
        /// </param>
        /// <param name="pattern">
        /// The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        /// True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// (指定目录)确保将需要加载的程序集 加载 到 App
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">
        /// The physical path to a directory containing dlls to load in the app domain.
        /// </param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            // 当前 App 内 所有 已加载的程序集
            var loadedAssemblyNames = new List<string>();
            foreach (Assembly a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }
            // 该目录下所有 dll
            foreach (string dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    // 该程序集 需要被加载 且 当前已加载程序集中不存在它
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        // 将它加载到 App
                        App.Load(an);
                    }

                    //old loading stuff
                    //Assembly a = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    //if (Matches(a.FullName) && !loadedAssemblyNames.Contains(a.FullName))
                    //{
                    //    App.Load(a.FullName);
                    //}
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Does type implement generic?
        /// type 类型 是否 继承 openGeneric 类型（非实现接口）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                // type 类型 继承的类 和 实现的接口
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    // 实现的 不是 普通类 跳过
                    if (!implementedInterface.IsGenericType)
                        continue;
                    // 此类型的实例 是否 可以被 分配给 当前 openGeneric 类型
                    // type 类型 是否 继承 openGeneric 类型
                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }catch
            {
                return false;
            }
        }

        #endregion
    }
}
