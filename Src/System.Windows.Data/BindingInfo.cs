using System.Collections.Generic;
using System.Text.Tokenizers;
using Bridge.Html5;

namespace System.Windows.Data
{
    public class BindingInfo
    {
        #region Static Fields
        static readonly Tokenizer BindingExpressionTokenizer = new Tokenizer
        {
            TokenDefinitions = BindingExpressionTokenDefinitions.Value
        };
        #endregion

        #region Public Properties
        public BindingMode     BindingMode        { get; set; }
        public IValueConverter Converter          { get; set; }
        public object          ConverterParameter { get; set; }
        public object          Source             { get; set; }
        public PropertyPath    SourcePath         { get; set; }
        public object          Target             { get; set; }
        public PropertyPath    TargetPath         { get; set; }
        #endregion

        #region Public Methods
        public static BindingInfo TryParseExpression(string value)
        {
            if (value == null)
            {
                return null;
            }

            value = value.Trim();

            if (value.StartsWith("{") == false)
            {
                return null;
            }

            if (value.EndsWith("}") == false)
            {
                return null;
            }

            string          sourcePath         = null;
            var             bindingMode        = BindingMode.TwoWay;
            IValueConverter valueConverter     = null;
            string          converterParameter = null;

            var tokens = BindingExpressionTokenizer.Tokenize(value);
            var len    = tokens.Count;
            for (int i = 0; i < len; i++)
            {
                var token = tokens[i];

                if (token.Value.ToUpperCase() == "BINDING" || token.Value == " ")
                {
                    continue;
                }

                if (sourcePath == null && token.TokenType == TokenType.Identifier)
                {
                    sourcePath = ReadPath(tokens, ref i);

                    continue;
                }

                if (token.Value.ToUpperCase() == "MODE")
                {
                    i++; // skip mode

                    SkipAssignmentAndSpace(tokens,ref i);

                    Enum.TryParse(tokens[i].Value, out bindingMode);
                    continue;
                }

                if (token.Value.ToUpperCase() == "CONVERTERPARAMETER")
                {
                    i++; // skip converterparameter
                    SkipAssignmentAndSpace(tokens, ref i);

                    converterParameter = ReadConverterParameter(tokens, ref i);
                    converterParameter = converterParameter.Trim();

                    if (converterParameter.StartsWith("'") && 
                        converterParameter.EndsWith("'"))
                    {
                        converterParameter = converterParameter.RemoveFromStart("'").RemoveFromEnd("'");
                    }

                    continue;
                }

                if (token.Value.ToUpperCase() == "CONVERTER")
                {
                    i++; // skip converter

                    SkipAssignmentAndSpace(tokens, ref i);

                    var converterTypeFullName = ReadPath(tokens, ref i);

                    var converterType = Type.GetType(converterTypeFullName);
                    if (converterType == null)
                    {
                        throw new MissingMemberException(converterTypeFullName);
                    }

                    valueConverter = (IValueConverter) Activator.CreateInstance(converterType);
                }
            }

            return new BindingInfo
            {
                SourcePath         = sourcePath,
                BindingMode        = bindingMode,
                Converter          = valueConverter,
                ConverterParameter = converterParameter
            };
        }

        public void Connect()
        {
            ConnectSourceToTarget();

            if (BindingMode == BindingMode.TwoWay)
            {
                ConnectTargetToSource();
            }
            else
            {
                TargetPath.Walk(Target);
            }

            UpdateTarget();
        }

        public virtual void UpdateSource()
        {
            if (SourcePath.IsNotReadyToUpdate)
            {
                return;
            }

            SourcePath.SetPropertyValue(GetTargetValue());
        }

        public virtual void UpdateTarget()
        {
            if (TargetPath.IsNotReadyToUpdate)
            {
                return;
            }

            var value = SourcePath.GetPropertyValue();

            if (Converter != null)
            {
                value = Converter.Convert(value, null, ConverterParameter, null);
            }

            TargetPath.SetPropertyValue(value);
        }
        #endregion

        #region Methods
        protected virtual void ConnectSourceToTarget()
        {
            SourcePath.Listen(Source, UpdateTarget);
        }

        protected virtual void ConnectTargetToSource()
        {
            TargetPath.Listen(Target, UpdateSource);
        }

        protected virtual object GetTargetValue()
        {
            return TargetPath.GetPropertyValue();
        }



        internal  static void SkipAssignmentAndSpace(IReadOnlyList<Token> tokens, ref int i)
        {
            var len = tokens.Count;

            while (i < len)
            {
                var token = tokens[i];

                if (token.Value == "=" ||
                    token.Value == " ")
                {
                    i++;
                    continue;
                }
                return;
                
            }

        }


        static string ReadConverterParameter(IReadOnlyList<Token> tokens, ref int i)
        {
            var len = tokens.Count;

            var path = "";
            while (i < len)
            {
                var token = tokens[i];

                if (token.TokenType == TokenType.Comma ||
                    token.TokenType == TokenType.RightBracket)
                {
                    i--;
                    break;
                }

                path += token.Value;
                i++;
            }

            return path;
        }

        internal static string ReadPath(IReadOnlyList<Token> tokens, ref int i)
        {
            var path = "";
            while (true)
            {
                var token = tokens[i];

                if (token.Value == " ")
                {
                    i++;
                    continue;
                }

                if (token.TokenType == TokenType.Identifier ||
                    token.TokenType == TokenType.Dot)
                {
                    path += token.Value;
                    i++;
                }
                else
                {
                    i--;
                    break;
                }
            }

            return path;
        }
        #endregion
    }
}