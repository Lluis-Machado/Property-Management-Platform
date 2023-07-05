namespace CountriesAPI.Models
{
    public class State
    {
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string Name { get; set; }

        public State()
        {
            CountryCode = string.Empty;
            StateCode = string.Empty;
            Name = string.Empty;
        }
    }
}
