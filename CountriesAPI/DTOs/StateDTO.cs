namespace CountriesAPI.DTOs
{
    public class StateDTO
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string LanguageCode { get; set; }
        public string StateCode { get; set; }
        public string Name { get; set; }

        public StateDTO()
        {
            StateCode = String.Empty;
            Name = String.Empty;
            LanguageCode = "default";
        }
    }
}
