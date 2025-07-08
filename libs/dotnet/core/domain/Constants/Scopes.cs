namespace CheeseGrater.Core.Domain.Constants;

public abstract class Scopes
{
  public const string List = nameof(List);
  public const string View = nameof(View);
  public const string Create = nameof(Create);
  public const string Edit = nameof(Edit);
  public const string Delete = nameof(Delete);

  public static List<string> All = new([List, View, Create, Edit, Delete]);
}
