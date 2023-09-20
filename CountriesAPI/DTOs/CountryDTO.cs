namespace CountriesAPI.DTOs
{
    public class CountryDTO
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }

        public CountryDTO()
        {
            CountryCode = string.Empty;
            Name = string.Empty;
            LanguageCode = "default";
        }
    }
}
