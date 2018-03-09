using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bridge.BOAIntegration.Injection
{
    class Injector
    {
        #region Public Methods
        public void Inject(InjectInfo injectInfo)
        {
            var allText = File.ReadAllText(injectInfo.SourceJsFilePath);

            if (IsAlreadyInjected(allText))
            {
                return;
            }

            injectInfo.JSData = allText;

            injectInfo.JSCodeWillbeInject = @"
// --- Injected Code --->
    if (!window.Bridge)
    {

        $.ajax({url: 'Bridge_BOAIntegration_Loader.js',dataType: 'script',async: false});
        Bridge.BOAIntegration.$Setup(__webpack_require__,React);   
    }
// <--- Injected Code ---

";

            

            InjectInitializerPart(injectInfo);

            injectInfo.JSData = injectInfo.JSDataInjectedVersion;



            injectInfo.JSCodeWillbeInject = @"

        // --- Injected Code --->
            Bridge.BOAIntegration.$Connect_Typescript_And_Dotnet_Instances_for_BrowsePage(_this," + injectInfo.ViewTypeFullName + @");
		// <--- Injected Code ---
       

";

            InjectConstructorPart(injectInfo);

            injectInfo.JSData = injectInfo.JSDataInjectedVersion;


            injectInfo.JSCodeWillbeInject = " return this.$DotNetVersion.onActionClick(command); ";
            SetFirstStatementOfFunction(injectInfo, "function onActionClick(command)");
            injectInfo.JSData = injectInfo.JSDataInjectedVersion;


            injectInfo.JSCodeWillbeInject = " return this.$DotNetVersion.render(); ";
            SetFirstStatementOfFunction(injectInfo, "function render()");
            injectInfo.JSData = injectInfo.JSDataInjectedVersion;


            injectInfo.JSCodeWillbeInject = " return this.$DotNetVersion.proxyDidRespond(proxyResponse); ";
            SetFirstStatementOfFunction(injectInfo, "function proxyDidRespond(proxyResponse)");
            injectInfo.JSData = injectInfo.JSDataInjectedVersion;



            File.WriteAllText(injectInfo.SourceJsFilePath, injectInfo.JSDataInjectedVersion);
        }

        public virtual void InjectInitializerPart(InjectInfo injectInfo)
        {
            var lines = Split(injectInfo.JSData);

            var injectLocationIndex = FindInjectLocationIndex(lines);

            lines.Insert(injectLocationIndex, injectInfo.JSCodeWillbeInject);

            injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);
        }

        public virtual void InjectConstructorPart(InjectInfo injectInfo)
        {
            var lines = Split(injectInfo.JSData);

            var injectLocationIndex = FindInjectLocationIndex(lines, "_this.connect");

            lines.Insert(injectLocationIndex, injectInfo.JSCodeWillbeInject);

            injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);
        }

        
        #endregion

        #region Methods
        static int FindInjectLocationIndex(List<string> lines)
        {
            return FindInjectLocationIndex(lines, "__webpack_require__(");
        }

        static int FindInjectLocationIndex(List<string> lines,string lineContains)
        {
            var isFirstFunctionFound = false;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Contains(lineContains))
                {
                    isFirstFunctionFound = true;
                    continue;
                }

                if (isFirstFunctionFound)
                {
                    return i;
                }
            }

            throw new InvalidOperationException(nameof(FindInjectLocationIndex) + " not found.");
        }


        

        static bool IsAlreadyInjected(string allText)
        {
            return allText.Contains("Bridge.$BOAIntegration");
        }

        static List<string> Split(string injectInfoJsData)
        {
            return injectInfoJsData.Split(Environment.NewLine.ToCharArray()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
        #endregion

        public void SetFirstStatementOfFunction(InjectInfo injectInfo,string functionName)
        {
            var lines = Split(injectInfo.JSData);


            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                
                var isEqual = SpaceCaseInsensitiveComparator.Compare(line, "value: "+ functionName + " {");
                if (isEqual)
                {
                    lines[i] = line + injectInfo.JSCodeWillbeInject;

                    injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);
                    return;
                }


                isEqual = SpaceCaseInsensitiveComparator.Compare(line, "value: " + functionName + " {}");
                if (isEqual)
                {
                    lines[i] = line.Trim().RemoveFromEnd("}") + injectInfo.JSCodeWillbeInject + " } ";

                    injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);
                    return;
                }

            }

            throw new ArgumentException(functionName);
        }

    }
}






namespace Bridge.BOAIntegration.Injection
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    public static class SpaceCaseInsensitiveComparator
    {
        #region Public Methods
        public static bool Compare(string left, string right)
        {
            return Compare(left, right, CultureInfo.CurrentCulture);
        }

        public static bool Compare(string left, string right, CultureInfo cultureInfo)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return true;
                }

                return false;
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return ExceptChars(left.ToLower(cultureInfo), ExceptCharachters).Equals(ExceptChars(right.ToLower(cultureInfo), ExceptCharachters));
        }
        #endregion

        static readonly char[] ExceptCharachters = new[] { ' ', '\t', '\n', '\r' };

        public static string ToLowerAndClearSpaces(this string value)
        {
            if (value == null)
            {
                return null;

            }
            return ExceptChars(value.ToLower(), ExceptCharachters);
        }

        #region Methods
        static string ExceptChars(string str, char[] toExclude)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (!toExclude.Contains(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}