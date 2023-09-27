namespace MessagingContracts
{
    public class MessageContract
    {
        public string Payload { get; set; } = string.Empty;

        public string Sender { get; set; } = string.Empty;
        public string Destination { get; set;} = string.Empty;
    }
}