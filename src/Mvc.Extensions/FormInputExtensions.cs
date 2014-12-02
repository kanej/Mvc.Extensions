using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Restful.Wiretypes;

namespace Mvc.Extensions
{
    public static class FormInputExtensions
    {
        public static string GetMemberName<T>(this HtmlHelper<T> htmlHelper, Expression<Func<T, object>> action)
        {
            return GetMemberInfo(action).Member.Name;
        }


        static MvcHtmlString BuildEditableFieldWithGroups<T>(this HtmlHelper<T> htmlHelper,
                                                          Expression<Func<T, object>> action)
        {
            var name = htmlHelper.GetMemberName(action).FriendlyName();
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model) as FieldWithGroupedOptions<string>;
            if (!field.Viewable) return new MvcHtmlString("");

            var type = GetTextBoxInputType(name);
            if (field.Options.Count > 0)
                return htmlHelper.BuildSelect(field.Options, name, "", action);
            return htmlHelper.BuildInput(!field.Editable, type, "", name, "", action, null);
        }

        public static MvcHtmlString BuildEditableField<T>(this HtmlHelper<T> htmlHelper,
                                                          Expression<Func<T, object>> action)
        {
            var name = htmlHelper.GetMemberName(action).FriendlyName();
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model) as Field<string>;
            if (field == null)
                return htmlHelper.BuildEditableFieldWithGroups(action);
            if (!field.Viewable) return new MvcHtmlString("");

            var type = GetTextBoxInputType(name);

