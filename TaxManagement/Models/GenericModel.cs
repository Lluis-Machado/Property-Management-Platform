namespace TaxManagementAPI.Models
{
    [Serializable]
    public class GenericModel
    {

        // JSON Structure according to documentation
        // https://www.agenciatributaria.es/static_files/AEAT_Desarrolladores/EEDD/General/EspecificacionesServiciosDeclaracionesAEAT_ES.pdf

        public string? MODELO { get; set; }
        public string? EJERCICIO { get; set; }
        public string? PERIODO { get; set; }
        public string? NRC { get; set; }
        public string? IDI { get; set; }
        public string? F01 { get; set; }
        public string FIR { get; } = "FirmaBasica";
        public string? FIRNIF { get; set; }
        public string? FIRNOMBRE { get; set; }

    }
}