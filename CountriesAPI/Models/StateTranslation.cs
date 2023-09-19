namespace CountriesAPI.Models
{
    public class StateTranslation
    {
        public int Id { get; set; }
        public int StateId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
    }
}
