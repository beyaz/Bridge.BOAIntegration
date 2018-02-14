namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    public class GridColumnInfo
    {
        #region Public Properties
        [Name("key")]
        public string Key { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("resizable")]
        public bool Resizable { get; set; }

        [Name("width")]
        public int Width { get; set; }
        #endregion
    }
}