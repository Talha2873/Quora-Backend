public class Follow
{
    public int Id { get; set; }

    public int FollowerId { get; set; }   // me
    public int FollowingId { get; set; }  // user I follow
}