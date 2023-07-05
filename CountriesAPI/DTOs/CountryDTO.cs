namespace CountriesAPI.DTOs
{
    public class CountryDTO
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }

        public CountryDTO() 
        {
            CountryCode = string.Empty;
            Name = string.Empty;
        }
    }
}
