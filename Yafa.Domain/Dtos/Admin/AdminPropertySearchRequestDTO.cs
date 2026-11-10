using System.ComponentModel;

namespace Yafa.Domain.Dtos.Admin
{
	public class AdminPropertySearchRequestDTO
	{
		[DefaultValue(10)]
		public int PageSize { get; set; } = 10;
		[DefaultValue(1)]
		public int PageNumber { get; set; } = 1;
		public string SearchText { get; set; } = string.Empty;
		public bool? IsActive { get; set; }
	}
}
