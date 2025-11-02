using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class EnemyDto
{
  public Guid Id { get; set; }
  public string Type { get; set; } = string.Empty;
  public float X { get; set; }
  public float Y { get; set; }
  public int Health { get; set; }
}
