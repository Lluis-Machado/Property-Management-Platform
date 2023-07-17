using AccountingAPI.Exceptions;

namespace AccountingAPI.Validators
{
    public static class UserNameValidator
    {
        public static string GetValidatedUserName(string? username)
        {
            if (username is null) throw new UserIdentityException();
            return username;
        }
    }
}
