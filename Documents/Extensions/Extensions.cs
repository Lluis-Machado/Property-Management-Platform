
namespace Documents.Extensions
{
    public static class Extensions
    {
        public static bool HasSpecialCharacters(this string stringToCheck)
        {
            int nbOfSpecialChar = stringToCheck.Count(p => !char.IsLetterOrDigit(p) && p != '-');
            return nbOfSpecialChar > 0;
        }
    }
}
