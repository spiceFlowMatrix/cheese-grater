using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseGrater.RpgApi.Domain.ValueObjects;

namespace CheeseGrater.RpgApi.Application.Common.Models;

public class CharacterState
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Position Position { get; set; } = new(0, 0);
  public float Speed { get; set; } = 5f;
  public int Health { get; set; } = 100;
}
