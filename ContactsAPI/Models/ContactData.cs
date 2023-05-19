namespace ContactsAPI.Models
{
    public class ContactData
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }

        public ContactData()
        {
            Email = String.Empty;
            PhoneNumber = String.Empty;
            MobilePhoneNumber = String.Empty;
        }
    }
}
