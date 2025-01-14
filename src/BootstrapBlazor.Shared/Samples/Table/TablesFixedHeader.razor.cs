﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Shared.Samples.Table;

/// <summary>
/// Table 组件固定表头示例
/// </summary>
public partial class TablesFixedHeader
{
    [NotNull]
    private List<Foo>? Items { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? FooLocalizer { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TablesFixedHeader>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private DialogService? DialogService { get; set; }

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = Foo.GenerateFoo(FooLocalizer);
    }

    private async Task OnClickDialog()
    {
        var op = new DialogOption
        {
            Class = "dialog-table",
            Title = Localizer["DialogTitle"],
            BodyTemplate = RenderTable()
        };

        await DialogService.Show(op);
    }
}
