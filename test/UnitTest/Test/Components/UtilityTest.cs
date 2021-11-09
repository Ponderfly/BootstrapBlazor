﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Xunit;

namespace UnitTest.Components
{
    public class UtilityTest
    {
        [Fact]
        public void GetDisplayName_Ok()
        {
            using var ctx = new BlazorTestContext();
            var localizer = ctx.Services.GetRequiredService<IStringLocalizer<Foo>>();

            ctx.RenderComponent<Alert>();

            var fooData = new DataTable();
            fooData.Columns.Add(new DataColumn(nameof(Foo.DateTime), typeof(DateTime)) { DefaultValue = DateTime.Now });
            fooData.Columns.Add(nameof(Foo.Name), typeof(string));
            fooData.Columns.Add(nameof(Foo.Complete), typeof(bool));
            fooData.Columns.Add(nameof(Foo.Education), typeof(string));
            fooData.Columns.Add(nameof(Foo.Count), typeof(int));
            Foo.GenerateFoo(localizer, 10).ForEach(f =>
            {
                fooData.Rows.Add(f.DateTime, f.Name, f.Complete, f.Education, f.Count);
            });

            Assert.Equal("日期", localizer[nameof(Foo.DateTime)]);

            var context = new DataTableDynamicContext(fooData, (context, col) =>
            {
                var propertyName = col.GetFieldName();
                if (propertyName == nameof(Foo.DateTime))
                {
                    context.AddRequiredAttribute(nameof(Foo.DateTime));
                    // 使用 AutoGenerateColumnAttribute 设置显示名称示例
                    context.AddAutoGenerateColumnAttribute(nameof(Foo.DateTime), new KeyValuePair<string, object?>[] {
                        new(nameof(AutoGenerateColumnAttribute.Text), localizer[nameof(Foo.DateTime)].Value)
                    });
                }
                else if (propertyName == nameof(Foo.Name))
                {
                    context.AddRequiredAttribute(nameof(Foo.Name), localizer["Name.Required"].Value);
                    // 使用 Text 设置显示名称示例
                    col.Text = localizer[nameof(Foo.Name)];
                }
                else if (propertyName == nameof(Foo.Count))
                {
                    context.AddRequiredAttribute(nameof(Foo.Count));
                    // 使用 DisplayNameAttribute 设置显示名称示例
                    context.AddDisplayNameAttribute(nameof(Foo.Count), localizer[nameof(Foo.Count)].Value);
                }
                else if (propertyName == nameof(Foo.Complete))
                {
                    col.Filterable = true;
                    // 使用 DisplayAttribute 设置显示名称示例
                    context.AddDisplayAttribute(nameof(Foo.Complete), new KeyValuePair<string, object?>[] {
                        new(nameof(DisplayAttribute.Name), localizer[nameof(Foo.Complete)].Value)
                    });
                }
            });

            // 静态类
            var dn = Utility.GetDisplayName(typeof(Foo), nameof(Foo.Count));
            Assert.Equal("数量", dn);

            // 动态类
            dn = Utility.GetDisplayName(context.GetItems().First(), nameof(Foo.Count));
            Assert.Equal("数量", dn);
        }

        [Fact]
        public void GetPropertyValue_Ok()
        {
            using var ctx = new BlazorTestContext();
            var Localizer = ctx.Services.GetRequiredService<IStringLocalizer<Foo>>();

            var foo = Foo.Generate(Localizer);

            var v1 = Utility.GetPropertyValue<Foo, string>(foo, nameof(Foo.Name));
            Assert.Contains("张三", v1);

            var v2 = Utility.GetPropertyValue<object, object>(foo, nameof(Foo.Name));
            Assert.Contains("张三", v2.ToString());

            var v3 = Utility.GetPropertyValue(foo, nameof(Foo.Name));
            Assert.Contains("张三", v3?.ToString());
        }

        [Fact]
        public void SetPropertyValue_Ok()
        {
            using var ctx = new BlazorTestContext();
            var Localizer = ctx.Services.GetRequiredService<IStringLocalizer<Foo>>();

            var foo = Foo.Generate(Localizer);
            var v1 = "张三";
            var val = "李四";
            Utility.SetPropertyValue<Foo, string>(foo, nameof(Foo.Name), val);
            Assert.Equal(foo.Name, val);

            foo.Name = v1;
            Utility.SetPropertyValue<Foo, object>(foo, nameof(Foo.Name), val);
            Assert.Equal(foo.Name, val);

            foo.Name = v1;
            Utility.SetPropertyValue<object, string>(foo, nameof(Foo.Name), val);
            Assert.Equal(foo.Name, val);

            foo.Name = v1;
            Utility.SetPropertyValue<object, object>(foo, nameof(Foo.Name), val);
            Assert.Equal(foo.Name, val);
        }

        [Fact]
        public void TryGetProperty_Ok()
        {
            var condition = Utility.TryGetProperty(typeof(Foo), nameof(Foo.Name), out _);
            Assert.True(condition);

            condition = Utility.TryGetProperty(typeof(Foo), "Test1", out _);
            Assert.False(condition);
        }
    }
}
