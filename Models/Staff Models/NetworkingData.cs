namespace StudentPortfolio.Models.Staff_Models
{
    public class NetworkingData
    {
        public ApplicationUser User { get; set; }
        public IList<IndustryContactLog> NetworkingContacts { get; set; }
        public IList<IndustryContactInfo> NetworkingContactInfo { get; set; }
        public IList<NetworkingEvent> NetworkingEvents { get; set; }
        public IList<NetworkingQuestion> NetworkingQuestions { get; set; }
    }
}
