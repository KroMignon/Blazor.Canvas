﻿using Excubo.Blazor.Canvas.Extensions;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Excubo.Blazor.Canvas.Contexts
{
    public class Context
    {
        protected readonly string ctx;
        private readonly IJSRuntime js;
        public Context(string ctx, IJSRuntime js)
        {
            this.ctx = ctx;
            this.js = js;
        }
        protected ValueTask InvokeAsync(string method_name, params object[] parameters) => js.InvokeVoidAsync(ctx + "." + method_name, parameters);
        protected ValueTask<T> InvokeAsync<T>(string method_name, params object[] parameters) => js.InvokeAsync<T>(ctx + "." + method_name, parameters);
        protected ValueTask InvokeEvalAsync(string field, string calculation) => js.InvokeVoidAsync("eval", $"{ctx}.{field} = {calculation}");
        protected ValueTask SetAsync(string field, string value) => InvokeEvalAsync(field, "\"" + value + "\"");
        protected ValueTask SetAsync(string field, bool value) => SetAsync(field, value.ToString());
        protected ValueTask SetAsync(string field, double value) => InvokeEvalAsync(field, value.ToInvariantString());
        [Obsolete("obj parameter type is not meant for long-term. Replace ASAP")]
        protected ValueTask SetAsync(string field, object value) => InvokeEvalAsync(field, JsonSerializer.Serialize(value));
        protected ValueTask SetAsync<TEnum>(string field, TEnum value) where TEnum : Enum => SetAsync(field, value.ToJsEnumValue());
        protected ValueTask<string> GetStringAsync(string field) => js.InvokeAsync<string>("eval", ctx + "." + field);
        protected ValueTask<bool> GetBoolAsync(string field) => js.InvokeAsync<bool>("eval", ctx + "." + field);
        protected ValueTask<double> GetDoubleAsync(string field) => js.InvokeAsync<double>("eval", ctx + "." + field);
        [Obsolete("obj parameter type is not meant for long-term. Replace ASAP")]
        protected ValueTask<object> GetObjectAsync(string field) => js.InvokeAsync<object>("eval", ctx + "." + field);
        protected async ValueTask<TEnum> GetAsync<TEnum>(string field) where TEnum : Enum
        {
            var value = await GetStringAsync(field);
            var match = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().FirstOrDefault(v => v.ToJsEnumValue() == value);
            return match == null ? default : match;

        }
    }
}