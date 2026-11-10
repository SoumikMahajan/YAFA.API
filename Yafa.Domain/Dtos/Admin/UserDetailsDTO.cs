namespace Yafa.Domain.Dtos.Admin
{
    public class UserDetailsDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZIP { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string PhoneNo2 { get; set; } = string.Empty;
        public string DOB { get; set; } = string.Empty;
        public string ProfileImagePath { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool? IsSocialUser { get; set; }
        public string Password { get; set; } = string.Empty;
        public int? TotalCount { get; set; } = 0;
    }
}
