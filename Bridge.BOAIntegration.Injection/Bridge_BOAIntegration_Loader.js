


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

    if (!Bridge.$BOAIntegration) {
        Bridge.$BOAIntegration =
        {
            $_extends: Object.assign ||
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
                }


        };
    }

    IncludeJs('Bridge.BOAIntegration.js');
    IncludeJs('Bridge.BOAIntegration.meta.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.js');
    IncludeJs('BOA.One.Office.CardGeneral.DebitCard.meta.js');


    Bridge.BOAIntegration.$Setup = function (__webpack_require__, React) {


        if (__webpack_require__ == null) {
            throw new Error('@parameter:__webpack_require__ is null');
        }
        if (React == null) {
            throw new Error('@parameter:React is null');
        }


        Bridge.BOAIntegration.$__webpack_require__ = __webpack_require__;
        window.React = React; 
    }


})();