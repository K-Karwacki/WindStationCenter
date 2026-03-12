using Domain.Entities;
using Domain.Entities.IoT;
using Domain.Interfaces.Utility;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        // For design-time (migrations), read connection string from environment variable
        // Falls back to localhost if not set
        var envHelper = new EnvHelper();

        var builder = new DbContextOptionsBuilder<MyDbContext>();
        builder.UseNpgsql(EnvHelper.LoadAndGetConnectionString(true));
        
        return new MyDbContext(builder.Options);
    }
}
public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Farm>  Farms => Set<Farm>();
    public DbSet<Telemetry>  Telemetries => Set<Telemetry>();
    public DbSet<Turbine> Turbines => Set<Turbine>();
    public DbSet<TelemetryAlert> TelemetryAlerts => Set<TelemetryAlert>();
    public DbSet<Command> Commands => Set<Command>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasMany(e => e.Commands)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserInternalId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // IoT: Farm, Turbine and Telemetry relationships
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id);

            // One Farm has many Turbines. Turbine does not declare FarmId in the CLR model,
            // so use a shadow FK named "FarmId" on the Turbine table.
            entity.HasMany(e => e.Turbines)
                .WithOne(t => t.Farm)
                .HasForeignKey("FarmId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Turbine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TurbineExternalId).IsUnique();
        });

        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Telemetry -> Farm (many telemetries belong to one farm)
            entity.HasOne<Farm>()
                .WithMany()
                .HasForeignKey(e => e.FarmInternalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Telemetry -> Turbine (many telemetries belong to one turbine)
            entity.HasOne<Turbine>()
                .WithMany()
                .HasForeignKey(e => e.TurbineInternalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for querying timeseries by farm, turbine and timestamp
            entity.HasIndex(e => new { e.FarmId, e.TurbineInternalId, Timestamp = e.Timestamp });
        });

        modelBuilder.Entity<TelemetryAlert>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne<Turbine>()
                .WithMany()
                .HasForeignKey(e => e.TurbineInternalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne<Turbine>()
                .WithMany()
                .HasForeignKey(e => e.TurbineInternalId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}