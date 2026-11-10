using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.User
{
	public class UpdateProfileImageDTO
	{
		public string UserId { get; set; } = string.Empty;
		public string ProfileImageBase64 { get; set; } = string.Empty; 
		public string ProfileImageExtension { get; set; } = string.Empty;
	}
}
