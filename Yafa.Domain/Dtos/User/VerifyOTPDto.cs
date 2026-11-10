namespace Yafa.Domain.Dtos.User
{
    public class VerifyOTPDto
    {
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}
