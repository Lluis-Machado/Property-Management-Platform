using TaxManagementAPI.Models;
using static ContactsAPI.Models.IIndividual;

namespace ContactsAPI.Models
{
    public class Contact :ICompany, IIndividual, IAuditable
    {
        public enum ContactType
        {
            Individual,
            Company
        }
        public Guid _id { get; set; }
        public Guid TenantId { get; set; }
        public ContactType Type { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public ContactData ContactData { get; set; }
        public string NIF { get; set; }
        public string WebSite { get; set; }
        public string[] Categories { get; set; }
        public Guid ParentId { get; set; }
        public string Individual_Position { get; set; }
        public IndividualType Individual_Type { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }


        IndividualType IIndividual.Type { 
            get => Individual_Type; 
            set => Individual_Type = value; 
        }

        string IIndividual.Position
        {
            get => Individual_Position;
            set => Individual_Position = value;
        }
    }
}
