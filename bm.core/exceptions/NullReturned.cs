namespace bm.core.exceptions;

public class NullResultException : Exception
{
    public NullResultException()
    : base("Unexpected null returned")
    {
    }
}
