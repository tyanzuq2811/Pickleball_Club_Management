namespace PCM.Application.DTOs.News;

public class NewsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class NewsCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
}

public class NewsUpdateDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? IsPinned { get; set; }
}
