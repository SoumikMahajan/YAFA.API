namespace Yafa.Domain.Dtos.Admin;

public class FeesTypeDTO
{
    public Guid FeePublicId { get; set; }
    public string FeeName { get; set; } = string.Empty;
    public string FeeDescription { get; set; } = string.Empty;
    public string FeeCategory { get; set; } = string.Empty;
    public decimal? FeeValue { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsActive { get; set; }
}

public class AdminAddFeeRequestDto
{
    public string FeeName { get; set; } = string.Empty;
    public string FeeDescription { get; set; } = string.Empty;
    public string FeeCategory { get; set; } = "FLAT";  // FLAT | PERCENTAGE | PER_NIGHT
    public decimal FeeValue { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class AdminUpdateFeeTypeDTO
{
    public Guid FeePublicId { get; set; }
    public string? FeeName { get; set; }
    public string? FeeDescription { get; set; }
    public string? FeeCategory { get; set; }
    public decimal? FeeValue { get; set; }
    public bool? IsMandatory { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}
