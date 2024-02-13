﻿namespace dotNetBackend.models.DbFirst;

public partial class Request
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Name { get; set; } = null!;
    public short PairNumber { get; set; }
    public bool Repeated { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Guid? UserId { get; set; }
    public Guid? KeyId { get; set; }
    public virtual Key? Key { get; set; }
    public virtual User? User { get; set; }
}