namespace Bridge.BOAIntegration.Injection
{
    class InjectInfo
    {
        #region Public Properties
        public string InitializerJSCode     { get; set; }
        public string JSData                { get; set; }
        public string JSDataInjectedVersion { get; set; }
        public string SourceJsFilePath      { get; set; }
        public string ViewTypeFullName      { get; set; }
        #endregion
    }
}