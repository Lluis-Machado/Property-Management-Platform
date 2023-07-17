
namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceDTO
    {
        public FieldInfo<string> RefNumber { get; set; }
        public FieldInfo<DateTime> Date { get; set; }
        public FieldInfo<string> Currency { get; set; }
        //public List<APInvoiceLineDTO> InvoiceLines { get; set; }

        public APInvoiceDTO()
        {
            //InvoiceLines = new List<APInvoiceLineDTO>();
        }
    }
}
