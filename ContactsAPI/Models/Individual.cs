namespace ContactsAPI.Models
{
    public interface IIndividual
    {
        public enum IndividualType{
            Contact,
            AddressBilling,
            AddressDelivery,
            Other
        }
        public IndividualType Type { get; set; }
        public string Position { get; set; }

    }
}
