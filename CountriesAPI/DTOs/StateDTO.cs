namespace CountriesAPI.DTOs
{
    public class StateDTO
    {
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string Name { get; set; }

        public StateDTO()
        {
            CountryCode = String.Empty;
            StateCode = String.Empty;
            Name = String.Empty;
        }
    }
}