            if (field.Options.Count > 0)
                return htmlHelper.BuildSelect(field.Options, name, "", action);
            return htmlHelper.BuildInput(!field.Editable, type, "", name, "", action, null);
        }

        static string GetTextBoxInputType(string name)
        {
            return name.ToLower().Contains("password") ? "password" : "text";
        }

        public static MvcHtmlString BuildTypeAhead<T>(this HtmlHelper<T> htmlHelper, string placeholder, string displayName, Expression<Func<T, object>> action, IEnumerable<string> data)
        {
            return htmlHelper.BuildInput(false, "typeahead", placeholder, displayName, "", action, data);
        }

        public static MvcHtmlString BuildFormTextInput<T>(this HtmlHelper<T> htmlHelper, bool @readonly, string placeholder, string displayName, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildFormTextInput(@readonly, placeholder, displayName, "", action);
        }

        public static MvcHtmlString BuildFormTextInput<T>(this HtmlHelper<T> htmlHelper, string placeholder, string displayName, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildFormTextInput(placeholder, displayName, "", action);
        }

        public static MvcHtmlString BuildFormTextInput<T>(this HtmlHelper<T> htmlHelper, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildInput(false, "text", placeholder, displayName, helpText, action, null);
        }

        public static MvcHtmlString BuildFormTextInput<T>(this HtmlHelper<T> htmlHelper, bool @readonly, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildInput(@readonly, "text", placeholder, displayName, helpText, action, null);
        }

        public static MvcHtmlString BuildFormTextInput<T>(this HtmlHelper<T> htmlHelper, string id, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildInput(id, false, "text", placeholder, displayName, helpText, action, null);
        }

        public static MvcHtmlString BuildFormPasswordInput<T>(this HtmlHelper<T> htmlHelper, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            return htmlHelper.BuildInput(false, "password", placeholder, displayName, helpText, action, null);
        }

        private static MvcHtmlString BuildInput<T>(this HtmlHelper<T> htmlHelper, bool @readonly, string inputType, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action, IEnumerable<string> typeaheadData)
        {

            return htmlHelper.BuildInput("", @readonly, inputType, placeholder, displayName, helpText, action, typeaheadData);
        }

        private static MvcHtmlString BuildInput<T>(this HtmlHelper<T> htmlHelper, string id, bool @readonly, string inputType, string placeholder, string displayName, string helpText, Expression<Func<T, object>> action, IEnumerable<string> typeaheadData)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";
            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            if (inputType == "typeahead")
            {
                builder.Append(string.Format("\n\t\t<input type=\"text\" autocomplete=\"off\" data-provide=\"typeahead\" data-source='[{3}]' id=\"{0}\" name=\"{0}\" placeholder=\"{1}\" value=\"{2}\" class=\"xlarge\"/>", inputName, placeholder, value, string.Join(",", typeaheadData.Select(x => string.Format("\"{0}\"", x)))));
            }
            else
            {
                builder.Append(string.Format("\n\t\t<input {0} type=\"{1}\" id=\"{2}\" name=\"{2}\" placeholder=\"{3}\" value=\"{4}\" class=\"xlarge\"/>", @readonly ? "readonly=\"readonly\"" : "", inputType, string.IsNullOrWhiteSpace(id) ? inputName : id, placeholder, value));
            }
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildTextArea<T>(this HtmlHelper<T> htmlHelper, string displayName, string helpText, string classesString, int cols, int rows, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";
            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<textarea cols=\"{0}\" rows=\"{1}\"class=\"xlarge {2}\" name=\"{3}\" id=\"{3}\">{4}</textarea>", cols, rows, classesString, inputName, value));
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildTextArea<T>(this HtmlHelper<T> htmlHelper, string displayName, string id, string helpText, string classesString, int cols, int rows, Expression<Func<T, object>> action)
        {            
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var value = field != null ? field.ToString() : "";
            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, id, displayName);
            builder.Append(string.Format("\n\t\t<textarea cols=\"{0}\" rows=\"{1}\"class=\"xlarge {2}\" name=\"{3}\" id=\"{3}\">{4}</textarea>", cols, rows, classesString, id, value));
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(id, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString Submit(this HtmlHelper htmlHelper, string name)
        {
            return new MvcHtmlString(string.Format("<input type=\"submit\" value=\"{0}\" class=\"btn btn-primary btn-large\"/>", name));
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, string name)
        {
            return new MvcHtmlString(string.Format("<button class=\"btn btn-primary btn-large\">{0}</button>", name));
        }

        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string name, string href)
        {
            return new MvcHtmlString(string.Format("<a href=\"{0}\" class=\"btn btn-primary btn-large\">{1}</a>", href, name));
        }

        public static MvcHtmlString BuildRadioButtons<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, string displayName, string helpText, Expression<Func<T, object>> action, string radioButtonSeparatorasHtml = "<br/>")
        {
            var expression = GetMemberInfo(action);
            var inputName = expression.Member.Name;
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);

            foreach (var option in options)
            {
                builder.Append(string.Format("\n\t\t<input type=\"radio\" name=\"{0}\" value=\"{1}\" class=\"xlarge\" {4}/> {2}{3}", inputName, option.Key, option.Value, radioButtonSeparatorasHtml, option.Key.ToLower() == value.ToLower() ? "checked" : ""));

            }
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }


        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<string> options, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", inputName));
            builder.Append(string.Format("<option name={0}>{0}</option>", value));
            foreach (var option in options)
            {
                builder.Append(string.Format("<option name={0}>{0}</option>", option));
            }
            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, string id, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", id));
            if (string.IsNullOrWhiteSpace(value) || options.Where(x => x.Key == value).Count() == 0)
            {
                builder.Append("<option>Select ...</option>");
            }
            foreach (var option in options)
            {
                builder.Append(option.Key == value
                    ? string.Format("<option value=\"{0}\" selected=\"selected\">{1}</option>", option.Key, option.Value)
                    : string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
            }

            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, IEnumerable<string> data, string dataName,  string id, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", id));
            if (string.IsNullOrWhiteSpace(value) || options.Where(x => x.Key == value).Count() == 0)
            {
                builder.Append("<option>Select ...</option>");
            }
            for (int i = 0; i < options.Count(); i++)
            {                          
                builder.Append(options.ElementAt(i).Key == value
                    ? string.Format("<option value=\"{0}\" data-{2}=\"{3}\" selected=\"selected\">{1}</option>", options.ElementAt(i).Key, options.ElementAt(i).Value, dataName, data.ElementAt(i))
                    : string.Format("<option value=\"{0}\" data-{2}=\"{3}\">{1}</option>", options.ElementAt(i).Key, options.ElementAt(i).Value, dataName, data.ElementAt(i)));
            }

            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);

            return new MvcHtmlString(builder.ToString());
        }


        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", inputName));
            if (string.IsNullOrWhiteSpace(value) || options.Where(x => x.Key == value).Count() == 0)
            {
                builder.Append("<option>Select ...</option>");                
            }
            foreach (var option in options)
            {
                builder.Append(option.Key == value
                    ? string.Format("<option value=\"{0}\" selected=\"selected\">{1}</option>", option.Key, option.Value)
                    : string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
            }
            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<GroupOf<KeyValuePair<string, string>>> groupOfOptions, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", inputName));
            var allOptions = groupOfOptions.SelectMany(x => x.Items);
            if (string.IsNullOrWhiteSpace(value) || allOptions.Where(x => x.Key == value).Count() == 0)
            {
                builder.Append("<option>Select ...</option>");
            }
            foreach (var group in groupOfOptions)
            {
                builder.Append(string.Format("<optgroup id=\"{0}\" label=\"{1}\">", group.GroupId, group.GroupName));
                var options = group.Items;
                foreach (var option in options)
                {
                    builder.Append(option.Key == value
                        ? string.Format("<option value=\"{0}\" selected>{1}</option>", option.Key, option.Value)
                        : string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
                }
                builder.Append("</optgroup>");

            }
            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<GroupOf<KeyValuePair<string, string>>> groupOfOptions, string displayName, string helpText, string inputName)
        {
            return BuildSelect<T>(htmlHelper, groupOfOptions, displayName, helpText, inputName, false);
        }

        public static MvcHtmlString BuildGroupedMultipleSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<GroupOf<KeyValuePair<string, string>>> groupOfOptions, string displayName, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var inputName = expression.Member.Name;
            return BuildSelect<T>(htmlHelper, groupOfOptions, displayName, helpText, inputName, true);
        }


        public static MvcHtmlString BuildSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<GroupOf<KeyValuePair<string, string>>> groupOfOptions, string displayName, string helpText, string inputName, bool multiple)
        {

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" {1} class=\"xlarge\">", inputName, multiple ? "multiple=\"\"" : ""));
            foreach (var group in groupOfOptions)
            {
                builder.Append(string.Format("<optgroup id=\"{0}\" label=\"{1}\">", group.GroupId, group.GroupName));
                var options = group.Items;
                foreach (var option in options)
                {
                    builder.Append(string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
                }
                builder.Append("</optgroup>");

            }
            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildMultipleSelect<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, string displayName, string placeholder, string helpText, Expression<Func<T, object>> action)
        {
            var expression = GetMemberInfo(action);
            var field = action.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, displayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" multiple=\"\" class=\"xlarge\">", inputName));
            if ((string.IsNullOrWhiteSpace(value) || options.Where(x => x.Key == value).Count() == 0) && !string.IsNullOrWhiteSpace(placeholder))
            {
                builder.Append(string.Format("<option>{0}</option>", placeholder));
            }
            foreach (var option in options)
            {
                builder.Append(value.Contains(option.Key)
                    ? string.Format("<option value=\"{0}\" selected>{1}</option>", option.Key, option.Value)
                    : string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
            }
            builder.Append("</select>");
            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, helpText)));
            AppendFormEndOfInputWrappers(builder);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString BuildSelectWithTextInput<T>(this HtmlHelper<T> htmlHelper, IEnumerable<KeyValuePair<string, string>> options, string selectId, string selectDisplayName, string selectHelpText, Expression<Func<T, object>> selectAction, string textInputId, string textInputPlaceholder, string textInputDisplayName, Expression<Func<T, object>> textInputAction)
        {
            var expression = GetMemberInfo(selectAction);
            var field = selectAction.Compile().Invoke(htmlHelper.ViewData.Model);
            var inputName = expression.Member.Name;
            var value = field != null ? field.ToString() : "";

            var builder = new StringBuilder();
            AppendFormStartOfInputWrappers(htmlHelper, builder, inputName, selectDisplayName);
            builder.Append(string.Format("\n\t\t<select id=\"{0}\" name=\"{0}\" class=\"xlarge\">", selectId));
            if (string.IsNullOrWhiteSpace(value) || options.Where(x => x.Key == value).Count() == 0)
            {
                builder.Append("<option>Select ...</option>");
            }
            foreach (var option in options)
            {
                builder.Append(option.Key == value
                    ? string.Format("<option value=\"{0}\" selected>{1}</option>", option.Key, option.Value)
                    : string.Format("<option value=\"{0}\">{1}</option>", option.Key, option.Value));
            }

            builder.Append("</select>");

            builder.Append(htmlHelper.BuildFormTextInput(textInputId, textInputPlaceholder, textInputDisplayName, "",
                                                          textInputAction));

            builder.Append(string.Format("\n\t\t<span class=\"help-inline\">{0}</span>", htmlHelper.GetErrorOrDisplayHelp(inputName, selectHelpText)));
            AppendFormEndOfInputWrappers(builder);

            return new MvcHtmlString(builder.ToString());
        }       
        
        static void AppendFormStartOfInputWrappers<T>(this HtmlHelper<T> htmlHelper, StringBuilder builder, string inputName, string displayName)
        {
            builder.Append(string.Format("\n<div class=\"{0}\">", htmlHelper.GetFormInputClass(inputName)));
            if (!string.IsNullOrWhiteSpace(displayName))
                builder.Append(string.Format("\n\t<label for=\"{0}\">{1}:</label>", inputName, displayName));
            builder.Append("\n\t<div class=\"input\">");
        }

        public static MvcHtmlString GetError(this HtmlHelper htmlHelper, string name)
        {
            return htmlHelper.HasError(name)
                ? new MvcHtmlString(htmlHelper.ViewData.ModelState[name].Errors[0].ErrorMessage)
                : new MvcHtmlString("");
        }

        public static MvcHtmlString GetErrorOrDisplayHelp(this HtmlHelper htmlHelper, string name, string helpText)
        {
            return htmlHelper.HasError(name)
                ? new MvcHtmlString(htmlHelper.ViewData.ModelState[name].Errors[0].ErrorMessage)
                : new MvcHtmlString(helpText);
        }

        public static MvcHtmlString GetFormInputClass(this HtmlHelper htmlHelper, string name)
        {
            return htmlHelper.HasError(name)
                ? new MvcHtmlString("control-group error")
                : new MvcHtmlString("control-group");
        }

        public static string FriendlyName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var sb = new StringBuilder();
            foreach (var c in s.ToCharArray())
            {
                if ((Char.IsUpper(c) || Char.IsDigit(c)) && sb.Length > 0)
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        public static bool HasError(this HtmlHelper htmlHelper, string name)
        {
            return htmlHelper.ViewData.ModelState.ContainsKey(name) &&
                htmlHelper.ViewData.ModelState[name].Errors.Count > 0;
        }

        static void AppendFormEndOfInputWrappers(StringBuilder builder)
        {
            builder.Append("\n\t</div>");
            builder.Append("\n</div>");
        }

        static MemberExpression GetMemberInfo(Expression method)
        {
            var lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }
    }
    
}
