namespace bm.core.exceptions;

public class ListingsRequestException : Exception
{
    public ListingsRequestException(Exception ex)
    : base("Exception thrown whilst retrieving listigns from the listings provider", ex)
    {
    }
}