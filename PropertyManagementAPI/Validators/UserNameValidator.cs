using PropertiesAPI.Exceptions;

namespace PropertiesAPI.Validators
{
    public class UserNameValidator
    {
        public static string GetValidatedUserName(string? username)
        {
            if (username == null) throw new UserIdentityException();
            return username;
        }
    }
}
