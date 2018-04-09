using System.Globalization;
using Bridge.Html5;
using Bridge.jQuery2;

namespace System.Windows.Data
{
    public class HTMLBindingInfo : BindingInfo
    {

       


        #region Public Properties
        public new jQuery Target
        {
            get { return (jQuery) base.Target; }
            set { base.Target = value; }
        }
        #endregion

        #region Public Methods
        public  static HTMLBindingInfo TryParseExpression(string value)
        {
            var bindingInfo = BindingExpressionParser.TryParse(value);
            if (bindingInfo == null)
            {
                return null;
            }

            return new HTMLBindingInfo
            {
                SourcePath         = bindingInfo.SourcePath,
                Converter          = bindingInfo.Converter,
                ConverterParameter = bindingInfo.ConverterParameter
            };
        }

        public override void UpdateTarget()
        {
            var value = SourcePath.GetPropertyValue();

            var path = TargetPath.Path.ToUpperCase();

            if (path == "INNERHTML")
            {
                if (Target.Get(0).NodeType == NodeType.Text)
                {
                    Target.Get(0).NodeValue = value + "";
                    return;
                }

                Target.Html(value + "");
                return;
            }

            if (path == "VALUE")
            {
                Target.Val(value + "").Change();
                return;
            }

            Target.Attr(TargetPath.Path, value + "");
        }
        #endregion

        #region Methods
        internal static bool TargetCanUpdateSource(jQuery element)
        {
            if (element.Get(0).TagName == "INPUT")
            {
                var type = element.Attr("type");
                if (type == null)
                {
                    return false;
                }

                type = type.ToUpperCase();

                if (type == "HIDDEN" ||
                    type == "TEXT" ||
                    type == "TEXTAREA")
                {
                    return true;
                }
            }

            return false;
        }

        protected override void ConnectTargetToSource()
        {
            var element = Target;

            if (element.Get(0).TagName == "INPUT")
            {
                var type = element.Attr("type");

                if (type?.ToUpperCase() == "HIDDEN")
                {
                    Target.On("change", UpdateSource);
                    return;
                }
            }

            Target.FocusOut(ev => { UpdateSource(); });
        }

        protected override object GetTargetValue()
        {
            var val = Target.Val();

            if (val == string.Empty)
            {
                return null;
            }

            return val;
        }

        Type _sourceValueType;
        public override void UpdateSource()
        {
            if (SourcePath.IsNotReadyToUpdate)
            {
                return;
            }

            var targetValue = GetTargetValue();

            var sourceValueType = SourcePath.GetPropertyValue()?.GetType();
            if (sourceValueType == null)
            {
                sourceValueType = _sourceValueType;
            }
            else
            {
                _sourceValueType = sourceValueType;
            }

            if ( targetValue != null &&  sourceValueType?.IsNumeric() == true)
            {
                targetValue =  Cast.To(targetValue, sourceValueType,CultureInfo.CurrentCulture);
            }

            SourcePath.SetPropertyValue(targetValue);
        }

        #endregion
    }
}