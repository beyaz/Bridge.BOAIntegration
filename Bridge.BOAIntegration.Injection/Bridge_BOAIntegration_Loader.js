


(function() {


    var IncludeJs = function(url) {
        window.$.ajax({
            url: url,
            dataType: 'script',
            async: false
        });
    };

    IncludeJs('bridge.js');
    IncludeJs('bridge.meta.js');
    IncludeJs('newtonsoft.json.js');

    var Bridge = window.Bridge;


    var extendFunction = Object.assign ||
        function(target) {
            for (var i = 1; i < arguments.length; i++) {
                var source = arguments[i];
                for (var key in source) {
                    if (Object.prototype.hasOwnProperty.call(source, key)) {
                        target[key] = source[key];
                    }
                }
            }
            return target;
        };


    var AssertParameterNotNull = function(parameter, parameterName) {
        if (parameter == null) {
            throw new Error('@parameter:' + parameterName + ' is null');
        }
    }


    Bridge.$BOAIntegration =
    {
        $_extends: extendFunction,

        $Setup: function(__webpack_require__, React) {

            AssertParameterNotNull(__webpack_require__, '__webpack_require__');
            AssertParameterNotNull(React, 'React');

            Bridge.BOAIntegration.$__webpack_require__ = __webpack_require__;
            window.React = React;
        },

        $Connect_Typescript_And_Dotnet_Instances_for_BrowsePage: function(typescriptWrittenInstance,
            bridgeWrittenClassConstructor) {

            AssertParameterNotNull(bridgeWrittenClassConstructor, 'bridgeWrittenClassConstructor');

            typescriptWrittenInstance.$DotNetVersion = new bridgeWrittenClassConstructor();
            typescriptWrittenInstance.$DotNetVersion.$TypeScriptVersion = typescriptWrittenInstance;
            typescriptWrittenInstance.state.columns = typescriptWrittenInstance.$DotNetVersion.$columns;
        }


    };

    IncludeJs('Bridge.BOAIntegration.js');
    IncludeJs('Bridge.BOAIntegration.meta.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.meta.js');


})();