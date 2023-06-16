using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TaxManagementAPI.Models;

namespace TaxManagement.Models
{
    public class Declarant: BaseModel
    {
        public string Name { get; set; }

        [JsonConstructor]
        public Declarant() {
            Name = String.Empty;
            CreatedByUser = string.Empty;
            LastUpdateByUser = string.Empty;
        }
    }

}
