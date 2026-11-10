using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos
{
    public class LoginResponseDTO
    {
        public LoginResponseUserDetailsDTO UserDetails { get; set; } = new ();

        public string JwtTokenId { get; set; } = string.Empty;
    }

    public class LoginResponseUserDetailsDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string LastLoginDate { get; set; } = string.Empty;
        public string MainProfileImagePath { get; set; } = string.Empty;
        public string ThumImagePath { get; set; } = string.Empty;
        public string AboutUs { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
    }
}
