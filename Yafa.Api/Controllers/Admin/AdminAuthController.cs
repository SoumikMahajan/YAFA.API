using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Api.Controllers.Admin
{
	/// <summary>
	/// Provides API endpoints for administrator authentication, user registration via the admin panel, and password
	/// retrieval for users. Supports login, registration, and password decryption operations for administrative workflows.
	/// </summary>
	/// <remarks>This controller is intended for use in administrative scenarios and exposes endpoints that do not
	/// always require prior authentication. Some actions, such as password retrieval, are restricted to users with the
	/// Admin role. The controller leverages dependency-injected services for logging, data access, response formatting,
	/// JWT token generation, and file handling. All endpoints return standardized API responses to facilitate consistent
	/// client handling.</remarks>
	[Route("api/[controller]")]
	[ApiController]
	public class AdminAuthController : ControllerBase
	{
		private readonly ILogService _logService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApiResponseService _responseService;
		private readonly IJwtService _jwtService;
		private readonly IFileRepo _fileService;

		/// <summary>
		/// Initializes a new instance of the AdminAuthController class with the specified services and configuration.
		/// </summary>
		/// <param name="logService">The logging service used to record application events and errors.</param>
		/// <param name="unitOfWork">The unit of work instance that manages database transactions and repository access.</param>
		/// <param name="configuration">The application configuration settings used for controller operations.</param>
		/// <param name="responseService">The service used to generate standardized API responses.</param>
		/// <param name="jwtService">The service responsible for handling JSON Web Token (JWT) operations.</param>
		/// <param name="fileService">The file repository service used for file management operations.</param>
		public AdminAuthController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IJwtService jwtService, IFileRepo fileService)
		{
			_logService = logService;
			_unitOfWork = unitOfWork;
			_responseService = responseService;
			_jwtService = jwtService;
			_fileService = fileService;
		}

		/// <summary>
		/// Authenticates an administrator using the provided credentials and returns a JWT token along with user details if
		/// authentication is successful.
		/// </summary>
		/// <remarks>This endpoint does not require prior authentication. The returned JWT token can be used for
		/// subsequent authorized requests. If the credentials are invalid, a generic error message is returned to avoid
		/// disclosing sensitive information.</remarks>
		/// <param name="model">The login request data containing the administrator's email, password, and optional 'RememberMe' flag. The
		/// password must be provided in plain text; it will be encrypted before authentication.</param>
		/// <returns>An ActionResult containing an APIResponse with the authenticated administrator's details and a JWT token if
		/// authentication succeeds; otherwise, an error response indicating invalid credentials.</returns>
		[AllowAnonymous]
		[HttpPost("admin-login")]
		public async Task<ActionResult<APIResponse>> AdminLogin([FromBody] LoginRequestDTO model)
		{

			Encryption eobj = new();
			string encryptedPassword = eobj.Encrypt_final(model.Password);
			model.Password = encryptedPassword;

			var response = await _unitOfWork.DapperRepo.AdminLoginAsync(model);
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
		/// Registers a new user through the admin panel with the specified registration details.
		/// </summary>
		/// <remarks>This endpoint is accessible without authentication. The method processes and normalizes user
		/// names, encrypts the password, and optionally handles profile image upload if provided. Logging is performed for
		/// both successful and failed registration attempts.</remarks>
		/// <param name="model">An object containing the registration information for the new user, including personal details and credentials.
		/// Cannot be null.</param>
		/// <returns>An API response indicating the result of the registration operation. Returns a success response if registration is
		/// completed; otherwise, returns a bad request response with an error message.</returns>
		[AllowAnonymous]
		[HttpPost("user-registration-on-admin-panel")]
		public async Task<ActionResult<APIResponse>> AdminRegister([FromBody] AdminRegisterRequestDTO model)
		{

			var UserId = Guid.NewGuid().ToString();
			var UserDetailsId = Guid.NewGuid().ToString();

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

			if (!string.IsNullOrEmpty(model.ProfileImageBase64))
			{
				//var result = await _fileService
				//    .SaveBase64ImageAsync(model.ProfileImageBase64, model.ProfileImageExtension ?? "jpg", $"{UserId}_{model.FirstName}_{model.LastName}", "UploadFiles/ProfilePic");

				imageUploadReult = await _fileService.SaveBase64ImageAsync(
					new Base64ImageSaveRequestDTO
					{
						Base64Image = model.ProfileImageBase64,
						Extension = model.ProfileImageExtension ?? "jpg",
						FileName = $"{UserId}_{model.FirstName}_{model.LastName}",
						ImageFolderPath = "UploadFiles/ProfilePic",
						ThumbnailFolderPath = "ThumbnailImages/TNProfilePic",
					}
					);

				//ProfileImage = result.FileName;
				//ProfileImageName = "/" + result.WebPath;
				//LocalPath = result.LocalPath;
				//ThumbnailPath = "/" + result.ThumbnailWebPath;
				//ThumbnailImageName = result.ThumbnailImageName;
			}

			var response = await _unitOfWork.DapperRepo.AdminRegisterAsync(model, imageUploadReult, UserId, UserDetailsId);

			if (response <= 0)
			{
				await _logService.LogCustomAsync($"Registration Failed On UserId {UserId} and UserDetailsId {UserDetailsId}", "FAILED");
				return _responseService.BadRequestResponse("Registration failed.");
			}


			await _logService.LogCustomAsync($"Registration Succesfully On UserId {UserId} and UserDetailsId {UserDetailsId}", "SUCCESS");

			return _responseService.SuccessResponse("Registration Succesfully");
		}

		/// <summary>
		/// Retrieves and decrypts the password associated with the specified email address.
		/// </summary>
		/// <remarks>This method is restricted to users with the Admin role. The response will indicate an error if
		/// the email is not provided or if the password cannot be found for the specified email.</remarks>
		/// <param name="email">The email address of the user whose password is to be retrieved and decrypted. Cannot be null or empty.</param>
		/// <returns>An API response containing the decrypted password if found; otherwise, an error response indicating the reason for
		/// failure.</returns>
		[Authorize(Roles = "Admin")]
		[HttpGet("get-userpassword-from-email/{email}")]
		public async Task<ActionResult<APIResponse>> DecryptPassWord(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return _responseService.BadRequestResponse("please required email");
			}

			var password = await _unitOfWork.DapperRepo.DecryptPassAsync(email);

			if (string.IsNullOrEmpty(password.Item3))
				return _responseService.InternalServerResponse("password can not found");

			Encryption eobj = new();
			string encryptedPassword = eobj.Decrypt_final(password.Item3);

			return _responseService.SuccessResponse(encryptedPassword);

		}

		/// <summary>
		/// Authenticates an administrator using the provided credentials and issues an access token for API usage.
		/// </summary>
		/// <remarks>The generated access token is set as an HTTP-only, secure cookie named 'yafa_auth'. The cookie's
		/// expiration depends on the remember-me option: 6 days if selected, otherwise 1 day. This endpoint does not require
		/// prior authentication.</remarks>
		/// <param name="model">The login request containing the administrator's email, password, and remember-me preference. The password must be
		/// provided in plain text; it will be encrypted before authentication.</param>
		/// <returns>An ActionResult containing an APIResponse indicating the result of the login attempt. Returns a success response
		/// with an access token if authentication is successful; otherwise, returns an error response indicating invalid
		/// credentials.</returns>
		[AllowAnonymous]
		[HttpPost("admin-login-for-access-api")]
		public async Task<ActionResult<APIResponse>> AdminLoginForAccessAPI([FromBody] LoginRequestDTO model)
		{

			Encryption eobj = new();
			string encryptedPassword = eobj.Encrypt_final(model.Password);
			model.Password = encryptedPassword;

			var response = await _unitOfWork.DapperRepo.AdminLoginAsync(model);
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

			Response.Cookies.Append("yafa_auth", token, new CookieOptions
			{
				HttpOnly = true,       // JS cannot read → XSS safe
				Secure = true,       // HTTPS only
				SameSite = SameSiteMode.Strict,
				Expires = model.RememberMe
				  ? DateTimeOffset.UtcNow.AddDays(6)
				  : DateTimeOffset.UtcNow.AddDays(1)
			});

			return _responseService.SuccessResponse("Login successful.");

		}

		/// <summary>
		/// Logs out the current user by removing the authentication cookie.
		/// </summary>
		/// <remarks>This method invalidates the user's authentication session by expiring the authentication cookie.
		/// After calling this method, the user will no longer be authenticated in subsequent requests.</remarks>
		/// <returns>An <see cref="OkResult"/> indicating that the logout operation was successful.</returns>
		[HttpPost("logout")]
		public IActionResult Logout()
		{
			Response.Cookies.Append("yafa_auth", "", new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddDays(-1),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});

			return Ok();
		}

		/// <summary>
		/// Determines whether the current user is authenticated and returns an appropriate HTTP response.
		/// </summary>
		/// <remarks>This endpoint can be used by clients to verify authentication status without performing any
		/// additional operations.</remarks>
		/// <returns>An <see cref="OkResult"/> if the user is authenticated; otherwise, an <see cref="UnauthorizedResult"/>.</returns>
		[HttpGet("check")]
		public IActionResult CheckAuth()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				return Ok();
			}

			return Unauthorized();
		}

        /// <summary>
        /// Returns a short-lived (5 min) JWT for Scalar portal use only.
        /// Called by the portal page via fetch() after the HttpOnly session is verified.
        /// The main session token (HttpOnly cookie) is NEVER exposed to JavaScript.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("scalar-token")]
        public async Task<ActionResult<APIResponse>> GetScalarToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                return await Task.FromResult(_responseService.UnAuthorize("Invalid session."));

            var token = _jwtService.GenerateScalarToken(userId, role);

			return _responseService.SuccessResponse(new {token});
        }
    }
}
