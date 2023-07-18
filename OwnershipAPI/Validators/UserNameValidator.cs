using OwnershipAPI.Exceptions;

namespace OwnershipAPI.Validators
{
    public static class UserNameValidator
    {
        public static string GetValidatedUserName(string? username)
        {
            if (username == null) throw new UserIdentityException();
            return username;
        }
    }
}
