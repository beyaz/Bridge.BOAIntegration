namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    public class ComboBoxColumn
    {
        [Name("key")]
        public string Key { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("width")]
        public int Width { get;   set; }
        public string type { get; set; }
        
    }
}