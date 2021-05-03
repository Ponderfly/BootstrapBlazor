﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// MenuLink 组件内部封装 NavLink 组件
    /// </summary>
    public sealed partial class MenuLink
    {
        private string? ClassString => CssBuilder.Default("nav-link")
            .AddClass("active", Parent.DisableNavigation && Item.IsActive && !Item.IsDisabled)
            .AddClass("disabled", Item.IsDisabled)
            .AddClass("expand", Parent.IsVertical && !Item.IsCollapsed)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        private string? GetMenuArrowClassString => CssBuilder.Default("arrow")
            .AddClass("fa fa-fw", Parent != null && Parent.IsVertical)
            .AddClass("fa-angle-left", Item.Items.Any())
            .Build();

        private string? GetHrefString => (Parent.DisableNavigation || Item.IsDisabled) ? null : (Item.Items.Any() ? "#" : Item.Url?.TrimStart('/'));

        /// <summary>
        /// 获得/设置 MenuItem 实例 不可为空
        /// </summary>
        [Parameter]
        [NotNull]
        public MenuItem? Item { get; set; }

        /// <summary>
        /// 获得/设置 点击菜单回调委托方法
        /// </summary>
        [Parameter]
        public Func<MenuItem, Task>? OnClick { get; set; }

        [CascadingParameter]
        [NotNull]
        private Menu? Parent { get; set; }

        private async Task OnClickLink()
        {
            if (OnClick != null) await OnClick(Item);
        }

        private NavLinkMatch GetMatch() => string.IsNullOrEmpty(Item.Url) ? NavLinkMatch.All : Item.Match;

        private string? GetIconString => string.IsNullOrEmpty(Item.Icon)
            ? (Parent.IsVertical
                ? (Parent.IsCollapsed
                    ? "fa-none"
                    : "fa fa-fw")
                : null)
            : Item.Icon.Contains("fa-fw", StringComparison.OrdinalIgnoreCase)
                ? Item.Icon
                : $"{Item.Icon} fa-fw";

        private string? GetStyleClassString => (Parent.IsVertical && !Parent.IsCollapsed)
            ? (Item.Indent == 0
                ? null
                : $"padding-left: {Item.Indent * Parent.IndentSize}px;")
            : null;
    }
}
