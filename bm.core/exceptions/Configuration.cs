namespace bm.core.exceptions;

public class ConfigurationException : Exception
{
    public ConfigurationException(string configItem)
    : base($"Missing configuration item {configItem}")
    {
    }
}
