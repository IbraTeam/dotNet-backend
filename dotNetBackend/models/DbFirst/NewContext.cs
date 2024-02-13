using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dotNetBackend.models.DbFirst;

public partial class NewContext : DbContext
{
    public NewContext()
    {
    }

    public NewContext(DbContextOptions<NewContext> options) : base(options)
    {
    }

    public virtual DbSet<Key> Keys { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Key>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("keys_pkey");

            entity.ToTable("keys");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Room)
                .HasMaxLength(255)
                .HasColumnName("room");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Keys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fkbu243bwm4nirm8fukfqos9dk9");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requests_pkey");

            entity.ToTable("requests");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DateTime)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("date_time");
            entity.Property(e => e.KeyId).HasColumnName("key_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PairNumber).HasColumnName("pair_number");
            entity.Property(e => e.Repeated).HasColumnName("repeated");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Key).WithMany(p => p.Requests)
                .HasForeignKey(d => d.KeyId)
                .HasConstraintName("fkb5hbhqly0ot03g0h4pxr0brmi");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk8usbpx9csc6opbjg1d7kvtf8c");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "uk_6dotkott2kjsp8vw4d0m25fb7").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(1000)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
