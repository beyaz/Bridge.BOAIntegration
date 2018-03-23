


(function() {


    var $ = window.$;

    var IncludeJs = function(url) {
        $.ajax({
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

    var LoadedAssemblyNames = {};

    Bridge.$BOAIntegration =
    {
        $_extends: extendFunction,

        $Setup: function(__webpack_require__, React) {

            AssertParameterNotNull(__webpack_require__, '__webpack_require__');
            AssertParameterNotNull(React, 'React');

            Bridge.BOAIntegration.$__webpack_require__ = __webpack_require__;
            window.React = React;
        },       

        RequireDll: function(assemblyName) {
            AssertParameterNotNull(assemblyName, 'assemblyName');
            if (LoadedAssemblyNames[assemblyName]) {
                return;
            }

            $.ajax({ url: assemblyName+'.js',      dataType: 'script', async: false });
            $.ajax({ url: assemblyName+'.meta.js', dataType: 'script', async: false });

            LoadedAssemblyNames[assemblyName] = 1;

        }


    };

    IncludeJs('Bridge.BOAIntegration.js');
    IncludeJs('Bridge.BOAIntegration.meta.js');


})();


//# sourceURL=Bridge_BOAIntegration_Loader.js