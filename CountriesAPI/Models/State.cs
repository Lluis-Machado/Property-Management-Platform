namespace CountriesAPI.Models
{
    public class State
    {
        public int Id { get; set; }
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
