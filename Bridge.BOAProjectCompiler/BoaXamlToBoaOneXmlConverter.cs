using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Bridge.BOAProjectCompiler
{
    class BoaXamlToBoaOneXmlConverter
    {
        public string InputXamlString { get; set; }

        XDocument _xDocument;

        public XDocument XDocument
        {
            get
            {
                if (_xDocument == null)
                {
                    _xDocument = System.Xml.Linq.XDocument.Parse(InputXamlString);
                }

                return _xDocument;
            }
        }

        public string OutputXmlString
        {
            get { return XDocument.ToString(); }
        }

        internal void TransformNodes()
        {
            var elements = XDocument.XPathSelectElements("//StackPanel");

            foreach (var element in elements)
            {
                Transform_StackPanel(element);
            }
            
            
            
        }

        void Transform_StackPanel(XElement element)
        {
            var bGridSection = new XElement("BGridSection");




            var nodes = element.Nodes().ToList();

           
            foreach (var node in nodes)
            {
                var bGridRow = new XElement("BGridRow");
                node.Remove();
                bGridRow.Add(node);

                bGridSection.Add(bGridRow);
            }
          


            element.AddBeforeSelf(bGridSection);

            element.Remove();

        }
    }
}