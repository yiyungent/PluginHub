using Autofac;
using Autofac.Integration.Mvc;
using AspNetMvc5Demo;
using AspNetMvc5Demo.Fakes;
using AspNetMvc5Demo.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetMvc5Demo
{
    public class AutofacConfig
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise()
        {
            var builder = RegisterService();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }

        /// <summary>
        /// 注入实现
        /// </summary>
        /// <returns></returns>
        private static ContainerBuilder RegisterService()
        {
            var builder = new ContainerBuilder();

            

            return builder;
        }
    }
}
