using CheeseGrater.RpgApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheeseGrater.RpgApi.Infrastructure.Data.Configuration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
  public void Configure(EntityTypeBuilder<Player> builder)
  {
    builder.Property(t => t.Username).IsRequired();

    // builder.OwnsOne(b => b.Colour);
  }
}
