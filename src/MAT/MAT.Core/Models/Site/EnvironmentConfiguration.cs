namespace MAT.Core.Models.Site
{
    public class EnvironmentConfiguration
    {
        public EnvironmentConfiguration(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}