﻿using System.IO;
using System.Text;

namespace Bridge.BOAProjectCompiler
{
    class CsprojFile
    {
        #region Public Properties
        public string FileName         { get; set; }
        public string WorkingDirectory { get; set; } = @"d:\temp\";
        public string PackagesDirectory { get; set; } = @"D:\github\Bridge.BOAIntegration\packages\";
        #endregion

        #region Public Methods
        public void WriteToFile()
        {
            var sb = new StringBuilder();


            sb.Append(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <NoStdLib>true</NoStdLib>
    <NuGetPackageImportStamp>
    </NuGetPackageImportStamp>
  </PropertyGroup>
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{501100EC-6824-494B-89ED-8EC8ED9873D2}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <AssemblyName>Bridge.BOAIntegration</AssemblyName>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>TRACE;DEBUG;IsTraceEnabled</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include=""..\..\..\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\UI\BOA.UI.CardGeneral.DebitCard.Common\Data\DataGridColumnInfoContract.cs"">
      <Link>Bridge.BOAIntegration\DataGridColumnInfoContract.cs</Link>
    </Compile>
   
  </ItemGroup>
  <ItemGroup>
    <None Include=""bridge.json"" />
    <None Include=""packages.config"">
      <SubType>Designer</SubType>
    </None>
  </ItemGroup>
  
  <ItemGroup>
    <Reference Include=""Bridge, Version=16.8.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\packages\Bridge.Core.16.8.2\lib\net40\Bridge.dll</HintPath>
    </Reference>
    <Reference Include=""Bridge.Html5, Version=16.8.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\packages\Bridge.Html5.16.8.2\lib\net40\Bridge.Html5.dll</HintPath>
    </Reference>
    <Reference Include=""Bridge.jQuery2, Version=2.13.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\packages\Bridge.jQuery.2.13.0\lib\net40\Bridge.jQuery2.dll</HintPath>
    </Reference>
    <Reference Include=""Newtonsoft.Json, Version=1.6.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\packages\Bridge.Newtonsoft.Json.1.6.0\lib\net40\Newtonsoft.Json.dll</HintPath>
    </Reference>
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <Import Project=""$(SolutionDir)packages\Bridge.Min.16.7.1\build\Bridge.Min.targets"" Condition=""Exists('$(SolutionDir)packages\Bridge.Min.16.7.1\build\Bridge.Min.targets')"" />
  <Target Name=""EnsureNuGetPackageBuildImports"" BeforeTargets=""PrepareForBuild"">
    <PropertyGroup>
      <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>
    </PropertyGroup>
    <Error Condition=""!Exists('$(SolutionDir)packages\Bridge.Min.16.7.1\build\Bridge.Min.targets')"" Text=""$([System.String]::Format('$(ErrorText)', '$(SolutionDir)packages\Bridge.Min.16.7.1\build\Bridge.Min.targets'))"" />
    <Error Condition=""!Exists('..\packages\Bridge.Min.16.8.2\build\Bridge.Min.targets')"" Text=""$([System.String]::Format('$(ErrorText)', '..\packages\Bridge.Min.16.8.2\build\Bridge.Min.targets'))"" />
  </Target>
  <Import Project=""..\packages\Bridge.Min.16.8.2\build\Bridge.Min.targets"" Condition=""Exists('..\packages\Bridge.Min.16.8.2\build\Bridge.Min.targets')"" />
</Project>

");

            File.WriteAllText(WorkingDirectory + FileName, sb.ToString(), Encoding.UTF8);
        }
        #endregion
    }
}