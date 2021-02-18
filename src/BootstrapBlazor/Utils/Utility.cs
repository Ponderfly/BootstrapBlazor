﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Localization.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utility
    {
        private static ConcurrentDictionary<(string CultureInfoName, Type ModelType, string FieldName), string> DisplayNameCache { get; } = new ConcurrentDictionary<(string, Type, string), string>();
        private static ConcurrentDictionary<(Type ModelType, string FieldName), PropertyInfo> PropertyInfoCache { get; } = new ConcurrentDictionary<(Type, string), PropertyInfo>();
        private static ConcurrentDictionary<(Type ModelType, string FieldName), string> PlaceHolderCache { get; } = new ConcurrentDictionary<(Type, string), string>();

        /// <summary>
        /// 获取显示名称方法
        /// </summary>
        /// <param name="model">模型实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetDisplayName(object model, string fieldName) => GetDisplayName(model.GetType(), fieldName);

        /// <summary>
        /// 获取显示名称方法
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetDisplayName(Type modelType, string fieldName)
        {
            var cacheKey = (CultureInfoName: CultureInfo.CurrentUICulture.Name, Type: modelType, FieldName: fieldName);
            if (!DisplayNameCache.TryGetValue(cacheKey, out var dn))
            {
                // 显示名称为空时通过资源文件查找 FieldName 项
                var localizer = JsonStringLocalizerFactory.CreateLocalizer(cacheKey.Type);
                var stringLocalizer = localizer[fieldName];
                if (!stringLocalizer.ResourceNotFound)
                {
                    dn = stringLocalizer.Value;
                }
                else if (TryGetProperty(cacheKey.Type, cacheKey.FieldName, out var propertyInfo))
                {
                    // 回退查找 Display 标签
                    dn = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name
                        ?? propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

                    // 回退查找资源文件通过 dn 查找匹配项 用于支持 Validation
                    if (!string.IsNullOrEmpty(dn))
                    {
                        var resxType = ServiceProviderHelper.ServiceProvider.GetRequiredService<IOptions<JsonLocalizationOptions>>();
                        if (resxType.Value.ResourceManagerStringLocalizerType != null)
                        {
                            localizer = JsonStringLocalizerFactory.CreateLocalizer(resxType.Value.ResourceManagerStringLocalizerType);
                            stringLocalizer = localizer[dn];
                            if (!stringLocalizer.ResourceNotFound)
                            {
                                dn = stringLocalizer.Value;
                            }
                        }
                    }
                }

                // add display name into cache
                if (!string.IsNullOrEmpty(dn))
                {
                    DisplayNameCache.GetOrAdd(cacheKey, key => dn);
                }
            }
            return dn ?? cacheKey.FieldName;
        }

        /// <summary>
        /// 获取 PlaceHolder 方法
        /// </summary>
        /// <param name="model">模型实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string? GetPlaceHolder(object model, string fieldName) => GetPlaceHolder(model.GetType(), fieldName);

        /// <summary>
        /// 获取 PlaceHolder 方法
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string? GetPlaceHolder(Type modelType, string fieldName)
        {
            var cacheKey = (Type: modelType, FieldName: fieldName);
            if (!PlaceHolderCache.TryGetValue(cacheKey, out var placeHolder))
            {
                // 通过资源文件查找 FieldName 项
                var localizer = JsonStringLocalizerFactory.CreateLocalizer(cacheKey.Type);
                var stringLocalizer = localizer[$"{fieldName}.PlaceHolder"];
                if (!stringLocalizer.ResourceNotFound)
                {
                    placeHolder = stringLocalizer.Value;
                }
                else if (Utility.TryGetProperty(cacheKey.Type, cacheKey.FieldName, out var propertyInfo))
                {
                    var placeHolderAttribute = propertyInfo.GetCustomAttribute<PlaceHolderAttribute>();
                    if (placeHolderAttribute != null)
                    {
                        placeHolder = placeHolderAttribute.Text;
                    }
                    if (!string.IsNullOrEmpty(placeHolder))
                    {
                        // add display name into cache
                        PlaceHolderCache.GetOrAdd(cacheKey, key => placeHolder);
                    }
                }
            }
            return placeHolder;
        }

        private static bool TryGetProperty(Type modelType, string fieldName, [NotNullWhen(true)] out PropertyInfo? propertyInfo)
        {
            var cacheKey = (ModelType: modelType, FieldName: fieldName);
            if (!PropertyInfoCache.TryGetValue(cacheKey, out propertyInfo))
            {
                // Validator.TryValidateProperty 只能对 Public 属性生效
                propertyInfo = cacheKey.ModelType.GetProperty(cacheKey.FieldName);

                if (propertyInfo != null) PropertyInfoCache[cacheKey] = propertyInfo;
            }
            return propertyInfo != null;
        }
    }
}
