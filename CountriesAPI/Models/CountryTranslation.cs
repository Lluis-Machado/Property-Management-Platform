namespace CountriesAPI.Models
{
    public class CountryTranslation
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty ;
    }
}
