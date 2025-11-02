using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class WorldStateDto
{
  public List<CharacterDto> Characters { get; set; } = new();
  public List<EnemyDto> Enemies { get; set; } = new();
}
