using ContactsAPI.Models;
using System.Buffers;

namespace ContactsAPI.Models
{
    public class UpdateContactDTO 
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? ResidentIn { get; set; }
        public string? NIE { get; set; }
        public DateOnly? NIEValidUntil { get; set; }
        public ContactData? ContactData { get; set; }
        


    }
}
