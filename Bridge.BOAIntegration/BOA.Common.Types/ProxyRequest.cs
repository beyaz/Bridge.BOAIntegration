using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    [IgnoreGeneric]
    public class ProxyRequest<T>
    {
        #region Public Properties
        [Name("key")]
        public string Key { get; set; }

        [Name("requestBody")]
        public T RequestBody { get; set; }

        [Name("requestClass")]
        public string RequestClass { get; set; }

        [Name("showProgress")]
        public bool? ShowProgress { get; set; }
        #endregion
    }
}