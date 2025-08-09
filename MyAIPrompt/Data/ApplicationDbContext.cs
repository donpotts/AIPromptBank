using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAIPrompt.Models;
using MyAIPrompt.Shared.Models;

namespace MyAIPrompt.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AIPrompt> AIPrompt => Set<AIPrompt>();
    public DbSet<AITag> AITag => Set<AITag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AIPrompt>()
            .HasMany(x => x.AITag);
        modelBuilder.Entity<AITag>()
            .HasMany(x => x.AIPrompt);
    }
}
