using System.Collections.Generic;
using System.Xml;
using BOA.Common.Helpers;


namespace Bridge.BOAProjectCompiler
{
    class CSharpCodeGeneratorInput
    {
        #region Fields
        public IReadOnlyDictionary<string, string> FieldDefinitions;
        #endregion

        #region Public Properties
        public string OutputXmlString
        {
            get
            {
                if (RootNodeIsBrowseForm)
                {
                    return XmlHelper.PrettyXml(RootNode.FirstChild.OuterXml);
                }

                return XmlHelper.PrettyXml(RootNode.OuterXml);
            }
        }

        public XmlNode RootNode             { get; set; }
        public bool    RootNodeIsBrowseForm { get; set; }
        public List<XmlNode> InfragisticsDataPresenterFields { get; set; }
        #endregion
    }
}