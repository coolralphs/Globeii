using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Core.Models.Enums;
using TailwindMauiBlazorApp.Shared.Services;

namespace TailwindMauiBlazorApp.Core.Data;

public class AppDbContext : DbContext
{
    //public AppDbContext(DbContextOptions<AppDbContext> options)
    //    : base(options) { }
    //private readonly Guid _currentUserId;
    //private readonly ICurrentUserService _currentUserService;
    //public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
    //   : base(options)
    //{
    //    _currentUserService = currentUserService;
    //}
    public Guid? CurrentUserId { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
       : base(options)
    {
        // Read CurrentUserId from appsettings.json
        //_currentUserId = configuration.GetValue<Guid>("CurrentUserId");
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();
    public DbSet<ItineraryAccomodation> ItineraryAccomodations => Set<ItineraryAccomodation>();
    public DbSet<ItineraryPlace> ItineraryPlaces => Set<ItineraryPlace>();
    public DbSet<ItineraryReservation> ItineraryReservations => Set<ItineraryReservation>();
    public DbSet<Place> Places => Set<Place>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert table and column names to snake_case globally
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()));
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }

        // Rename tables
        modelBuilder.Entity<AppUser>().ToTable("app_user");
        modelBuilder.Entity<Itinerary>().ToTable("itinerary");
        modelBuilder.Entity<ItineraryAccomodation>().ToTable("itinerary_accomodation");
        modelBuilder.Entity<ItineraryPlace>().ToTable("itinerary_place");
        modelBuilder.Entity<ItineraryReservation>().ToTable("itinerary_reservation");
        modelBuilder.Entity<Place>().ToTable("place");
        modelBuilder.Entity<UserLogin>().ToTable("user_login");

        modelBuilder.Entity<AppUser>()
            .Property(u => u.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(i => i.Id);

            // Creator relationship
            entity.HasOne(i => i.Creator)
                .WithMany() // or .WithMany(u => u.CreatedItineraries)
                .HasForeignKey(i => i.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Updator relationship
            entity.HasOne(i => i.Updator)
                .WithMany() // or .WithMany(u => u.UpdatedItineraries)
                .HasForeignKey(i => i.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Dates
            entity.Property(i => i.StartDate).HasColumnType("date");
            entity.Property(i => i.EndDate).HasColumnType("date");

            // CreatedAt default
            entity.Property(i => i.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // UpdatedAt nullable
            entity.Property(i => i.UpdatedAt)
                .IsRequired(false); // Nullable
        });

        modelBuilder.Entity<ItineraryAccomodation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartTime).HasColumnType("time");
            entity.Property(e => e.EndTime).HasColumnType("time");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(i => i.UpdatedAt)
                .IsRequired(false); // Nullable

            entity.Property(e => e.UserRating)
                .HasConversion<int?>();

            entity.Property(e => e.Status)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(20)
                .HasDefaultValue(ItineraryPlaceStatus.Planned);

            // Creator relationship
            entity.HasOne(ip => ip.Creator)
                .WithMany()
                .HasForeignKey(ip => ip.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Updator relationship
            entity.HasOne(ip => ip.Updator)
                .WithMany()
                .HasForeignKey(ip => ip.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Place relationship
            entity.HasOne(ip => ip.Place)
                .WithMany()
                .HasForeignKey(ip => ip.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Itinerary relationship
            entity.HasOne(ip => ip.Itinerary)
                .WithMany(i => i.ItineraryAccomodations)
                .HasForeignKey(ip => ip.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(p => p.StartTime);

        });

        modelBuilder.Entity<ItineraryPlace>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartTime).HasColumnType("time");
            entity.Property(e => e.EndTime).HasColumnType("time");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(i => i.UpdatedAt)
                .IsRequired(false); // Nullable

            entity.Property(e => e.UserRating)
                .HasConversion<int?>();

            entity.Property(e => e.Status)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(20)
                .HasDefaultValue(ItineraryPlaceStatus.Planned);

            // Creator relationship
            entity.HasOne(ip => ip.Creator)
                .WithMany()
                .HasForeignKey(ip => ip.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Updator relationship
            entity.HasOne(ip => ip.Updator)
                .WithMany()
                .HasForeignKey(ip => ip.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Place relationship
            entity.HasOne(ip => ip.Place)
                .WithMany()
                .HasForeignKey(ip => ip.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Itinerary relationship
            entity.HasOne(ip => ip.Itinerary)
                .WithMany(i => i.ItineraryPlaces)
                .HasForeignKey(ip => ip.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(p => p.StartTime);

        });

        modelBuilder.Entity<ItineraryReservation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartTime).HasColumnType("time");
            entity.Property(e => e.EndTime).HasColumnType("time");
            entity.Property(e => e.BookingDate).HasColumnType("date");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .IsRequired(false); // Nullable

            entity.Property(e => e.Status)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(20)
                .HasDefaultValue(ItineraryReservationStatus.Confirmed);

            entity.Property(e => e.ReservationType)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(30);

            entity.Property(e => e.ReservationSubType)
                .HasConversion<string>() // store enum as string
                 .HasMaxLength(30)
                .IsRequired(false);

            // Creator relationship
            entity.HasOne(ir => ir.Creator)
                .WithMany()
                .HasForeignKey(ir => ir.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Updator relationship
            entity.HasOne(ir => ir.Updator)
                .WithMany()
                .HasForeignKey(ir => ir.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Place relationship
            entity.HasOne(ir => ir.Place)
                .WithMany()
                .HasForeignKey(ir => ir.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Departure place relationship
            entity.HasOne(ir => ir.DeparturePlace)
                .WithMany()
                .HasForeignKey(ir => ir.DeparturePlaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Destination place relationship
            entity.HasOne(ir => ir.DestinationPlace)
                .WithMany()
                .HasForeignKey(ir => ir.DestinationPlaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Itinerary relationship
            entity.HasOne(ir => ir.Itinerary)
                .WithMany(i => i.ItineraryReservations)
                .HasForeignKey(ir => ir.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.HasMeal)
                .HasDefaultValue(false);

        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OpeningHoursJson)
                .HasColumnType("jsonb");

            entity.Property(e => e.Types)
                .HasColumnType("text[]");

            entity.Property(e => e.Lat).HasPrecision(9, 6);
            entity.Property(e => e.Lng).HasPrecision(9, 6);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(ul => ul.Id);

            // Configure relationship
            entity.HasOne(ul => ul.AppUser)
                  .WithMany(u => u.Logins)
                  .HasForeignKey(ul => ul.AppUserId);
            // Unique index to prevent duplicate logins
            entity.HasIndex(ul => new { ul.Provider, ul.ProviderUserId })
                  .IsUnique();

            entity.Property(ul => ul.Provider).HasMaxLength(100).IsRequired();
            entity.Property(ul => ul.ProviderUserId).HasMaxLength(200).IsRequired();
        });

    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        //    var currentUserId = await AppUsers
        //.Where(u => u.Email == "metalpunk007@gmail.com")
        //.Select(u => u.Id)
        //.FirstOrDefaultAsync(cancellationToken);

        //var currentUserId = _currentUserService.UserId ?? Guid.Empty;


        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Itinerary>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = CurrentUserId.Value;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                if (entry.Entity.UpdatedBy == Guid.Empty)
                {
                    entry.Entity.UpdatedBy = CurrentUserId.Value;
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<ItineraryPlace>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                if (entry.Entity.CreatedBy == Guid.Empty)
                {
                    entry.Entity.CreatedBy = CurrentUserId.Value;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                if (entry.Entity.UpdatedBy == Guid.Empty)
                {

                    entry.Entity.UpdatedBy = CurrentUserId.Value;
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<ItineraryAccomodation>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = CurrentUserId.Value;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = CurrentUserId.Value;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ItineraryReservation>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;

                if (entry.Entity.CreatedBy == Guid.Empty)
                {
                    entry.Entity.CreatedBy = CurrentUserId.Value;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                if (entry.Entity.UpdatedBy == Guid.Empty)
                {
                    entry.Entity.UpdatedBy = CurrentUserId.Value;
                }
            }
        }
    }

    // Helper method for snake_case conversion
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var builder = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0) builder.Append('_');
                builder.Append(char.ToLower(c));
            }
            else
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }
}