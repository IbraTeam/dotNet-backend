using System;
using System.Collections.Generic;

namespace dotNetBackend.models.DbFirst;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Key> Keys { get; set; } = new List<Key>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
