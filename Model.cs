class Todo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Completed { get; set; } = false;
}

record TodoSummaryDto(int Id, string Name, bool Completed);

record TodoCreateDto(string Name, string? Description);

record TodoUpdateDto(string Name, string? Description, bool Completed);