namespace ContactsAPI.Models
{
    public class Contact : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDay { get; set; }
        public string ResidentIn { get; set; }
        public string NIE { get; set; }
        public DateOnly NIEValidUntil { get; set; }
        public ContactData ContactData { get; set; }

        public Contact() { 
            FirstName = string.Empty; 
            LastName = string.Empty; 
            NIE = string.Empty;  
            BirthDay = new DateOnly();
            ResidentIn = string.Empty;
            NIEValidUntil = new DateOnly();
            ContactData = new ContactData();
        }
    }
}
