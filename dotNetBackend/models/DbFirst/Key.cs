namespace dotNetBackend.models.DbFirst;

public partial class Key
{
    public Guid Id { get; set; }

    public string Room { get; set; } = null!;

    public Guid? UserId { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual User? User { get; set; }
}
