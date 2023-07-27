namespace CountriesAPI.Models
{
    public class State
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string StateCode { get; set; }
        public string Name { get; set; }

        public State()
        {
            StateCode = string.Empty;
            Name = string.Empty;
        }
    }
}
