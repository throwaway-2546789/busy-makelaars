namespace bm.core.exceptions;

public class StatisticsProvisionException : Exception
{
    public StatisticsProvisionException(Exception ex)
    : base("Exception thrown whilst retrieving statistics from the stats provider", ex)
    {

    }
}