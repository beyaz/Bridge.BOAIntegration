namespace Bridge.BOAIntegration.Injection
{
    class Program
    {
        #region Methods
        static void Main(string[] args)
        {
            var jsFileName   = args[0];
            var typeFullName = args[1];

            new Injector().Inject(new InjectInfo
            {
                SourceJsFilePath = @"D:\BOA\One\wwwroot\" + jsFileName,

                ViewTypeFullName = typeFullName
            });
        }
        #endregion
    }
}