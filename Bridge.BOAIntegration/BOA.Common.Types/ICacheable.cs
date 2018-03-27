namespace BOA.Common.Types
{
    public interface ICacheable
    {
        #region Public Properties
        bool DoNotUseCache { get; set; }
        #endregion

        #region Public Methods
        string GenerateCacheKey();
        #endregion
    }
}