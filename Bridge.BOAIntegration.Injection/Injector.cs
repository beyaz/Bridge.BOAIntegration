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

	Bridge.$BOAIntegration.BrowsePageInTypeScript = b_framework_1.BrowsePage;

	IncludeJs('Bridge.BOAIntegration.js');
	IncludeJs('Bridge.BOAIntegration.meta.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.meta.js');

	Bridge.BOAIntegration.$__webpack_require__ = __webpack_require__;
			
	var InheritBridgeClassFromTypeScriptClass = function(subClass,baseClass)
	{
		var  prototypes =  subClass.prototype;
			
		_inherits(subClass,baseClass);
				
		for(var p in prototypes)
		{
			subClass.prototype[p] = prototypes[p];
		}

        // for support bridge type system.
		subClass.prototype['$getType'] = function(){  return subClass; }
	}
			
	InheritBridgeClassFromTypeScriptClass( Bridge.BOAIntegration.BrowsePage, b_framework_1.BrowsePage );
	InheritBridgeClassFromTypeScriptClass( " + injectInfo.ViewTypeFullName + @" , Bridge.BOAIntegration.BrowsePage );
}

// <--- Injected Code ---

";

            InjectInitializerPart(injectInfo);

            injectInfo.JSData = injectInfo.JSDataInjectedVersion;



            injectInfo.JSCodeWillbeInject = @"

        // --- Injected Code --->
		_this.constructor  				= _b_framework_1$Browse;
		
		// force to reinitialize Bridge.net type information
		_this.constructor.$initMetaData = false;
		// <--- Injected Code ---

";

            InjectConstructorPart(injectInfo);
            injectInfo.JSData = injectInfo.JSDataInjectedVersion;

            InjectInheritancePart(injectInfo);

            File.WriteAllText(injectInfo.SourceJsFilePath, injectInfo.JSDataInjectedVersion);
        }

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