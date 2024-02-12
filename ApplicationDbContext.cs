using Microsoft.EntityFrameworkCore;

namespace Api;

public abstract class AuditableEntity
{
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}

public class Container1 : AuditableEntity
{
    public Guid Id { get; set; }
    public string Container1Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class Foo
{
    public string Bar { get; set; } = default!;
}

public class Container2 : AuditableEntity
{
    // Works when the Id isn't present
    public Guid Id { get; set; }
    public string Container2Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public Foo Foo { get; set; } = default!;
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseCosmos(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                "Daily")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Container1>().ToContainer("Container1")
           .HasPartitionKey(i => i.Container1Id);

        modelBuilder.Entity<Container1>().Property(i => i.Id)
            .HasConversion<string>();

        // Hangs here
        modelBuilder.Entity<Container2>().ToContainer("Container2")
           .HasPartitionKey(i => i.Container2Id);

        modelBuilder.Entity<Container2>().Property(i => i.Id)
            .HasConversion<string>();


        base.OnModelCreating(modelBuilder);
    }
}

