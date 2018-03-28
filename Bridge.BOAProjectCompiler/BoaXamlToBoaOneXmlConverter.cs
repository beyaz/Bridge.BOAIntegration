using System;
using System.Collections.Generic;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    class BoaXamlToBoaOneXmlConverter
    {
        #region Fields
        readonly List<Action<TransformerData>> XmlNodeTransformers = new List<Action<TransformerData>>
        {
            Transformer.StackPanel,
            Transformer.BMaskedEditorLabeled,
            Transformer.AccountComponent,
            Transformer.BDateTimeEditorLabeled,
            Transformer.BComboEditorMultiSelect,
            Transformer.BBranchComponent,
            Transformer.ParameterComponent,
            Transformer.BTextEditorLabeled,
            Transformer.BOAControls_BField,
            Transformer.BNumericEditorLabeled,
            Transformer.BComboEditorLabeled
        };
        #endregion

        #region Public Properties
        public string InputXamlString { get; set; }
        #endregion

        #region Public Methods
        public string GenerateCsharpCode()
        {
            var document = new XmlDocument();
            document.LoadXml(InputXamlString);
            var rootNode = document.FirstChild;

            var FieldDefinitions = new Dictionary<string, string>();

            var namespaces                = XmlHelper.GetAllNamespaces(document);
            var boa_BusinessComponents_ns = BOAXamlHelper.GetNamespacePrefix_boa_BusinessComponents_ns(namespaces);
            var boa_ui_ns                 = BOAXamlHelper.GetNamespacePrefix_boa_ui_ns(namespaces);

            var elements = document.GetElementsByTagName("*").ToList();
            var transformerInput = new TransformerData
            {
                FieldDefinitions          = FieldDefinitions,
                Document                  = document,
                Namespaces                = namespaces,
                boa_BusinessComponents_ns = boa_BusinessComponents_ns,
                boa_ui_ns                 = boa_ui_ns
            };

            // TransformNodes
            foreach (var xmlNode in elements)
            {
                transformerInput.XmlNode = xmlNode;

                XmlNodeTransformers.ForEach(t => t(transformerInput));
            }

            var generator = new CSharpCodeGenerator
            {
                Data = new CSharpCodeGeneratorData
                {
                    RootNode             = rootNode,
                    FieldDefinitions     = FieldDefinitions,
                    RootNodeIsBrowseForm = rootNode.Name == boa_ui_ns + ":BrowseForm",
                    InfragisticsDataPresenterFields = transformerInput.InfragisticsDataPresenterFields
                }
            };

            return generator.GenerateCsharpCode();
        }
        #endregion
    }
}