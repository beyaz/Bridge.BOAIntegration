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
if (!window['Bridge']) 
{    
    window.React = React; 
    

	var IncludeJs = function IncludeJs(url) 
	{
		$.ajax({
			url: url,
			dataType: 'script',
			async: false
		});
	};

	IncludeJs('bridge.js');
	IncludeJs('bridge.meta.js');
    IncludeJs('newtonsoft.json.js');

	if (!Bridge.$BOAIntegration) {
		Bridge.$BOAIntegration = 
        {
            $_extends: _extends        
        };
	}

	IncludeJs('Bridge.BOAIntegration.js');
	IncludeJs('Bridge.BOAIntegration.meta.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.meta.js');

	Bridge.BOAIntegration.$__webpack_require__ = __webpack_require__;
}
// <--- Injected Code ---

";

            

            InjectInitializerPart(injectInfo);

            injectInfo.JSData = injectInfo.JSDataInjectedVersion;



            injectInfo.JSCodeWillbeInject = @"

        // --- Injected Code --->
		_this.$DotNetVersion                    = new" + injectInfo.ViewTypeFullName + @"();
		_this.$DotNetVersion.$TypeScriptVersion = _this;
		_this.state.columns 					= _this.$DotNetVersion.$columns

        // forward some functions to .net version
        _this.onActionClick     = function(command)         {  this.$DotNetVersion.onActionClick(command);   }
        _this.render            = function()                {  this.$DotNetVersion.render();   }
        _this.proxyDidRespond   = function(proxyResponse)   {  this.$DotNetVersion.proxyDidRespond(proxyResponse);  }

		// <--- Injected Code ---
       

";

            InjectConstructorPart(injectInfo);

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
    }
}