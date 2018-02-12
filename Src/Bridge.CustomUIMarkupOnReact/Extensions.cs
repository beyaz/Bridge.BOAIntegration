using System;
using Bridge.Html5;

namespace Bridge.CustomUIMarkupOnReact
{
    static class Extensions
    {
        #region Public Methods
        public static string GetInnerText(this Element node)
        {
            if (node.NodeType == NodeType.Text)
            {
                return node["textContent"].As<string>();
            }

            return node["innerHTML"].As<string>();
        }
        #endregion
    }
}