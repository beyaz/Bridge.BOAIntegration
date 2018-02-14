using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    [IgnoreGeneric]
    public class GenericResponse<T> : ResponseBase
    {
        #region Public Properties
        [Name("value")]
        public T Value { get; set; }
        #endregion
    }
}