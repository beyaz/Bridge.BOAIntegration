using System.Collections.Generic;
using BOA.Common.Types;
using BOA.UI.Types;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.UI
{
    public class BDialogBox
    {
        public static DialogResponses Show(string message, DialogTypes dialogType, List<Result> resultList)
        {
            // TODO: fix error message
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");

            return DialogResponses.Ok;
        }

        public static DialogResponses Show(string message, DialogTypes dialogType, Result[] resultList)
        {
            // TODO: fix error message resultList :  readonly collection olmalı
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");

            return DialogResponses.Ok;
        }
    }
}