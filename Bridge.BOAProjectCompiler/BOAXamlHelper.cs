﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    static class BOAXamlHelper
    {
        #region Public Methods
        public static string GetNamespacePrefix_boa_BusinessComponents_ns(IDictionary<string, string> Namespaces)
        {
            return Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI.BusinessComponents;assembly=BOA.UI.BusinessComponents").Key;
        }

        public static string GetNamespacePrefix_boa_ui_ns(IDictionary<string, string> Namespaces)
        {
            return Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI;assembly=BOA.UI").Key;
        }

        public static void WriteAsDataGridColumnInfoContract(PaddedStringBuilder builder, XmlNode node)
        {
            builder.AppendLine("new DataGridColumnInfoContract");
            builder.AppendLine("{");
            builder.PaddingCount++;

            var name  = node.Attributes?["Name"]?.Value;
            var label = node.Attributes?["Label"]?.Value;

            bool? isFirst = null;

            if (name != null)
            {
                name = '"' + name + '"';
                builder.AppendLine("BindingPath = " + name);

                isFirst = true;
            }

            if (label != null)
            {
                if (isFirst == true)
                {
                    builder.AppendLine(",");
                }

                if (MessagingResolver.IsMessagingExpression(label))
                {
                    var messagingExpression = MessagingResolver.GetMessagingExpressionValue(label);

                    label = $"BOA.Messaging.MessagingHelper.GetMessage(\"{messagingExpression.GroupName}\",\"{messagingExpression.PropertyName}\")";
                }
                else
                {
                    label = '"' + label + '"';
                }

                builder.AppendLine("Label = " + label);
            }

            builder.PaddingCount--;
            builder.AppendLine("}");
        }
        #endregion
    }
}