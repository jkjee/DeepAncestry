using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DeependAncestry.Helpers
{
    public static class CheckboxHelper
    {
        public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            string checkBoxWithHidden = htmlHelper.CheckBox(name, htmlAttributes).ToHtmlString().Trim();
            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1, StringComparison.Ordinal));
            return new MvcHtmlString(pureCheckBox);
        }
    }
}