using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;
using MvcContrib.Pagination;

namespace MAT.Web.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    string id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers 
                    MvcHtmlString label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    string radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id }).ToHtmlString();

                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"RadioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString Span(this HtmlHelper helper, string text, object htmlAttributes)
        {
            var span = new TagBuilder("span");
            span.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            span.SetInnerText(text);
            return MvcHtmlString.Create(span.ToString());
        }

        public static IHtmlString RawJson(this HtmlHelper helper, object data)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return helper.Raw(json);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes, string spanText)
        {
            var htmlAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributesDict);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            TagBuilder span = new TagBuilder("strong");
            span.SetInnerText(spanText);
            tag.InnerHtml = string.Format(labelText, span.ToString(TagRenderMode.Normal));
            
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <remarks>
        /// Source: MvcContrib Pager.CreateDefaultUrl and Pager.CreatePageLink
        /// </remarks>
        public static MvcHtmlString PageLink(this HtmlHelper html, int pageNumber, string linkText, string pageQueryName = "page")
        {
            var routeValues = new RouteValueDictionary();
            
            foreach (var key in html.ViewContext.HttpContext.Request.QueryString.AllKeys.Where(key => key != null))
            {
                routeValues[key] = html.ViewContext.RequestContext.HttpContext.Request.QueryString[key];
            }
            
            routeValues[pageQueryName] = pageNumber;

            var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, html.ViewContext.RequestContext, true);
            var tag = new TagBuilder("a");
            tag.SetInnerText(linkText);
            tag.MergeAttribute("href", url);
            var link = tag.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(link);
        }
    }    
}