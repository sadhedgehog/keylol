﻿using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using SimpleInjector.Integration.Owin;

namespace Keylol.StateTreeManager
{
    /// <summary>
    /// 提供一些帮助方法
    /// </summary>
    public static class StateTreeHelper
    {
        /// <summary>
        /// 判断当前登录用户是否有权访问指定属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>如果有权访问，返回 <c>true</c></returns>
        /// <exception cref="ArgumentException">无法获取 <paramref name="propertyName"/> 指定的属性</exception>
        public static async Task<bool> CanAccessAsync<T>(string propertyName)
        {
            var owinContext = Startup.Container.GetInstance<OwinContextProvider>().Current;
            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Invalid property name.", nameof(propertyName));
            if (property.GetCustomAttribute<AllowAnonymousAttribute>() != null)
                return true;
            foreach (var authorizeAttribute in property.GetCustomAttributes<AuthorizeAttribute>())
            {
                if (!await authorizeAttribute.AuthorizeAsync(owinContext))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 从容器获取一个 Service 实例
        /// </summary>
        /// <typeparam name="T">Service 类型</typeparam>
        /// <returns>Service 实例</returns>
        public static T GetService<T>() where T : class
        {
            return Startup.Container.GetInstance<T>();
        }

        /// <summary>
        /// 从容器获取一个 Service 实例
        /// </summary>
        /// <param name="serviceType">Service 类型</param>
        /// <returns>Service 实例</returns>
        public static object GetService(Type serviceType)
        {
            return Startup.Container.GetInstance(serviceType);
        }

        /// <summary>
        /// 获取当前登录的用户
        /// </summary>
        /// <returns>当前用户 Principal</returns>
        public static IPrincipal CurrentUser() => GetService<OwinContextProvider>().Current.Request.User;
    }
}