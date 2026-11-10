namespace Yafa.Domain.Dtos.Admin;

public class AddTopLocationRequestDto
{
	public string City { get; set; } = string.Empty;
	public string LocationName { get; set; } = string.Empty;
	public string? Description { get; set; }
    public string ProfileImageBase64 { get; set; } = string.Empty;
    public string ProfileImageExtension { get; set; } = string.Empty;
	public string CreateBy { get; set; } = string.Empty;
}

public class UpdateTopLocationRequestDto
{
	public Guid LocationPublicId { get; set; }
	public string? City { get; set; }
	public string? LocationName { get; set; }
	public string? Description { get; set; }
    public string ProfileImageBase64 { get; set; } = string.Empty;
    public string ProfileImageExtension { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

public class AdminTopLocationDto
{
	public Guid LocationPublicId { get; set; }
	public string City { get; set; } = string.Empty;
	public string LocationName { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? ImagePath { get; set; }
	public string? ThumbImagePath { get; set; }
	public string? ImageName { get; set; }
	public int DisplayOrder { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedDate { get; set; }

	// Pagination meta
	public int TotalCount { get; set; }
	public int CurrentPage { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
}