namespace QuoraBackend.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int Value { get; set; }
    }
}
