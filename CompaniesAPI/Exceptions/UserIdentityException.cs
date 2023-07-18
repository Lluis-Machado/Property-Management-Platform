namespace CompaniesAPI.Exceptions;

public class UserIdentityException : Exception
{
    private const string ErrorMessage = "User Identity is null";

    public UserIdentityException()
        : base(ErrorMessage)
    {
    }
}