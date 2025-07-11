namespace CheeseGrater.Core.Domain.Constants;

public abstract class Scopes
{
  public const string List = nameof(List);
  public const string Read = nameof(Read);
  public const string Create = nameof(Create);
  public const string Edit = nameof(Edit);
  public const string Delete = nameof(Delete);

  public static List<string> All = new([List, Read, Create, Edit, Delete]);
}
