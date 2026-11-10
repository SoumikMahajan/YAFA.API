using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Api.Controllers.Admin
{
    /// <summary>
    /// Provides API endpoints for managing and retrieving user information accessible only to users with the Admin
    /// role.
    /// </summary>
    /// <remarks>All actions in this controller require authentication and the caller must be assigned the
    /// Admin role. The controller is intended for administrative operations related to user management and is not
    /// accessible to non-admin users.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponseService _responseService;
        private readonly IFileRepo _fileService;

        /// <summary>
        /// Initializes a new instance of the AdminUserController class with the specified logging, unit of work,
        /// configuration, and response services.
        /// </summary>
        /// <param name="logService">The service used for logging application events and errors. Cannot be null.</param>
        /// <param name="unitOfWork">The unit of work instance that manages data persistence and transactions. Cannot be null.</param>
        /// <param name="configuration">The application configuration settings. Cannot be null.</param>
        /// <param name="responseService">The service used to generate standardized API responses. Cannot be null.</param>
        /// <param name="fileService">The service used for handling file uploads. Cannot be null.</param>
        public AdminUserController(ILogService logService, IUnitOfWork unitOfWork, IConfiguration configuration, ApiResponseService responseService, IFileRepo fileService)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _responseService = responseService;
            _fileService = fileService;
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>An <see cref="ActionResult{APIResponse}"/> containing the list of users if any exist; otherwise, a response
        /// indicating that no users were found.</returns>
        [HttpGet("get-all-user-list")]
        public async Task<ActionResult<APIResponse>> GetAllUserList([FromQuery, BindRequired] int pageNumber, [FromQuery, BindRequired] int pageSize, [FromQuery, BindRequired] string? searchKeyword)
        {
            if (pageNumber < 1 || pageSize < 1)
                return _responseService.BadRequestResponse("Page number and page size must be greater than 0.");

            var response = await _unitOfWork.DapperRepo.GetAllUserListAsync(pageNumber, pageSize, searchKeyword);

            if (response is null)
            {
                return _responseService.SuccessResponse("No user found!");
            }

            return _responseService.SuccessResponse(response);
        }

        /// <summary>
        /// Retrieves detailed information for a user specified by the unique user identifier.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user whose details are to be retrieved. Cannot be null or empty.</param>
        /// <returns>An asynchronous operation that returns an ActionResult containing an APIResponse with the user's details if
        /// found; otherwise, an error response indicating the reason for failure.</returns>
        [HttpGet("user-details-by-id")]
        public async Task<ActionResult<APIResponse>> GetUserDetailsById([FromQuery, BindRequired] string UserId)
        {

            if (string.IsNullOrEmpty(UserId))
            {
                return _responseService.BadRequestResponse("UserId is required.");
            }

            var response = await _unitOfWork.DapperRepo.GetUserDetailsByIdAsync(UserId);

            Encryption eobj = new();
            string decryptedPassword = eobj.Decrypt_final(response.Password);
            if (!string.IsNullOrEmpty(decryptedPassword))
            {
                response.Password = decryptedPassword;
            }

            if (response is null)
            {
                return _responseService.NotFoundResponse("No user found!");
            }

            return _responseService.SuccessResponse(response);
        }

        /// <summary>
        /// Adds a new user to the system with the provided details. This action is restricted to users with the Admin role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("admin-add-new-user")]
        public async Task<ActionResult<APIResponse>> AdminAddNewUser([FromBody] AddUserRequestDTO model)
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

            var response = await _unitOfWork.DapperRepo.AdminAddNewUserAsync(model, UserId, UserDetailsId);

            if (response <= 0)
            {
                await _logService.LogCustomAsync($"Registration Failed On UserId {UserId} and UserDetailsId {UserDetailsId}");
                return _responseService.BadRequestResponse("Registration failed.");
            }

            if (!string.IsNullOrEmpty(model.ProfileImageBase64))
            {
                var imageUploadReult = await _fileService.SaveBase64ImageAsync(
                    new Base64ImageSaveRequestDTO
                    {
                        Base64Image = model.ProfileImageBase64,
                        Extension = model.ProfileImageExtension ?? "jpg",
                        FileName = $"{UserId}_{model.FirstName}_{model.LastName}",
                        ImageFolderPath = "UploadFiles/ProfilePic",
                        ThumbnailFolderPath = "ThumbnailImages/TNProfilePic",
                    }
                );

                var UpdateImagerResponse = await _unitOfWork.DapperRepo.AdminNewUserUpdateUserProfileImageAsync(UserId, imageUploadReult);

                if (UpdateImagerResponse <= 0)
                {
                    await _logService.LogCustomAsync($"Profile Image Update Failed On UserId {UserId} and UserDetailsId {UserDetailsId}");
                    return _responseService.SuccessResponse("Registration Succesfull but Profile Image Update failed.");
                }
            }



            await _logService.LogCustomAsync($"Registration Succesfully On UserId {UserId} and UserDetailsId {UserDetailsId}");

            return _responseService.SuccessResponse("Registration Succesfully");
        }

        /// <summary>
        /// Updates the details of an existing user in the system. This action is restricted to users with the Admin role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("admin-update-user")]
        public async Task<ActionResult<APIResponse>> AdminUpdateUser([FromBody] UpdateUserRequestDTO model)
        {

            model.FirstName = char.ToUpper(model.FirstName[0]) + model.FirstName.Substring(1).ToLower();
            if (!string.IsNullOrEmpty(model.MiddleName))
            {
                model.MiddleName = char.ToUpper(model.MiddleName[0]) + model.MiddleName.Substring(1).ToLower();
            }
            model.LastName = char.ToUpper(model.LastName[0]) + model.LastName.Substring(1).ToLower();

            Encryption eobj = new();
            string encryptedPassword = eobj.Encrypt_final(model.Password);
            model.Password = encryptedPassword;


            var response = await _unitOfWork.DapperRepo.AdminUpdateUserAsync(model);

            if (response <= 0)
            {
                await _logService.LogCustomAsync($"User Update Failed On UserId {model.UserId}");
                return _responseService.BadRequestResponse("User update failed.");
            }


            if (!string.IsNullOrEmpty(model.ProfileImageBase64))
            {
                var imageUploadReult = await _fileService.SaveBase64ImageAsync(
                    new Base64ImageSaveRequestDTO
                    {
                        Base64Image = model.ProfileImageBase64,
                        Extension = model.ProfileImageExtension ?? "jpg",
                        FileName = $"{model.UserId}_{model.FirstName}_{model.LastName}",
                        ImageFolderPath = "UploadFiles/ProfilePic",
                        ThumbnailFolderPath = "ThumbnailImages/TNProfilePic",
                    }
                );

                var UpdateImagerResponse = await _unitOfWork.DapperRepo.AdminNewUserUpdateUserProfileImageAsync(model.UserId, imageUploadReult);

                if (UpdateImagerResponse <= 0)
                {
                    await _logService.LogCustomAsync($"Profile Image Update Failed On UserId {model.UserId}");
                    return _responseService.SuccessResponse("User updated but Profile Image Update failed.");
                }
            }

            return _responseService.SuccessResponse("User updated successfully");
        }


        /// <summary>
        /// Deletes a user from the system based on the provided UserId. This action is restricted to users with the Admin role.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        [HttpPost("admin-delete-user")]
        public async Task<ActionResult<APIResponse>> AdminDeleteUser([FromQuery, BindRequired] string UserId, [FromQuery, BindRequired] string updatedBy)
        {

            if (string.IsNullOrEmpty(UserId))
                return _responseService.BadRequestResponse("UserId is required.");

            if(string.IsNullOrEmpty(updatedBy))
                return _responseService.BadRequestResponse("UpdatedBy is required.");

            var response = await _unitOfWork.DapperRepo.AdminUserDeleteAsycn(UserId, updatedBy);
          

            if (response <= 0)
            {
                return _responseService.NotFoundResponse("Something went wrong.");
            }

            return _responseService.SuccessResponse("User deleted");
        }

    }
}
