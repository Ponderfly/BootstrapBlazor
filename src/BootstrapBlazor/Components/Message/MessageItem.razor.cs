﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// MessageItem 组件
    /// </summary>
    public sealed partial class MessageItem
    {
        private ElementReference MessageItemElement { get; set; }

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        /// <returns></returns>
        protected override string? ClassName => CssBuilder.Default("alert")
            .AddClass($"alert-{Color.ToDescriptionString()}", Color != Color.None)
            .AddClass("is-bar", ShowBar)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得/设置 Toast Body 子组件
        /// </summary>
        [Parameter]
        public string? Content { get; set; }

        /// <summary>
        /// 获得/设置 是否自动隐藏
        /// </summary>
        [Parameter]
        public bool IsAutoHide { get; set; } = true;

        /// <summary>
        /// 获得/设置 自动隐藏时间间隔
        /// </summary>
        [Parameter]
        public int Delay { get; set; } = 4000;

        /// <summary>
        /// 获得/设置 Message 实例
        /// </summary>
        /// <value></value>
        [CascadingParameter] public MessageBase? Message { get; set; }

        private JSInterop<MessageBase>? _interop;

        /// <summary>
        /// OnAfterRender 方法
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                if (JSRuntime != null && Message != null)
                {
                    _interop = new JSInterop<MessageBase>(JSRuntime);
                    _interop.Invoke(Message, MessageItemElement, "showMessage", nameof(MessageBase.Clear));
                }
            }
        }
    }
}
