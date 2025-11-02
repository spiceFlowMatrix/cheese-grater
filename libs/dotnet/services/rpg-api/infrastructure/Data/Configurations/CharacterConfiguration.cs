using CheeseGrater.RpgApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheeseGrater.RpgApi.Infrastructure.Data.Configuration;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
  public void Configure(EntityTypeBuilder<Character> builder)
  {
    // Primary Key
    builder.Property(t => t.Name).IsRequired();

    // Flatten Position value object
    builder.OwnsOne(
      c => c.LastKnownPosition,
      pos =>
      {
        pos.Property(p => p.X).HasColumnName("PosX").HasColumnType("real").IsRequired();

        pos.Property(p => p.Y).HasColumnName("PosY").HasColumnType("real").IsRequired();
      }
    );
  }
}
