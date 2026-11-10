namespace Yafa.Domain.Dtos.Admin;

public class BlockPropRequestDTO
{
    public Guid PropertyPublicId { get; set; }
    public string BlockDates { get; set; } = string.Empty;
    public string BlockReason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}


public class BlockPropDatesDetailsDTO
{
    public Guid UnavailablePublicId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string BlockReason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}