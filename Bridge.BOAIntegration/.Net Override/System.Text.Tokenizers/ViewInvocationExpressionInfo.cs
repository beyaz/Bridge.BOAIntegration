using System.Collections.Generic;

namespace System.Text.Tokenizers
{
    internal class ViewInvocationExpressionInfo
    {
        #region Static Fields
        static readonly Tokenizer BindingExpressionTokenizer = new Tokenizer
        {
            TokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition(TokenType.Binding, "this", 1),
                new TokenDefinition(TokenType.OpenParenthesis, "\\(", 1),
                new TokenDefinition(TokenType.CloseParenthesis, "\\)", 1),

                new TokenDefinition(TokenType.Identifier, "[a-zA-Z_$][a-zA-Z0-9_$]*", 1),

                new TokenDefinition(TokenType.StringValue, "'([^']*)'", 1),
                new TokenDefinition(TokenType.Comma, ",", 1),
                new TokenDefinition(TokenType.Dot, ".", 1)
            }
        };
        #endregion

        #region Public Properties
        public bool                  IsStartsWithThis { get; set; }
        public string                MethodName       { get; set; }
        public IReadOnlyList<object> Parameters       { get; set; }
        #endregion

        #region Public Methods
        public static ViewInvocationExpressionInfo Parse(string expression)
        {
            var info = new ViewInvocationExpressionInfo();

            var parameters = new List<object>();

            var tokens = BindingExpressionTokenizer.Tokenize(expression);
            var len    = tokens.Count;
            for (var i = 0; i < len; i++)
            {
                var token = tokens[i];

                if (token.Value.ToUpper() == "THIS" || token.Value == " " || token.Value == "(" || token.Value == ")" || token.Value == "," || token.Value == ".")
                {
                    info.IsStartsWithThis = true;
                    continue;
                }

                if (info.MethodName == null && token.TokenType == TokenType.Identifier)
                {
                    info.MethodName = token.Value;

                    continue;
                }

                // in parameters

                if (token.Value.StartsWith("'"))
                {
                    var valueLen = token.Value.Length;
                    parameters.Add(token.Value.Substring(1, valueLen - 2));
                    continue;
                }

                parameters.Add(decimal.Parse(token.Value));
            }

            info.Parameters = parameters;
            return info;
        }
        #endregion
    }
}