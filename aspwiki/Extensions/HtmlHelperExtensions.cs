using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace ASPWiki
{
    //http://asp.net-hacker.rocks/2016/02/18/extending-razor-views.html
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Allows you to add scripts from partial view to the end of DOM.
        /// </summary>
        /// <param name="htmlHelper">@Html</param>
        /// <param name="template">@&lt;script&gt;Your script &lt;/script&gt;</param>
        /// <returns>-</returns>
        public static HtmlString ScriptSection(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return HtmlString.Empty;
        }

        /// <summary>
        /// Renders all scripts from SciprtSection() to the end of DOM.
        /// </summary>
        /// <param name="htmlHelper">@Html</param>
        /// <returns>-</returns>
        public static HtmlString RenderScriptsSection(this IHtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return HtmlString.Empty;
        }
    }
}
