using System.IO;
using System.Linq;
using System.Text;

namespace Bridge.BOAProjectCompiler
{
    class CsprojFileCreator
    {
        #region Public Properties
        public CsprojFileCreatorData Data { get; set; }
        #endregion

        #region Public Methods
        public void CreateFile()
        {
            CleanProjectFolders();

            ConvertXamlFiles();

            var sb = new StringBuilder();

            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.AppendLine(@"<Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
            sb.AppendLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
            sb.AppendLine(@"  <PropertyGroup>");
            sb.AppendLine(@"    <NoStdLib>true</NoStdLib>");
            sb.AppendLine(@"    <NuGetPackageImportStamp>");
            sb.AppendLine(@"    </NuGetPackageImportStamp>");
            sb.AppendLine(@"  </PropertyGroup>");
            sb.AppendLine(@"  <PropertyGroup>");
            sb.AppendLine(@"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>");
            sb.AppendLine(@"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>");
            sb.AppendLine(@"    <SchemaVersion>2.0</SchemaVersion>");
            sb.AppendLine(@"    <ProjectGuid>{501100EC-6824-494B-89ED-8EC8ED9873D2}</ProjectGuid>");
            sb.AppendLine(@"    <OutputType>Library</OutputType>");
            sb.AppendLine(@"    <AppDesignerFolder>Properties</AppDesignerFolder>");
            sb.AppendLine(@"    <RootNamespace>" + Data.AssemblyName + "</RootNamespace>");
            sb.AppendLine(@"    <AssemblyName>" + Data.AssemblyName + "</AssemblyName>");
            sb.AppendLine(@"    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>");
            sb.AppendLine(@"    <FileAlignment>512</FileAlignment>");
            sb.AppendLine(@"  </PropertyGroup>");
            sb.AppendLine(@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">");
            sb.AppendLine(@"    <DebugSymbols>true</DebugSymbols>");
            sb.AppendLine(@"    <DebugType>full</DebugType>");
            sb.AppendLine(@"    <Optimize>false</Optimize>");
            sb.AppendLine(@"    <OutputPath>bin\Debug\</OutputPath>");
            sb.AppendLine(@"    <DefineConstants>TRACE;DEBUG;IsTraceEnabled</DefineConstants>");
            sb.AppendLine(@"    <ErrorReport>prompt</ErrorReport>");
            sb.AppendLine(@"    <WarningLevel>4</WarningLevel>");
            sb.AppendLine(@"  </PropertyGroup>");
            sb.AppendLine(@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">");
            sb.AppendLine(@"    <DebugType>pdbonly</DebugType>");
            sb.AppendLine(@"    <Optimize>true</Optimize>");
            sb.AppendLine(@"    <OutputPath>bin\Release\</OutputPath>");
            sb.AppendLine(@"    <DefineConstants>TRACE</DefineConstants>");
            sb.AppendLine(@"    <ErrorReport>prompt</ErrorReport>");
            sb.AppendLine(@"    <WarningLevel>4</WarningLevel>");
            sb.AppendLine(@"  </PropertyGroup>");
            sb.AppendLine(@"  <ItemGroup>");

            foreach (var sourceFile in Data.SourceFiles)
            {
                sb.AppendLine(@"    <Compile Include=" + '"' + sourceFile + '"' + "/>");
            }

            sb.AppendLine(@"  </ItemGroup>");
            sb.AppendLine(@"  <ItemGroup>");

            sb.AppendLine(@"    <Reference Include=""Bridge"">");
            sb.AppendLine(@"      <HintPath>" + Data.PackagesDirectory + @"Bridge.Core." + Data.BridgeVersionNumber + @"\lib\net40\Bridge.dll</HintPath>");
            sb.AppendLine(@"    </Reference>");

            if (Data.ReferenceAssemblyPaths != null)
            {
                foreach (var assemblyPath in Data.ReferenceAssemblyPaths)
                {
                    sb.AppendLine(@"    <Reference Include=" + '"' + assemblyPath + '"' + "/>");
                }
            }

            sb.AppendLine(@"    <Reference Include=""Bridge.BOAIntegration.dll"">");
            sb.AppendLine(@"      <HintPath>" + Data.Bridge_BOAIntegration_dll_Path + @"</HintPath>");
            sb.AppendLine(@"    </Reference>");

            sb.AppendLine(@"    <Reference Include=""Bridge.Html5"">");
            sb.AppendLine(@"      <HintPath>" + Data.PackagesDirectory + @"Bridge.Html5." + Data.BridgeVersionNumber + @"\lib\net40\Bridge.Html5.dll</HintPath>");
            sb.AppendLine(@"    </Reference>");
            sb.AppendLine(@"    <Reference Include=""Bridge.jQuery2"">");
            sb.AppendLine(@"      <HintPath>" + Data.PackagesDirectory + @"Bridge.jQuery.2.13.0\lib\net40\Bridge.jQuery2.dll</HintPath>");
            sb.AppendLine(@"    </Reference>");
            sb.AppendLine(@"    <Reference Include=""Newtonsoft.Json"">");
            sb.AppendLine(@"      <HintPath>" + Data.PackagesDirectory + @"Bridge.Newtonsoft.Json.1.6.0\lib\net40\Newtonsoft.Json.dll</HintPath>");
            sb.AppendLine(@"    </Reference>");
            sb.AppendLine(@"  </ItemGroup>");
            sb.AppendLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
            sb.AppendLine(@"  <Import Project=""" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets"" Condition=""Exists('" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets')"" />");
            sb.AppendLine(@"  <Target Name=""EnsureNuGetPackageBuildImports"" BeforeTargets=""PrepareForBuild"">");
            sb.AppendLine(@"    <PropertyGroup>");
            sb.AppendLine(@"      <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>");
            sb.AppendLine(@"    </PropertyGroup>");
            sb.AppendLine(@"    <Error Condition=""!Exists('" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets')"" Text=""$([System.String]::Format('$(ErrorText)', '" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets'))"" />");
            sb.AppendLine(@"    <Error Condition=""!Exists('" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets')"" Text=""$([System.String]::Format('$(ErrorText)', '" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets'))"" />");
            sb.AppendLine(@"  </Target>");
            sb.AppendLine(@"  <Import Project=""" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets"" Condition=""Exists('" + Data.PackagesDirectory + @"Bridge.Min." + Data.BridgeVersionNumber + @"\build\Bridge.Min.targets')"" />");
            sb.AppendLine(@"</Project>");

            Directory.CreateDirectory(Data.OutputFileDirectory);
            File.WriteAllText(Data.OutputFilePath, sb.ToString(), Encoding.UTF8);
        }
        #endregion

        #region Methods
        void CleanProjectFolders()
        {
            if (Directory.Exists(Data.OutputFileDirectory + "bin"))
            {
                Directory.Delete(Data.OutputFileDirectory + "bin", true);
            }

            if (Directory.Exists(Data.OutputFileDirectory + "obj"))
            {
                Directory.Delete(Data.OutputFileDirectory + "obj", true);
            }
        }

        void ConvertXamlFiles()
        {
            var sourceFiles = Data.SourceFiles.ToList();

            for (var i = 0; i < sourceFiles.Count; i++)
            {
                var filePath = sourceFiles[i];

                if (!filePath.EndsWith(".xaml"))
                {
                    continue;
                }

                var converter = new BoaXamlToBoaOneXmlConverter
                {
                    InputXamlString = File.ReadAllText(filePath)
                };

                var generatedCode = converter.GenerateCsharpCode();

                sourceFiles[i] = Data.OutputFileDirectory + Path.GetFileNameWithoutExtension(filePath) + ".DOM.cs";

                Directory.CreateDirectory(Data.OutputFileDirectory);
                File.WriteAllText(sourceFiles[i], generatedCode, Encoding.UTF8);
            }

            Data.SourceFiles = sourceFiles;
        }
        #endregion
    }
}