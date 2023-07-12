namespace CountriesAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }

        public Country()
        {
            CountryCode = String.Empty;
            Name = String.Empty;
        }
    }
}
