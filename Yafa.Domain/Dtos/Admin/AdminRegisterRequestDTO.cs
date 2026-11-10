using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.Admin
{
    public class AdminRegisterRequestDTO
    {
        public string RoleId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string AdminNotes { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string Salutation { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZIP { get; set; } = string.Empty;
        public string Lat { get; set; } = string.Empty;
        public string Lang { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string PhoneNo2 { get; set; } = string.Empty;
        public string DOB { get; set; } = string.Empty;
        public string AboutUs { get; set; } = string.Empty;

        public string ProfileImageBase64 { get; set; } = string.Empty;   // Base64 string
        public string ProfileImageExtension { get; set; } = string.Empty; // jpg / png
    }
}
