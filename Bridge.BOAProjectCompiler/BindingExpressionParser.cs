using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using BOA.Common.Helpers;
using Lang.Data;
using Lang.Lexers;

namespace Bridge.BOAProjectCompiler
{
    class BindingExpressionParser
    {
        #region Public Methods
        public static BindingInfoContract TryParse(string attributeValue)
        {
            var lexer  = new Lexer(attributeValue);
            var tokens = lexer.Lex().ToList();

            if (tokens.First().TokenType != TokenType.LBracket)
            {
                return null;
            }

            if (tokens.Last().TokenType != TokenType.RBracket)
            {
                return null;
            }

            string sourcePath            = null;
            var    bindingMode           = BindingMode.TwoWay;
            string converterTypeFullName = null;
            string converterParameter    = null;

            var len = tokens.Count;
            for (var i = 0; i < len; i++)
            {
                var token = tokens[i];

                if (token.TokenValue.ToUpper() == "BINDING" || token.TokenValue == " ")
                {
                    continue;
                }

                if (sourcePath == null && token.TokenType == TokenType.Word)
                {
                    sourcePath = ReadPath(tokens, ref i);

                    continue;
                }

                if (token.TokenValue.ToUpper() == "MODE")
                {
                    i++; // skip mode

                    Enum.TryParse(tokens[i].TokenValue, out bindingMode);
                    continue;
                }

                if (token.TokenValue.ToUpper() == "CONVERTERPARAMETER")
                {
                    i++; // skip converterparameter

                    converterParameter = ReadConverterParameter(tokens, ref i);
                    converterParameter = converterParameter.Trim();

                    if (converterParameter.StartsWith("'") &&
                        converterParameter.EndsWith("'"))
                    {
                        converterParameter = converterParameter.RemoveFromStart("'").RemoveFromEnd("'");
                    }

                    continue;
                }

                if (token.TokenValue.ToUpper() == "CONVERTER")
                {
                    i++; // skip converter

                    converterTypeFullName = ReadPath(tokens, ref i);
                }
            }

            return new BindingInfoContract
            {
                SourcePath            = sourcePath,
                BindingMode           = bindingMode,
                ConverterTypeFullName = converterTypeFullName,
                ConverterParameter    = converterParameter
            };
        }
        #endregion

        #region Methods
        static string ReadConverterParameter(IReadOnlyList<Token> tokens, ref int i)
        {
            var len = tokens.Count;

            var path = "";
            while (i < len)
            {
                var token = tokens[i];

                if (token.TokenType == TokenType.Comma ||
                    token.TokenType == TokenType.RBracket)
                {
                    i--;
                    break;
                }

                path += token.TokenValue;
                i++;
            }

            return path;
        }

        static string ReadPath(IReadOnlyList<Token> tokens, ref int i)
        {
            var path = "";
            while (true)
            {
                var token = tokens[i];

                if (token.TokenValue == " ")
                {
                    i++;
                    continue;
                }

                if (token.TokenType == TokenType.Word ||
                    token.TokenType == TokenType.Dot)
                {
                    path += token.TokenValue;
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