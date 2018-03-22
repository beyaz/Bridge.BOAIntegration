using System;
using Bridge;

namespace BOA.Common.Types
{
    [Serializable]
    public class ResponseBase : BOAMessageBase
    {
        #region Public Properties
        public string Key { get; set; }

        public Result[] Results { get; set; }

        public bool Success { get; set; }
        #endregion
    }
}