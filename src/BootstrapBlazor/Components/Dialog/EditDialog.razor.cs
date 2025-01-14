﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Components;

/// <summary>
/// 编辑弹窗组件
/// </summary>
[JSModuleAutoLoader("edit-dialog")]
public partial class EditDialog<TModel>
{
    /// <summary>
    /// 获得/设置 保存回调委托
    /// </summary>
    [Parameter]
    [NotNull]
    public Func<EditContext, Task>? OnSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 获得/设置 重置按钮文本
    /// </summary>
    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    /// <summary>
    /// 获得/设置 查询时是否显示正在加载中动画 默认为 false
    /// </summary>
    [Parameter]
    public bool ShowLoading { get; set; }

    /// <summary>
    /// 获得/设置 组件是否采用 Tracking 模式对编辑项进行直接更新 默认 false
    /// </summary>
    [Parameter]
    public bool IsTracking { get; set; }

    /// <summary>
    /// 获得/设置 实体类编辑模式 Add 还是 Update
    /// </summary>
    [Parameter]
    public ItemChangedType ItemChangedType { get; set; }

    /// <summary>
    /// 获得/设置 查询按钮文本
    /// </summary>
    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    /// <summary>
    /// 获得/设置 关闭弹窗回调方法
    /// </summary>
    [Parameter]
    public Func<Task>? OnCloseAsync { get; set; }

    /// <summary>
    /// 获得/设置 是否禁用表单内回车自动提交功能 默认 null 未设置
    /// </summary>
    [Parameter]
    public bool? DisableAutoSubmitFormByEnter { get; set; }

    /// <summary>
    /// 获得/设置 DialogFooterTemplate 实例
    /// </summary>
    [Parameter]
    public RenderFragment<TModel>? FooterTemplate { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<EditDialog<TModel>>? Localizer { get; set; }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        SaveButtonText ??= Localizer[nameof(SaveButtonText)];
    }

    private async Task OnClickClose()
    {
        if (OnCloseAsync != null) await OnCloseAsync();
    }

    private async Task OnValidSubmitAsync(EditContext context)
    {
        if (OnSaveAsync != null)
        {
            await ToggleLoading(true);
            await OnSaveAsync(context);
            await ToggleLoading(false);
        }
    }

    /// <summary>
    /// 显示/隐藏 Loading 遮罩
    /// </summary>
    /// <param name="state">true 时显示，false 时隐藏</param>
    /// <returns></returns>
    public async ValueTask ToggleLoading(bool state)
    {
        if (ShowLoading)
        {
            await InvokeExecuteAsync(Id, state);
        }
    }

    private RenderFragment RenderFooter => builder =>
    {
        if (FooterTemplate != null)
        {
            builder.OpenComponent<CascadingValue<Func<Task>?>>(0);
            builder.AddAttribute(1, nameof(CascadingValue<Func<Task>?>.Value), OnCloseAsync);
            builder.AddAttribute(2, nameof(CascadingValue<Func<Task>?>.IsFixed), true);
            builder.AddAttribute(3, nameof(CascadingValue<Func<Task>?>.ChildContent), FooterTemplate(Model));
            builder.CloseComponent();
        }
        else
        {
            if (!IsTracking)
            {
                builder.OpenComponent<Button>(20);
                builder.AddAttribute(21, nameof(Button.Color), Color.Secondary);
                builder.AddAttribute(22, nameof(Button.Icon), "fa-solid fa-xmark");
                builder.AddAttribute(23, nameof(Button.Text), CloseButtonText);
                builder.AddAttribute(24, nameof(Button.OnClickWithoutRender), OnClickClose);
                builder.CloseComponent();
            }
            builder.OpenComponent<Button>(30);
            builder.AddAttribute(31, nameof(Button.Color), Color.Primary);
            builder.AddAttribute(32, nameof(Button.Icon), "fa-solid fa-floppy-disk");
            builder.AddAttribute(33, nameof(Button.Text), SaveButtonText);
            builder.AddAttribute(34, nameof(Button.ButtonType), ButtonType.Submit);
            builder.CloseComponent();
        }
    };
}
