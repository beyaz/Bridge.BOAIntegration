using System;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    class CSharpCodeGenerator
    {
        #region Public Properties
        public CSharpCodeGeneratorData Data { get; set; }
        #endregion

        #region Public Methods
        public string GenerateCsharpCode()
        {
            var sb = new PaddedStringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using BOA.Common.Helpers;");
            sb.AppendLine("using Bridge.BOAIntegration;");

            var fullTypeName = GetClassName(Data.RootNode);
            var nsName       = fullTypeName.Substring(0, fullTypeName.LastIndexOf(".", StringComparison.Ordinal));
            var typeName     = fullTypeName.Substring(fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1);

            sb.AppendLine("namespace " + nsName);
            sb.AppendLine("{");
            sb.PaddingCount++;

            var baseClassFullName = XamlHelper.GetCSharpClassFullName(Data.RootNode);

            sb.AppendLine($"public partial class {typeName} : " + baseClassFullName);
            sb.AppendLine("{");
            sb.PaddingCount++;

            foreach (var fieldNameFieldTypePair in Data.FieldDefinitions)
            {
                sb.AppendLine(fieldNameFieldTypePair.Value + " " + fieldNameFieldTypePair.Key + ";");
            }

            sb.AppendLine("");

            sb.AppendLine("protected override object Render()");
            sb.AppendLine("{");
            sb.PaddingCount++;

            var generator = new UIBuilderCodeGenerator
            {
                RootNode    = XmlHelper.GetRootNode(Data.OutputXmlString),
                Caller      = "this",
                Output      = sb,
                DataContext = "this"
            };
            generator.Generate();

            sb.AppendLine("return builder.Result;");

            sb.PaddingCount--;
            sb.AppendLine("}");



            sb.AppendLine("void InitializeComponent()");
            sb.AppendLine("{");
            sb.PaddingCount++;

            sb.AppendLine("XmlUI = @" + '"' + Data.OutputXmlString.Replace("\"", "\"\"") + '"' + ";");

            


            sb.AppendLine("");

            if (Data.RootNodeIsBrowseForm)
            {
                if (Data.InfragisticsDataPresenterFields.Count > 0)
                {
                    sb.AppendLine("this.ConfigureColumns(new []{");

                    for (var i = 0; i < Data.InfragisticsDataPresenterFields.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.AppendLine(",");
                        }

                        var node = Data.InfragisticsDataPresenterFields[i];
                        BOAXamlHelper.WriteAsDataGridColumnInfoContract(sb, node);
                    }

                    sb.AppendLine("},true);");
                }
            }

            sb.AppendLine("PropertyChanged += (s,e)=>{ ForceRender();}; // Model yapısı kullanılmış ise bu gibi şeylere gerek yok aslında.belki de var incelemek lazım");

            sb.AppendLine("");
            sb.AppendLine("// EvaluateInWhichCaseRenderMethodWillBeCall");

            var controlGridDataSourceBindingPath = GetBrowseForm_ControlGridDataSource_BindingPath(Data.RootNode);
            if (controlGridDataSourceBindingPath.StartsWith("Model."))
            {
                sb.AppendLine("this.OnPropertyChanged(nameof(Model), ForceRender);");
                sb.AppendLine("this.OnPropertyChanged(nameof(Model), ()=>{ ControlGridDataSource = new string[0]; });");

                sb.AppendLine("this.OnPropertyChanged(nameof(Model), () =>");

                sb.AppendLine("{");
                sb.PaddingCount++;

                sb.AppendLine("Model?.OnPropertyChanged( \"" + controlGridDataSourceBindingPath.Substring("Model.".Length) + "\" , () =>");

                sb.AppendLine("{");
                sb.PaddingCount++;

                sb.AppendLine("ControlGridDataSource = " + controlGridDataSourceBindingPath + ".ToArray();");

                sb.PaddingCount--;
                sb.AppendLine("});");

                sb.PaddingCount--;
                sb.AppendLine("});");
            }
            else
            {
                if (controlGridDataSourceBindingPath.Contains("."))
                {
                    throw new ArgumentException("controlGridDataSourceBindingPath.Contains('.')");
                }

                sb.AppendLine("this.OnPropertyChanged(nameof(" + controlGridDataSourceBindingPath + "), () =>");
                sb.AppendLine("{");
                sb.PaddingCount++;

                sb.AppendLine("ControlGridDataSource = " + controlGridDataSourceBindingPath + "?.ToArray();");

                sb.PaddingCount--;
                sb.AppendLine("});");
            }

            sb.PaddingCount--;
            sb.AppendLine("}"); // end of method

            sb.PaddingCount--;
            sb.AppendLine("}"); // end of class

            sb.PaddingCount--;
            sb.AppendLine("}"); // end of namespace

            return sb.ToString();
        }
        #endregion

        #region Methods
        static string GetBrowseForm_ControlGridDataSource_BindingPath(XmlNode RootNode)
        {
            var value = RootNode.Attributes?.GetNamedItem("ControlGridDataSource")?.Value;
            if (value == null)
            {
                return null;
            }

            return value.Replace("{", "").Replace("}", "").Replace("Binding ", "").Remove(",Mode=OneWay").Remove(",Mode=TwoWay").Trim();
        }

        static string GetClassName(XmlNode RootNode)
        {
            var fullTypeName = RootNode.Attributes?.GetNamedItem("x:Class")?.Value;
            if (fullTypeName == null)
            {
                return null;
            }

            return fullTypeName;
        }
        #endregion
    }
}