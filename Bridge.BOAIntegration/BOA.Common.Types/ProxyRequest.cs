using System;

namespace BOA.Common.Types
{
    [Serializable]
    public class ProxyRequest
    {
        #region Public Properties
        public string Key { get; set; }

        public RequestBase RequestBody { get; set; }

        public string RequestClass { get; set; }

        public bool? ShowProgress { get; set; }
        #endregion
    }
}