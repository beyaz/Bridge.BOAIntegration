using System.Collections.Generic;
using System.Xml;
using BOA.UI;

namespace Bridge.BOAProjectCompiler
{
    public  class TransformerInput
    {
        public List<DataGridColumnInfoContract> DataGridColumnInfoContracts { get; set; } = new List<DataGridColumnInfoContract>();
        public Dictionary<string, string>  FieldDefinitions          { get; set; }
        public XmlNode                     XmlNode                   { get; set; }
        public XmlDocument                 Document                  { get; set; }
        public IDictionary<string, string> Namespaces                { get; set; }
        public string                      boa_BusinessComponents_ns { get; set; }
        public string                      boa_ui_ns                 { get; set; }
    }
}