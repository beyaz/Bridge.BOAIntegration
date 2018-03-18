namespace System
{
    public class MissingMemberException : SystemException
    {
        #region Constructors
        public MissingMemberException(string message = null, Exception innerException = null) : base(message, innerException)
        {
        }
        #endregion
    }
}