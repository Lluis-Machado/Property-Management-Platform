namespace ContactsAPI.Models
{
    public class ContactData
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Address { get; set; }

        public ContactData()
        {
            Email = string.Empty;
            PhoneNumber = string.Empty;
            MobilePhoneNumber = string.Empty;
            Address = string.Empty;
        }
    }
}
