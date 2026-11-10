using Azure.Core;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Security.Cryptography;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.User;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Controllers.User
{
    /// <summary>
    /// Provides API endpoints for user authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserAuthController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;
        private readonly IJwtService _jwtService;
        private readonly IFileRepo _fileService;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the UserAuthController class with the specified services and configuration.
        /// </summary>
        /// <param name="logService">The logging service used to record application events and errors.</param>
        /// <param name="unitOfWork">The unit of work instance that manages database transactions and repositories.</param>
        /// <param name="responseService">The service responsible for formatting and standardizing API responses.</param>
        /// <param name="jwtService">The service used to generate and validate JSON Web Tokens for authentication.</param>
        /// <param name="fileService">The file repository service used for file management operations.</param>
        /// <param name="httpClientFactory">The factory used to create HttpClient instances for making HTTP requests.</param>
        public UserAuthController(ILogService logService, IUnitOfWork unitOfWork, ApiResponseService responseService, IJwtService jwtService, IFileRepo fileService, IHttpClientFactory httpClientFactory)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
            _jwtService = jwtService;
            _fileService = fileService;
            _httpClient = httpClientFactory.CreateClient("GoogleImages");
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and returns a response containing user details and a JWT
        /// token if authentication is successful.
        /// </summary>
        /// <remarks>This endpoint does not require authentication. The JWT token returned can be used for
        /// subsequent authenticated requests. If the credentials are invalid, a generic error message is returned to
        /// avoid disclosing sensitive information.</remarks>
        /// <param name="model">The login credentials and options for the user attempting to sign in. Must include a valid email and
        /// password. The RememberMe property determines the token's expiration behavior.</param>
        /// <returns>An ActionResult containing an APIResponse with user details and a JWT token if authentication succeeds;
        /// otherwise, an error response indicating invalid credentials.</returns>
        [AllowAnonymous]
        [HttpPost("user-login")]
        public async Task<ActionResult<APIResponse>> UserLogin([FromBody] LoginRequestDTO model)
        {

            Encryption eobj = new();
            string encryptedPassword = eobj.Encrypt_final(model.Password);
            model.Password = encryptedPassword;

            var response = await _unitOfWork.DapperRepo.UserLoginAsync(model);
            if (response == null)
                return _responseService.InternalServerResponse("Invalid Email or Password.");

            var userClaims = new UserClaims
            {
                Id = response.UserId ?? "",
                Email = response.Email ?? "",
                Name = $"{response.FirstName} {response.LastName}",
                Roles = response.RoleName,
                RolesId = response.RoleId,
                ProfileImagePath = response.MainProfileImagePath,
                ThumbImagePath = response.ThumImagePath
            };

            var token = _jwtService.GenerateToken(userClaims, model.RememberMe);

            var result = new LoginResponseDTO()
            {
                UserDetails = response,
                JwtTokenId = token
            };

            return _responseService.SuccessResponse(result);

        }

        /// <summary>
        /// Registers a new user account using the provided registration details.
        /// </summary>
        /// <remarks>This endpoint is accessible without authentication. The registration process includes
        /// formatting user names and encrypting the password before storing the user information. Logging is performed
        /// for both successful and failed registration attempts.</remarks>
        /// <param name="model">An object containing the user's registration information, including personal details and credentials. Must
        /// not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ActionResult with an
        /// APIResponse indicating the outcome of the registration. Returns a success response if registration is
        /// successful; otherwise, returns a bad request response.</returns>
        [AllowAnonymous]
        [HttpPost("user-registration")]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterRequestDTO model)
        {

            var UserId = Guid.NewGuid().ToString();
            var UserDetailsId = Guid.NewGuid().ToString();

            var defaultRole = "9840A470-DB04-4AFD-80D2-39A19319CA9A";
            var allowedRoles = new[]
            {
                defaultRole,
                "DD33AAE3-81E3-413B-89DD-1B3DA159903D"
            };

            if (string.IsNullOrEmpty(model.RoleId) || !allowedRoles.Contains(model.RoleId))
            {
                model.RoleId = defaultRole;
            }

            model.FirstName = char.ToUpper(model.FirstName[0]) + model.FirstName.Substring(1).ToLower();
            if (!string.IsNullOrEmpty(model.MiddleName))
            {
                model.MiddleName = char.ToUpper(model.MiddleName[0]) + model.MiddleName.Substring(1).ToLower();
            }
            model.LastName = char.ToUpper(model.LastName[0]) + model.LastName.Substring(1).ToLower();

            Encryption eobj = new();
            string encryptedPassword = eobj.Encrypt_final(model.Password);
            model.Password = encryptedPassword;

            var imageUploadReult = new ImageUploadResultDTO();

            var response = await _unitOfWork.DapperRepo.RegisterAsync(model, imageUploadReult, UserId, UserDetailsId);

            if (response <= 0)
            {
                await _logService.LogCustomAsync($"Registration Failed On UserId {UserId} and UserDetailsId {UserDetailsId}");
                return _responseService.BadRequestResponse("Registration failed.");
            }

            await _logService.LogCustomAsync($"Registration Succesfully On UserId {UserId} and UserDetailsId {UserDetailsId}");

            return _responseService.SuccessResponse("Registration Succesfully");
        }

        /// <summary>
        /// Handles a password reset request by generating and sending a one-time password (OTP) to the user's email
        /// address.
        /// </summary>
        /// <remarks>The OTP is valid for 5 minutes. This endpoint does not require
        /// authentication.</remarks>
        /// <param name="request">The password reset request data containing the user's email address.</param>
        /// <returns>An API response indicating the result of the password reset request. Returns a success response if the OTP
        /// is sent, a not found response if the user does not exist, or an internal server error response if the email
        /// could not be sent.</returns>
        [AllowAnonymous]
        [HttpPost("user-fotgot-password-request")]
        public async Task<ActionResult<APIResponse>> ForgotPassWordRequest([FromBody] ForgotPasswordRequestDto request)
        {

            // Generate 6-digit OTP
            string otp = GenerateOTP();

            var response = await _unitOfWork.DapperRepo.ForgotPasswordRequestAsync(
                request.Email,
                otp,
                DateTime.UtcNow.AddMinutes(5) // 5 minutes expiry
            );

            if (response <= 0)
                return _responseService.NotFoundResponse("User not found with this email.");

            string subject = "Password Reset OTP";
            string emailBody = GenerateOTPEmailTemplate(otp);


            bool emailSent = await _unitOfWork.EmailRepo.SendMailAsync(request.Email, subject, null, null, emailBody);

            if (!emailSent)
            {
                await _logService.LogCustomAsync($"Failed to send OTP email to {request.Email}");
                return _responseService.InternalServerResponse("Failed to send email. Please try again.");
            }

            return _responseService.SuccessResponse("OTP has been sent to your email. Valid for 5 minutes.");
        }

        /// <summary>
        /// Verifies a one-time password (OTP) for a user's password reset request and issues a reset token if the OTP
        /// is valid.
        /// </summary>
        /// <remarks>This endpoint does not require authentication. The reset token returned is valid for
        /// 30 minutes and should be used to complete the password reset process.</remarks>
        /// <param name="request">An object containing the user's email address and the OTP to be verified. The email must correspond to a
        /// pending password reset request, and the OTP must be valid and unexpired.</param>
        /// <returns>An API response containing a reset token if the OTP is verified successfully; otherwise, a response
        /// indicating that the OTP is invalid or expired.</returns>
        [AllowAnonymous]
        [HttpPost("user-verify-otp")]
        public async Task<ActionResult<APIResponse>> VerifyOTP([FromBody] VerifyOTPDto request)
        {
            var response = await _unitOfWork.DapperRepo.VerifyForgotPasswordOtpAsync(request.Email, request.OTP);

            if (response <= 0)
                return _responseService.BadRequestResponse("Invalid or expired OTP.");

            string resetToken = GenerateResetToken();

            await _unitOfWork.DapperRepo.StoreResetTokenAsync(request.Email, resetToken, DateTime.UtcNow.AddMinutes(30));

            return _responseService.SuccessResponse(new
            {
                Message = "OTP verified successfully.",
                Token = resetToken
            });
        }

        /// <summary>
        /// Resets a user's password using the provided reset token and new password.
        /// </summary>
        /// <remarks>This endpoint does not require authentication. Ensure that the reset token is
        /// obtained through a secure password reset workflow. The new password is encrypted before being
        /// stored.</remarks>
        /// <param name="request">An object containing the reset token and the new password to set for the user. The reset token must be valid
        /// and not expired.</param>
        /// <returns>An API response indicating the result of the password reset operation. Returns a success response if the
        /// password is reset successfully; otherwise, returns a bad request response if the token is invalid or
        /// expired.</returns>
        [AllowAnonymous]
        [HttpPost("user-reset-password")]
        public async Task<ActionResult<APIResponse>> UserResetPassword([FromBody] ResetPasswordDto request)
        {
            Encryption eobj = new();
            string encryptedPassword = eobj.Encrypt_final(request.NewPassword);

            var response = await _unitOfWork.DapperRepo.ResetPasswordAsync(request.ResetToken, encryptedPassword);

            if (response <= 0)
                return _responseService.BadRequestResponse("Invalid or expired token.");

            return _responseService.SuccessResponse("Password has been reset successfully.");
        }

        /// <summary>
        /// Resends a one-time password (OTP) to the user's email address for password reset purposes.
        /// </summary>
        /// <remarks>This endpoint enforces rate limiting to prevent excessive OTP resend requests. The
        /// new OTP is valid for 5 minutes from the time it is generated.</remarks>
        /// <param name="request">An object containing the user's email address and any additional information required to resend the OTP.</param>
        /// <returns>An ActionResult containing an APIResponse that indicates the result of the resend operation. Returns a
        /// success response if the OTP is resent successfully; otherwise, returns an error response with details.</returns>
        [AllowAnonymous]
        [HttpPost("user-resend-otp")]
        public async Task<ActionResult<APIResponse>> ResendOTP([FromBody] ResendOTPDTO request)
        {
            var validationCheck = await _unitOfWork.DapperRepo.CheckResendOTPRateLimitAsync(request.Email);

            if (validationCheck <= 0)
                return _responseService.InternalServerResponse("something went wromng");

            string newOtp = GenerateOTP();

            var response = await _unitOfWork.DapperRepo.ResendOTPAsync(
                request.Email,
                newOtp,
                DateTime.UtcNow.AddMinutes(5) // 5 minutes expiry
            );

            if (response <= 0)
                return _responseService.BadRequestResponse("Unable to resend OTP. Please try forgot password again.");

            string subject = "Password Reset OTP";
            string emailBody = GenerateOTPEmailTemplate(newOtp);


            bool emailSent = await _unitOfWork.EmailRepo.SendMailAsync(request.Email, subject, null, null, emailBody);

            if (!emailSent)
            {
                await _logService.LogCustomAsync($"Failed to send OTP email to {request.Email}");
                return _responseService.InternalServerResponse("Failed to send email. Please try again.");
            }

            return _responseService.SuccessResponse("New OTP has been sent to your email. Valid for 5 minutes.");
        }

		//private string GenerateOTP()
		//{
		//    Random random = new Random();
		//    return random.Next(100000, 999999).ToString();
		//}

		

        /// <summary>
        /// Uploads and updates the user's profile image using the provided base64-encoded image data.
        /// </summary>
        /// <remarks>The request must include a valid user ID and a base64-encoded image. The method
        /// returns a not found response if the user does not exist, or an error response if the image upload or update
        /// fails.</remarks>
        /// <param name="request">An object containing the user ID, base64-encoded profile image, and image extension to be used for updating
        /// the user's profile image.</param>
        /// <returns>An API response indicating the result of the profile image upload and update operation. Returns a success
        /// response if the image is updated successfully; otherwise, returns an error response describing the failure.</returns>
        [HttpPost("user-upload-profile-image")]
        public async Task<ActionResult<APIResponse>> UserUploadProfileImage([FromBody] UpdateProfileImageDTO request)
        {

            var userDetails = await _unitOfWork.DapperRepo.GetUserDetailsByIdAsync(request.UserId);

            if (userDetails == null)
                return _responseService.NotFoundResponse("User not found.");

            var imageUploadReult = await _fileService.SaveBase64ImageAsync(
                    new Base64ImageSaveRequestDTO
                    {
                        Base64Image = request.ProfileImageBase64,
                        Extension = request.ProfileImageExtension ?? "jpg",
                        FileName = $"{request.UserId}_{userDetails.FirstName}_{userDetails.LastName}",
                        ImageFolderPath = "UploadFiles/ProfilePic",
                        ThumbnailFolderPath = "ThumbnailImages/TNProfilePic",
                    }
            );

            if (imageUploadReult == null)
                return _responseService.InternalServerResponse("Image upload failed. Please try again.");

            var response = await _unitOfWork.DapperRepo.UpdateUserProfileImageAsync(request.UserId, imageUploadReult);

            if (response <= 0)
                return _responseService.InternalServerResponse("Failed to update profile image. Please try again.");

            return _responseService.SuccessResponse("Profile image updated successfully.");
        }

        /// <summary>
        /// Retrieves detailed information for a user specified by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose details are to be retrieved. Must not be an empty GUID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ActionResult with an
        /// APIResponse holding the user details if found; returns a bad request response if the userId is invalid, or a
        /// not found response if the user does not exist.</returns>
        [HttpGet("user-details/{userId}")]
        public async Task<ActionResult<APIResponse>> GetUserDetailsByUserId([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
                return _responseService.BadRequestResponse("Invalid User Id.");

            var response = await _unitOfWork.DapperRepo.GetUserDetailsByUserPublicIdAsync(userId);
            if (response == null)
                return _responseService.NotFoundResponse("User not found.");
            return _responseService.SuccessResponse(response);
        }

        /// <summary>
        /// Authenticates a user using social login credentials and returns a response containing user details and a JWT
        /// token.
        /// </summary>
        /// <remarks>This endpoint allows users to log in using supported social providers such as Google.
        /// The method validates the provided ID token and creates or updates the user account as needed. The response
        /// includes a JWT token for subsequent authenticated requests.</remarks>
        /// <param name="model">The social login request data, including the provider, ID token, device token, and device token platform.
        /// Must not be null.</param>
        /// <returns>An API response containing the authenticated user's details and a JWT token if authentication is successful;
        /// otherwise, an error response.</returns>
        //[AllowAnonymous]
        //[HttpPost("social-login")]
        //public async Task<ActionResult<APIResponse>> SocialLogin([FromBody] SocialLoginRequestDTO model)
        //{
        //    var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken);


        //    string email = payload.Email.ToLower();
        //    string socialId = payload.Subject; // Google unique ID
        //    string firstName = payload.GivenName ?? "";
        //    string lastName = payload.FamilyName ?? "";

        //    var response = await _unitOfWork.DapperRepo.SocialLoginAsync(email, socialId, firstName, lastName, model.provider, model.DeviceToken, model.DeviceTokenPlatform);

        //    if (string.IsNullOrEmpty(response.UserId))
        //        return _responseService.InternalServerResponse("Something Went Wrong");

        //    var userClaims = new UserClaims
        //    {
        //        Id = response.UserId ?? "",
        //        Email = response.Email ?? "",
        //        Name = $"{response.FirstName} {response.LastName}",
        //        Roles = response.RoleName,
        //        RolesId = response.RoleId,
        //        ProfileImagePath = response.MainProfileImagePath,
        //        ThumbImagePath = response.ThumImagePath
        //    };

        //    var token = _jwtService.GenerateToken(userClaims, true);

        //    var result = new LoginResponseDTO()
        //    {
        //        UserDetails = response,
        //        JwtTokenId = token
        //    };

        //    return _responseService.SuccessResponse(result);
        //}

        [AllowAnonymous]
        [HttpPost("social-login")]
        public async Task<ActionResult<APIResponse>> SocialLogin([FromBody] SocialLoginRequestDTO model)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken);

                if (!payload.EmailVerified)
                    return _responseService.BadRequestResponse("Google email is not verified.");
            }
            catch
            {
                return _responseService.BadRequestResponse("Invalid Google token.");
            }

            var email = payload.Email.ToLower();
            var socialId = payload.Subject;
            var firstName = payload.GivenName ?? string.Empty;
            var lastName = payload.FamilyName ?? string.Empty;

            var response = await _unitOfWork.DapperRepo.SocialLoginAsync(email, socialId, firstName, lastName, model.provider, model.DeviceToken, model.DeviceTokenPlatform);

            if (string.IsNullOrEmpty(response?.UserId))
                return _responseService.InternalServerResponse("Something Went Wrong");

            if (string.IsNullOrEmpty(response.MainProfileImagePath) && !string.IsNullOrEmpty(payload.Picture))
            {
                try
                {
                    var googlePicUrl = payload.Picture.TrimEnd('=') + "=s400-c";
                    var imageBytes = await _httpClient.GetByteArrayAsync(googlePicUrl);

                    var extension = DetectImageExtension(imageBytes);

                    if (extension is null)
                        throw new InvalidOperationException("Unsupported image format from Google.");

                    // Validate magic bytes — never trust Content-Type from external URL
                    if (!IsValidImageBytes(imageBytes))
                        throw new InvalidOperationException("Google image failed magic-byte validation.");

                    var base64 = Convert.ToBase64String(imageBytes);
                    var fileName = $"{response.UserId}_{response.FirstName}_{response.LastName}";

                    var saveRequest = new Base64ImageSaveRequestDTO
                    {
                        Base64Image = base64,
                        Extension = extension,
                        FileName = fileName,
                        ImageFolderPath = "UploadFiles/ProfilePic",
                        ThumbnailFolderPath = "ThumbnailImages/TNProfilePic"
                    };

                    var uploadResult = await _fileService.SaveBase64ImageAsync(saveRequest);

                    await _unitOfWork.DapperRepo.UpdateUserProfileImageAsync(response.UserId, uploadResult);

                    response.MainProfileImagePath = uploadResult.WebPath;
                    response.ThumImagePath = uploadResult.ThumbnailWebPath;
                }
                catch (Exception ex)
                {
                    await _logService.LogCustomAsync(
                $"[SocialLogin] Google profile pic download failed for UserId={response.UserId}: {ex.Message}",
                "WARN");
                }
            }

            var userClaims = new UserClaims
            {
                Id = response.UserId ?? string.Empty,
                Email = response.Email ?? string.Empty,
                Name = $"{response.FirstName} {response.LastName}",
                Roles = response.RoleName,
                RolesId = response.RoleId,
                ProfileImagePath = response.MainProfileImagePath,
                ThumbImagePath = response.ThumImagePath
            };

            var token = _jwtService.GenerateToken(userClaims, true);
            var result = new LoginResponseDTO { UserDetails = response, JwtTokenId = token };

            return _responseService.SuccessResponse(result);
        }

        /// <summary>
        /// Changes the password for a user based on the provided request data.
        /// </summary>
        /// <param name="req">An object containing the current and new passwords required to perform the password change operation. Cannot
        /// be null.</param>
        /// <returns>An ActionResult containing an APIResponse that indicates whether the password change was successful or
        /// provides an error message if the operation failed.</returns>
        [HttpPost("change-user-password")]
        public async Task<ActionResult<APIResponse>> ChangeUserPasswrd([FromBody] ChangePasswordRequestDto req)
        {
            var user = await _unitOfWork.DapperRepo.GetUserDetailsByIdAsync(req.UserId);

            bool isSocialUser = user?.IsSocialUser == true && string.IsNullOrEmpty(user.Password);

            var encryption = new Encryption();

            string? encryptedCurrent = null;

            if (!isSocialUser)
            {
                if (string.IsNullOrEmpty(req.CurrentPassword))
                    return _responseService.BadRequestResponse("Current password is required.");

                encryptedCurrent = encryption.Encrypt_final(req.CurrentPassword!);
            }

			var encryptedNew = encryption.Encrypt_final(req.NewPassword);
            var encryptedConfirm = encryption.Encrypt_final(req.ConfirmPassword);
         
            req.CurrentPassword = encryptedCurrent;
            req.NewPassword = encryptedNew;
            req.ConfirmPassword = encryptedConfirm;

            var response = await _unitOfWork.DapperRepo.ChangePasswordAsync(req);

            if (response.Result <= 0)
                return _responseService.BadRequestResponse(response.Message);


            return _responseService.SuccessResponse(
                isSocialUser ? "Password set successfully" : "Password changed successfully"
            );
        }

        /// <summary>
        /// Determines whether the specified user account is registered only through social login and does not have a
        /// password set.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user to check. Cannot be null or empty.</param>
        /// <returns>An ActionResult containing an APIResponse with a Boolean value: <see langword="true"/> if the user is a
        /// social login user without a password; otherwise, <see langword="false"/>.</returns>
        [HttpGet("check-is-user-only-social-login")]
        public async Task<ActionResult<APIResponse>> CheckIsUserOnlySocialLogin([FromQuery, BindRequired] string UserId)
        {
            if(string.IsNullOrEmpty(UserId))
                return _responseService.BadRequestResponse("Invalid User Id.");

            var user = await _unitOfWork.DapperRepo.GetUserDetailsByIdAsync(UserId);

            bool isSocialUser = user.IsSocialUser == true && string.IsNullOrEmpty(user.Password);


            return _responseService.SuccessResponse(isSocialUser);
        }

        /// <summary>
        /// Logs out the user by deactivating the specified device token.
        /// </summary>
        /// <param name="model">The logout request containing the user identifier and device token to be deactivated. Both fields are
        /// required and must not be null or empty.</param>
        /// <returns>An API response indicating the result of the logout operation. Returns a bad request response if required
        /// fields are missing.</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<APIResponse>> Logout([FromBody] LogoutRequestDTO model)
        {
            if(string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.DeviceToken))
                return _responseService.BadRequestResponse("UserId and DeviceToken are required.");

            await _unitOfWork.DapperRepo.DeactivateDeviceTokenAsync(model);

            return _responseService.SuccessResponse("Logged out successfully.");
        }


        #region :: Private Helper Methods

        private string GenerateOTP() =>
            RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        private string GenerateOTPEmailTemplate(string otp)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                        .otp-box {{ background-color: #fff; border: 2px dashed #4CAF50; padding: 20px; text-align: center; font-size: 32px; font-weight: bold; letter-spacing: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
                        .warning {{ color: #d32f2f; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>You have requested to reset your password. Please use the following OTP to proceed:</p>
                            <div class='otp-box'>{otp}</div>
                            <p class='warning'>⚠️ This OTP is valid for 5 minutes only.</p>
                            <p>If you did not request this password reset, please ignore this email or contact support if you have concerns.</p>
                            <p>Thank you,<br/>Yafa Team</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated email. Please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        private string GenerateResetToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private static bool IsValidImageBytes(byte[] bytes)
        {
            if (bytes.Length < 4) return false;

            bool isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
            bool isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

            return isJpeg || isPng;
        }

        private static string? DetectImageExtension(byte[] bytes)
        {
            if (bytes.Length < 4) return null;

            // JPEG: FF D8 FF
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return "jpg";

            // PNG: 89 50 4E 47
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return "png";

            // WebP: 52 49 46 46 ... 57 45 42 50
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                return "webp";

            return null; // unsupported — caller throws
        }

        #endregion



    }
}
