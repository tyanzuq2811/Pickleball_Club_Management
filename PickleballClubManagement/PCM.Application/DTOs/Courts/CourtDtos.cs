namespace PCM.Application.DTOs.Courts;

public class CourtDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class CourtCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CourtUpdateDto
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
}
