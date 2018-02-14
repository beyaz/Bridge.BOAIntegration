using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.BOAIntegration.Injection
{
    class Injector
    {
        #region Public Methods
        public void InjectInheritancePart(InjectInfo injectInfo)
        {
            var lines = Split(injectInfo.JSData);

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Trim() == "}(b_framework_1.BrowsePage);")
                {
                    lines[i] = "}(" + injectInfo.ViewTypeFullName + ");";

                    injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);

                    return;
                }
            }

            throw new InvalidOperationException(nameof(InjectInheritancePart) + " not found.");
        }

        public virtual void InjectInitializerPart(InjectInfo injectInfo)
        {
            var lines = Split(injectInfo.JSData);

            var injectLocationIndex = FindInjectLocationIndex(lines);

            lines.Insert(injectLocationIndex, injectInfo.Code);

            injectInfo.JSDataInjectedVersion = string.Join(Environment.NewLine, lines);
        }
        #endregion

        #region Methods
        static int FindInjectLocationIndex(List<string> lines)
        {
            var isFirstFunctionFound = false;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Contains("__webpack_require__("))
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

        static List<string> Split(string injectInfoJsData)
        {
            return injectInfoJsData.Split(Environment.NewLine.ToCharArray()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
        #endregion
    }
}