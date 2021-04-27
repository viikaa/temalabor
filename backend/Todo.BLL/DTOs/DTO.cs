namespace Todo.BLL
{
    public sealed record Column(int? Id, string Title){}
    public sealed record Todo(
        int? Id,
        string Title,
        string? Description,
        string Deadline,
        int Priority,
        int ColumnId);
}
