using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.Payments;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;
using Yafa.Domain.Models;
using Yafa.Infrastructure.Data;

namespace Yafa.Infrastructure.Repository
{
	public class DapperService : IDapperRepo
	{
		private readonly ApplicationDbContext _db;
		private readonly ILogService _logService;
		private readonly string? _connectionString;
		private readonly IConfiguration _configuration;

		public DapperService(ApplicationDbContext db, ILogService logService, IConfiguration configuration)
		{
			_configuration = configuration;
			_db = db;
			_logService = logService;
			_connectionString = _configuration.GetConnectionString("DefaultConnection");

			if (string.IsNullOrWhiteSpace(_connectionString))
				throw new Exception("Connection string 'DefaultConnection' is missing.");
		}


		#region :: Admin

		#region :: Auth
		public async Task<int> AdminRegisterAsync(AdminRegisterRequestDTO model, ImageUploadResultDTO uploadResult, string UserId, string UserDetailsId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@RoleId", model.RoleId, DbType.String);
				parameters.Add("@Email", model.Email.ToLower(), DbType.String);
				parameters.Add("@Password", model.Password, DbType.String);
				parameters.Add("@AdminNotes", model.AdminNotes, DbType.String);
				parameters.Add("@CreatedBy", model.CreatedBy, DbType.String);
				parameters.Add("@UserDetailsId", UserDetailsId, DbType.String);
				parameters.Add("@Salutation", model.Salutation, DbType.String);
				parameters.Add("@FirstName", model.FirstName, DbType.String);
				parameters.Add("@MiddleName", model.MiddleName, DbType.String);
				parameters.Add("@LastName", model.LastName, DbType.String);
				parameters.Add("@Gender", model.Gender, DbType.String);
				parameters.Add("@Address1", model.Address1, DbType.String);
				parameters.Add("@Address2", model.Address2, DbType.String);
				parameters.Add("@City", model.City, DbType.String);
				parameters.Add("@State", model.State, DbType.String);
				parameters.Add("@County", model.County, DbType.String);
				parameters.Add("@Country", model.Country, DbType.String);
				parameters.Add("@ZIP", model.ZIP, DbType.String);
				parameters.Add("@Lat", model.Lat, DbType.String);
				parameters.Add("@Lang", model.Lang, DbType.String);
				parameters.Add("@PhoneNo", model.PhoneNo, DbType.String);
				parameters.Add("@PhoneNo2", model.PhoneNo2, DbType.String);
				parameters.Add("@DOB", model.DOB, DbType.String);
				parameters.Add("@ProfileImage", uploadResult.FileName, DbType.String);
				parameters.Add("@LocalPath", $"/{uploadResult.LocalPath}", DbType.String);
				parameters.Add("@ProfileImageName", $"/{uploadResult.WebPath}", DbType.String);
				parameters.Add("@ThumbImage", $"/{uploadResult.ThumbnailImageName}", DbType.String);
				parameters.Add("@ThumbImageName", $"/{uploadResult.ThumbnailWebPath}", DbType.String);
				parameters.Add("@AboutUs", model.AboutUs, DbType.String);
				parameters.Add("@Mode", "REGISTRATION", DbType.String);


				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<LoginResponseUserDetailsDTO> AdminLoginAsync(LoginRequestDTO model)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@EMAIL_ID", model.Email.ToLower(), DbType.String);
				parameters.Add("@PASSWORD", model.Password, DbType.String);
				parameters.Add("@DeviceToken", model.DeviceToken, DbType.String);
				parameters.Add("@DeviceTokenPlatform", model.DeviceTokenPlatform, DbType.String);
				parameters.Add("@Mode", "ADMIN_LOGIN", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var user = await sqlConnection.QueryFirstOrDefaultAsync<LoginResponseUserDetailsDTO>(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);


				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return user ?? new LoginResponseUserDetailsDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<LoginResponseUserDetailsDTO> UserLoginAsync(LoginRequestDTO model)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@EMAIL_ID", model.Email.ToLower(), DbType.String);
				parameters.Add("@PASSWORD", model.Password, DbType.String);
				parameters.Add("@DeviceToken", model.DeviceToken, DbType.String);
				parameters.Add("@DeviceTokenPlatform", model.DeviceTokenPlatform, DbType.String);
				parameters.Add("@Mode", "USER_LOGIN", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var user = await sqlConnection.QueryFirstOrDefaultAsync<LoginResponseUserDetailsDTO>(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);


				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return user ?? new LoginResponseUserDetailsDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, string)> DecryptPassAsync(string Email)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@EMAIL_ID", Email, DbType.String);
				parameters.Add("@mode", "DecryptPassword", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var password = await sqlConnection.QuerySingleAsync<string>(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";


				return (result, message, password);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin User Management
		public async Task<IEnumerable<UserDetailsDTO>> GetAllUserListAsync(int pageNumber, int pageSize, string? searchKeyword)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "UserDetailsList", DbType.String);
				parameters.Add("@PageNumber", pageNumber, DbType.Int32);
				parameters.Add("@PageSize", pageSize, DbType.Int32);
				parameters.Add("@SearchText", searchKeyword, DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var userList = await sqlConnection.QueryAsync<UserDetailsDTO>(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);


				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return userList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserDetailsDTO> GetUserDetailsByIdAsync(string UserId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@Mode", "UserDetailsById", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var user = await sqlConnection.QueryFirstOrDefaultAsync<UserDetailsDTO>(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return user ?? new UserDetailsDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> AdminAddNewUserAsync(AddUserRequestDTO model, string UserId, string UserDetailsId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@Email", model.Email.ToLower(), DbType.String);
				parameters.Add("@PASSWORD", model.Password, DbType.String);
				parameters.Add("@CreatedBy", model.CreatedBy, DbType.String);
				parameters.Add("@UserDetailsId", UserDetailsId, DbType.String);
				parameters.Add("@FirstName", model.FirstName, DbType.String);
				parameters.Add("@MiddleName", model.MiddleName, DbType.String);
				parameters.Add("@LastName", model.LastName, DbType.String);
				parameters.Add("@Gender", model.Gender, DbType.String);
				parameters.Add("@Address1", model.Address1, DbType.String);
				parameters.Add("@Address2", model.Address2, DbType.String);
				parameters.Add("@City", model.City, DbType.String);
				parameters.Add("@State", model.State, DbType.String);
				parameters.Add("@Country", model.Country, DbType.String);
				parameters.Add("@ZIP", model.ZIP, DbType.String);
				parameters.Add("@PhoneNo", model.PhoneNo, DbType.String);
				parameters.Add("@PhoneNo2", model.PhoneNo2, DbType.String);
				parameters.Add("@DOB", model.DOB, DbType.String);
				parameters.Add("@Mode", "AddNewUser", DbType.String);


				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> AdminUpdateUserAsync(UpdateUserRequestDTO Updatemodel)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@UserId", Updatemodel.UserId, DbType.String);
				parameters.Add("@PASSWORD", Updatemodel.Password, DbType.String);
				parameters.Add("@UpdatedBy", Updatemodel.UpdateBy, DbType.String);
				parameters.Add("@FirstName", Updatemodel.FirstName, DbType.String);
				parameters.Add("@MiddleName", Updatemodel.MiddleName, DbType.String);
				parameters.Add("@LastName", Updatemodel.LastName, DbType.String);
				parameters.Add("@Gender", Updatemodel.Gender, DbType.String);
				parameters.Add("@Address1", Updatemodel.Address1, DbType.String);
				parameters.Add("@Address2", Updatemodel.Address2, DbType.String);
				parameters.Add("@City", Updatemodel.City, DbType.String);
				parameters.Add("@State", Updatemodel.State, DbType.String);
				parameters.Add("@Country", Updatemodel.Country, DbType.String);
				parameters.Add("@ZIP", Updatemodel.ZIP, DbType.String);
				parameters.Add("@PhoneNo", Updatemodel.PhoneNo, DbType.String);
				parameters.Add("@PhoneNo2", Updatemodel.PhoneNo2, DbType.String);
				parameters.Add("@DOB", Updatemodel.DOB, DbType.String);
				parameters.Add("@Mode", "AdminUpdateUser", DbType.String);


				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> AdminNewUserUpdateUserProfileImageAsync(string userId, ImageUploadResultDTO uploadResult)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@ProfileImage", uploadResult.FileName, DbType.String);
				parameters.Add("@LocalPath", $"{uploadResult.LocalPath}", DbType.String);
				parameters.Add("@ProfileImageName", $"{uploadResult.WebPath}", DbType.String);
				parameters.Add("@ThumbImage", $"{uploadResult.ThumbnailImageName}", DbType.String);
				parameters.Add("@ThumbImageName", $"{uploadResult.ThumbnailWebPath}", DbType.String);
				parameters.Add("@Mode", "AdminUpdateUserProfileImage", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> AdminUserDeleteAsycn(string UserId, string updatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@UpdatedBy", updatedBy, DbType.String);
				parameters.Add("@Mode", "AdminDeleteUser", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_UserManagement",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin Prop Management

		public async Task<(int Result, string Message)> AddBlockPropertyDatesAsync(BlockPropRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", request.PropertyPublicId, DbType.Guid);
				parameters.Add("@BlockDates", request.BlockDates, DbType.String);
				parameters.Add("@BlockReason", request.BlockReason, DbType.String);
				parameters.Add("@Notes", request.Notes, DbType.String);
				parameters.Add("@CreatedBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "BlockPropertyDates", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UnBlockPropertyDatesDatesAsync(Guid UnavailablePublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@UnavailablePublicId", UnavailablePublicId, DbType.Guid);
				parameters.Add("@mode", "UnblockPropertyDate", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<BlockPropDatesDetailsDTO>> BlockPropDatesDetailsAsync(Guid PropertyPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PropertyPublicId", PropertyPublicId, DbType.Guid);
				parameters.Add("@mode", "GetPropertyBlockedDates", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var unavailDateList = await sqlConnection.QueryAsync<BlockPropDatesDetailsDTO>(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return unavailDateList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<FinalSubmitPropertyResponceDTO>> GetAllFinalSubmitPropertiesAsync(AdminPropertySearchRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@SearchText", request.SearchText, DbType.String);
				parameters.Add("@PageSize", request.PageSize, DbType.Int32);
				parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
				parameters.Add("@PropertyIsActive", request.IsActive, DbType.Boolean);
				parameters.Add("@mode", "GetAllFinalSubmitProperty", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var propList = await sqlConnection.QueryAsync<FinalSubmitPropertyResponceDTO>(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return propList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, AllBookedDetailResponseDTO)> AdminBookingListItemAsync(AllBookedDetailRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingStatus", request.BookingStatus, DbType.Int32);
				parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", request.PageSize, DbType.Int32);
				parameters.Add("@SearchText", request.SearchTerm, DbType.String);
				parameters.Add("@SortBy", request.SortBy, DbType.String);
				parameters.Add("@SortDirection", request.SortDirection, DbType.String);
				parameters.Add("@BStartDate", request.StartDate, DbType.String);
				parameters.Add("@BEndDate", request.EndDate, DbType.String);
				parameters.Add("@PaymentStatus", request.PaymentStatus, DbType.Int32);
				parameters.Add("@mode", "GetAllBookings", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				List<AdminBookingListItemDto>? AllBookedList = null;
				AdminBookingCount adminBookingCount = new();

				try
				{
					AllBookedList = (await multi.ReadAsync<AdminBookingListItemDto>()).ToList();
					adminBookingCount = (await multi.ReadAsync<AdminBookingCount>()).FirstOrDefault() ?? new AdminBookingCount();
				}
				catch { }

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				AllBookedDetailResponseDTO allBookedDetailResponseDTO = new()
				{
					Bookings = AllBookedList ?? new List<AdminBookingListItemDto>(),
					BookingCounts = adminBookingCount
				};
				return (result, message, allBookedDetailResponseDTO);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, AdminBookingDetailsDto)> AdminBookingDeatilsByIdAsync(Guid BookingPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@mode", "GetBookingDetails", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				AdminBookingDetailsDto? AllBookedList = null;
				List<BookingFeeDTO>? feeBreakdown = new();
				List<AdminBookingPaymentSlotDto>? paymentPlanSlots = new();
				List<AdminBookingTimelineDto>? AdminBookingTimelineDto = new();

				try
				{
					AllBookedList = await multi.ReadFirstOrDefaultAsync<AdminBookingDetailsDto>();
					feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
					paymentPlanSlots = (await multi.ReadAsync<AdminBookingPaymentSlotDto>()).ToList();
					AdminBookingTimelineDto = (await multi.ReadAsync<AdminBookingTimelineDto>()).ToList();
				}
				catch
				{
				}

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (AllBookedList is not null)
				{
					AllBookedList.FeeBreakdown = feeBreakdown;
					AllBookedList.PaymentPlan = paymentPlanSlots;
					AllBookedList.Timeline = AdminBookingTimelineDto;
				}

				return (result, message, AllBookedList ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, AdminAddNewBookingResponseDto)> AdminAddNewBookingAsync(AdminAddNewBookingRequestDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", req.PropertyPublicId, DbType.Guid);
				parameters.Add("@UserId", req.UserId, DbType.String);
				parameters.Add("@CheckInDate", req.CheckInDate, DbType.Date);
				parameters.Add("@CheckOutDate", req.CheckOutDate, DbType.Date);
				parameters.Add("@NumberOfGuests", req.NumberOfGuests, DbType.Int32);
				parameters.Add("@BookingSourceType", req.BookingSourceType, DbType.Guid);
				parameters.Add("@OfferPublicId", req.OfferPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", req.CreatedBy, DbType.String);
				parameters.Add("@mode", "AdminAddNewBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure);

				AdminAddNewBookingResponseDto? PaymentCreateResponse = null;
				List<BookingFeeDTO> feeBreakdown = new();

				try
				{
					PaymentCreateResponse = await multi.ReadFirstOrDefaultAsync<AdminAddNewBookingResponseDto>();
					feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
				}
				catch
				{
				}

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (PaymentCreateResponse is not null)
					PaymentCreateResponse.FeeBreakdown = feeBreakdown;

				return (result, message, PaymentCreateResponse ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminPanelConfirmBookingAsync(Guid BookingPublicId, string CreatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", CreatedBy, DbType.String);
				parameters.Add("@mode", "ConfirmBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCheckInAsync(Guid BookingPublicId, string CreatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", CreatedBy, DbType.String);
				parameters.Add("@mode", "CheckInBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var resultData = await sqlConnection.QueryFirstOrDefaultAsync<(string OwnerUserId, string PropertyName)>(cmd);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, resultData.OwnerUserId, resultData.PropertyName);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCheckOutAsync(Guid BookingPublicId, string CreatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", CreatedBy, DbType.String);
				parameters.Add("@mode", "CheckOutBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var resultData = await sqlConnection.QueryFirstOrDefaultAsync<(string OwnerUserId, string PropertyName)>(cmd);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, resultData.OwnerUserId, resultData.PropertyName);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCancelBookingAsync(CancelBookingRequestDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
				parameters.Add("@CancellationReason", req.CancellationReason, DbType.String);
				parameters.Add("@CreatedBy", req.CreatedBy, DbType.String);
				parameters.Add("@mode", "CancelBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var resultData = await sqlConnection.QueryFirstOrDefaultAsync<(string OwnerUserId, string PropertyName)>(cmd);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, resultData.OwnerUserId, resultData.PropertyName);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message)> AdminPanelUpdateBookingToProcessRefundAsync(RefundBookingRequestDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
				parameters.Add("@RefundAmount", req.RefundAmount, DbType.Decimal);
				parameters.Add("@CreatedBy", req.CreatedBy, DbType.String);
				parameters.Add("@mode", "ProcessRefund", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message, string? UserId, string? PropertyName, DateOnly? NewCheckOutDate)> AdminPanelExtendBookingAsync(ExtendBookingRequestDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
				parameters.Add("@CheckOutDate", req.CheckOutDate, DbType.Date);
				parameters.Add("@CreatedBy", req.CreatedBy, DbType.String);
				parameters.Add("@mode", "ExtendBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var resultData = await sqlConnection.QueryFirstOrDefaultAsync<(string UserId, string PropertyName, DateOnly? CheckOutDate)>(cmd);


				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, resultData.UserId, resultData.PropertyName, resultData.CheckOutDate);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminPanelUpdatePropertyIsPopularAsync(Guid propertyPublicId, bool isPopular)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@IsPopular", isPopular, DbType.Boolean);
				parameters.Add("@mode", "UpdateIsPopular", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message)> AdminPanelUpdatePropertyIsFeaturedAsync(Guid propertyPublicId, bool isFeatured)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@IsFeatured", isFeatured, DbType.Boolean);
				parameters.Add("@mode", "UpdateIsFeatured", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<PropertyImagesOnAdminPanelDTO>> AdminGetAllPropertyImagesAsync(Guid propertyPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@mode", "AdminGetAllPropertyImages", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var propList = await sqlConnection.QueryAsync<PropertyImagesOnAdminPanelDTO>(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return propList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminDeletePropertyImageAsync(Guid imagePublicId, string DeletedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@ImagePublicId", imagePublicId, DbType.Guid);
				parameters.Add("@DeletedBy", DeletedBy, DbType.String);
				parameters.Add("@mode", "AdminDeletePropertyImage", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, int LatestDisplayOrder)> GetLatestPropertyImageDisplayOrderAsync(Guid PropertyPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", PropertyPublicId, DbType.Guid);
				parameters.Add("@mode", "GetLatestDisplayOrderByPropertyId", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				int latestDisplayOrder = await sqlConnection.QueryFirstOrDefaultAsync<int>(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";


				return (result, message, latestDisplayOrder);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddNewPropertyImagesAsync(Guid propPublicId, string CreatedBy, string jsonImages)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@PropertyImagesJson", jsonImages, DbType.String);
				parameters.Add("@CreatedBy", CreatedBy, DbType.String);
				parameters.Add("@Mode", "AddNewPropertyImages", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UpdatePropertyCoverImageAsync(Guid propertyPublicId, Guid coverImagePublicId, string updateBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@ImagePublicId", coverImagePublicId, DbType.Guid);
				parameters.Add("@CreatedBy", updateBy, DbType.String);
				parameters.Add("@Mode", "UpdatePropertyCoverImage", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> InsertFullPropertyAsync(AdminInsertFullPropertyDTO req, Guid PropertyPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", PropertyPublicId, DbType.Guid);
				parameters.Add("@PropertyName", req.PropertyName, DbType.String);
				parameters.Add("@PropertyType", req.PropertyType, DbType.String);
				parameters.Add("@ShortDescription", req.ShortDescription, DbType.String);
				parameters.Add("@Description", req.Description, DbType.String);
				parameters.Add("@Address", req.Address, DbType.String);
				parameters.Add("@City", req.City, DbType.String);
				parameters.Add("@State", req.State, DbType.String);
				parameters.Add("@Country", req.Country, DbType.String);
				parameters.Add("@ZipCode", req.ZipCode, DbType.String);
				parameters.Add("@Latitude", req.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", req.Longitude, DbType.Decimal);
				parameters.Add("@TotalBedrooms", req.TotalBedrooms, DbType.Int32);
				parameters.Add("@TotalBathrooms", req.TotalBathrooms, DbType.Int32);
				parameters.Add("@MaxPersons", req.MaxPersons, DbType.Int32);
				parameters.Add("@DiscountPercentage", req.DiscountPercentage, DbType.Decimal);
				parameters.Add("@ContactFirstName", req.ContactFirstName, DbType.String);
				parameters.Add("@ContactLastName", req.ContactLastName, DbType.String);
				parameters.Add("@ContactEmail", req.ContactEmail, DbType.String);
				parameters.Add("@ContactPhone", req.ContactPhone, DbType.String);

				parameters.Add("@CheckInTime", req.CheckInTime, DbType.Time);
				parameters.Add("@CheckOutTime", req.CheckOutTime, DbType.Time);
				parameters.Add("@CheckInInstructions", req.CheckInInstructions, DbType.String);
				parameters.Add("@AmenityIds", req.AmenityIds, DbType.String);
				parameters.Add("@AdditionalServiceIds", req.ServiceIDs, DbType.String);
				parameters.Add("@ServicePrices", req.ServicePrices, DbType.String);

				parameters.Add("@HouseRuleIds", req.HouseRuleIds, DbType.String);
				parameters.Add("@RefundPolicyDescription", req.RefundPolicyDescription, DbType.String);
				parameters.Add("@RefundPercentage", req.RefundPercentage, DbType.Decimal);
				parameters.Add("@RefundDaysBeforeCheckIn", req.RefundDaysBeforeCheckIn, DbType.Int32);
				parameters.Add("@MemberPrice", req.MemberPrice, DbType.Decimal);
				parameters.Add("@NonMemberPrice", req.NonMemberPrice, DbType.Decimal);
				parameters.Add("@CreatedBy", req.CreatedBy, DbType.String);
				parameters.Add("@OwnerPublicId", req.OwnerPublicId, DbType.String);

				parameters.Add("@Mode", "AddProperty", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync("Sp_Yafa_User_Properties", parameters, commandType: CommandType.StoredProcedure);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<GetUserListForAdminPanelDto>> GetUserListForAdminPanelAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetUserListForAdminPanel", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var userList = await sqlConnection.QueryAsync<GetUserListForAdminPanelDto>(
				   "Sp_Yafa_Admin_Properties",
				   parameters,
				   commandType: CommandType.StoredProcedure
			   );
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";


				if (result <= 0)
					throw new InvalidOperationException(message);

				return userList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> DeletePropertyAsync(Guid PropertyPublicId, string ModifiedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", PropertyPublicId, DbType.Guid);
				parameters.Add("@UpdatedBy", ModifiedBy, DbType.String);
				parameters.Add("@Mode", "DeleteProperty", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync("Sp_Yafa_User_Properties", parameters, commandType: CommandType.StoredProcedure);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<CheckoutDetailsResponseDto> AdminGetCheckOutDetailsAsync(Guid BookingPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@mode", "GetCheckoutDetails", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync("Sp_Yafa_Admin_Bookings", parameters, commandType: CommandType.StoredProcedure);

				var booking = await multi.ReadFirstOrDefaultAsync<CheckoutBookingDetailsDto>();
				var appliedFees = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
				var availableFeeTypes = (await multi.ReadAsync<AvailableFeeTypeDto>()).ToList();

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message);

				var checkoutDetails = new CheckoutDetailsResponseDto();

				if (booking is not null)
				{
					checkoutDetails.Booking = booking;
					checkoutDetails.AppliedFees = appliedFees;
					checkoutDetails.AvailableFeeTypes = availableFeeTypes;
				}


				return checkoutDetails;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminAddExtraFeeAsync(AddExtraFeeRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@mode", "AddExtraFee");
				parameters.Add("@BookingPublicId", request.BookingPublicId);
				parameters.Add("@FeePublicId", request.FeePublicId);
				parameters.Add("@FeeAmount", request.FeeAmount);
				parameters.Add("@Notes", request.Notes);
				parameters.Add("@CreatedBy", request.CreatedBy);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.ExecuteAsync("Sp_Yafa_Admin_Bookings", parameters, commandType: CommandType.StoredProcedure);


				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminRemoveExtraFeeAsync(RemoveExtraFeeRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@mode", "RemoveExtraFee");
				parameters.Add("@BookingFeePublicId", request.BookingFeePublicId);
				parameters.Add("@CreatedBy", request.CreatedBy);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.ExecuteAsync("Sp_Yafa_Admin_Bookings", parameters, commandType: CommandType.StoredProcedure);


				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<DropDown>> AdminPanelListOfSourceTypesAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllSourceTypes", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var BookingSourceList = await sqlConnection.QueryAsync<DropDown>(
				   "Sp_Yafa_Admin_Bookings",
				   parameters,
				   commandType: CommandType.StoredProcedure
			   );
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";


				if (result <= 0)
					throw new InvalidOperationException(message);

				return BookingSourceList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<DropDown>> AdminPanelOwnerListAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@mode", "GetOwnerListForAdminProp", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var OwnerList = await sqlConnection.QueryAsync<DropDown>(
				   "Sp_Yafa_Admin_Properties",
				   parameters,
				   commandType: CommandType.StoredProcedure
			   );
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";


				if (result <= 0)
					throw new InvalidOperationException(message);

				return OwnerList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<AdminPropertyViewDetailsDto?> GetPropertyViewDetailsAsync(Guid propertyPublicId, CancellationToken ct)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PropertyPublicId", propertyPublicId);
				parameters.Add("@mode", "GetPropertyDetailsAdmin", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync("Sp_Yafa_Admin_Properties", parameters,commandType: CommandType.StoredProcedure);

				var details = await multi.ReadFirstOrDefaultAsync<AdminPropertyViewDetailsDto>();
				if (details is null)
					return null;

				details.Images = (await multi.ReadAsync<AdminPropertyViewImageDto>()).ToList();
				details.Amenities = (await multi.ReadAsync<AdminPropertyViewAmenityDto>()).ToList();
				details.HouseRules = (await multi.ReadAsync<AdminPropertyViewHouseRuleDto>()).ToList();
				details.RecentBookings = (await multi.ReadAsync<AdminPropertyViewBookingDto>()).ToList();
				

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result < 0) throw new InvalidOperationException(message); 

				return details;

			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, InvoiceResponseDto)> AdminGetOrCreateInvoiceAsync(Guid bookingPublicId, string IssueBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", bookingPublicId, DbType.Guid);
				parameters.Add("@IssueBy", IssueBy, DbType.String);
				parameters.Add("@mode", "AdminGetOrCreateInvoice", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure);

				InvoiceResponseDto? invoice = null;
				List<InvoiceLineItemDto> lineItems = new();
				List<InvoicePaymentDto> payments = new();

				try
				{
					invoice = await multi.ReadFirstOrDefaultAsync<InvoiceResponseDto>();
					lineItems = (await multi.ReadAsync<InvoiceLineItemDto>()).ToList();
					payments = (await multi.ReadAsync<InvoicePaymentDto>()).ToList();
				}
				catch
				{
				}

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (invoice is not null)
				{
					invoice.LineItems = lineItems;
					invoice.Payments = payments;
				}

				return (result, message, invoice ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, AdminBookingPriceSummery)> AdminBookingPriceSummeryAsync(Guid propertyPublicId, DateOnly CheckInDate, DateOnly CheckOutDate)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@CheckInDate", CheckInDate, DbType.Date);
				parameters.Add("@CheckOutDate", CheckOutDate, DbType.Date);
				parameters.Add("@mode", "AdminBookingPriceSummary", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure);

				AdminBookingPriceSummery? adminBookingPriceSummery = new();
				List<BookingFeeDTO> bookingFeeDTOs = new();
				try
				{
					adminBookingPriceSummery = await multi.ReadFirstOrDefaultAsync<AdminBookingPriceSummery>();
					bookingFeeDTOs = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
				}
				catch
				{
				}

				if(adminBookingPriceSummery is not null)
				{
					adminBookingPriceSummery.FeeBreakdown = bookingFeeDTOs;
				}
				
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";



				return (result, message, adminBookingPriceSummery ?? new AdminBookingPriceSummery());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminAddManualPaymentAsync(AddManualPaymentRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", request.BookingPublicId, DbType.Guid);
				parameters.Add("@Amount", request.Amount, DbType.Decimal);
				parameters.Add("@PaymentMethod", request.PaymentMethod, DbType.String);
				parameters.Add("@CreatedBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "AdminManualPaymentIfExtraFeesAdded", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_Bookings",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin Top Loaction

		public async Task<(int Result, string Message)> AdminAddLocationAsync(Guid LocationPublicId, ImageUploadResultDTO imageUploadResult, AddTopLocationRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@LocationPublicId", LocationPublicId, DbType.Guid);
				parameters.Add("@City", request.City, DbType.String);
				parameters.Add("@LocationName", request.LocationName, DbType.String);
				parameters.Add("@Description", request.Description, DbType.String);
				parameters.Add("@ImagePath", imageUploadResult.WebPath, DbType.String);
				parameters.Add("@ThumbImagePath", imageUploadResult.ThumbnailWebPath, DbType.String);
				parameters.Add("@ImageName", imageUploadResult.FileName, DbType.String);
				parameters.Add("@ThumbImageName", imageUploadResult.ThumbnailImageName, DbType.String);
				parameters.Add("@CreatedBy", request.CreateBy, DbType.String);
				parameters.Add("@mode", "AdminAddLocation", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_TopLocations",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}
		public async Task<(int Result, string Message)> AdminUpdateLocationAsync(ImageUploadResultDTO imageUploadResult, UpdateTopLocationRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@LocationPublicId", request.LocationPublicId, DbType.Guid);
				parameters.Add("@City", request.City, DbType.String);
				parameters.Add("@LocationName", request.LocationName, DbType.String);
				parameters.Add("@Description", request.Description, DbType.String);
				parameters.Add("@ImagePath", imageUploadResult.WebPath, DbType.String);
				parameters.Add("@ThumbImagePath", imageUploadResult.ThumbnailWebPath, DbType.String);
				parameters.Add("@ImageName", imageUploadResult.FileName, DbType.String);
				parameters.Add("@ThumbImageName", imageUploadResult.ThumbnailImageName, DbType.String);
				parameters.Add("@CreatedBy", request.UpdatedBy, DbType.String);
				parameters.Add("@mode", "AdminUpdateLocation", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_TopLocations",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminDeleteLocationAsync(Guid LocationPublicId, string UpdatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@LocationPublicId", LocationPublicId, DbType.Guid);
				parameters.Add("@UpdatedBy", UpdatedBy, DbType.String);
				parameters.Add("@mode", "AdminDeleteLocation", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_TopLocations",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminToggleActiveAsync(Guid LocationPublicId, string UpdatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@LocationPublicId", LocationPublicId, DbType.Guid);
				parameters.Add("@UpdatedBy", UpdatedBy, DbType.String);
				parameters.Add("@mode", "AdminToggleActive", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_TopLocations",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, IEnumerable<AdminTopLocationDto>)> AdminGetAllLocationsAsync(int PageNumber, int PageSize)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PageNumber", PageNumber, DbType.Int32);
				parameters.Add("@PageSize", PageSize, DbType.Int32);
				parameters.Add("@mode", "AdminGetAllLocations", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var List = await sqlConnection.QueryAsync<AdminTopLocationDto>(
					"Sp_Yafa_Admin_TopLocations",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, List);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin Offer Management
		public async Task<(int Result, string Message)> AdminAddOfferAsync(AdminAddOfferRequestDto request, ImageUploadResultDTO uploadResult, Guid offerPublicId)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "ADD_OFFER");
				p.Add("@OfferPublicId", offerPublicId, DbType.Guid);
				p.Add("@Title", request.Title?.Trim(), DbType.String);
				p.Add("@YafaCode", request.YafaCode?.Trim(), DbType.String);
				p.Add("@PriceLabel", request.PriceLabel, DbType.String);
				p.Add("@Description", request.Description, DbType.String);
				p.Add("@BadgeText", request.BadgeText, DbType.String);
				p.Add("@BadgeColor", request.BadgeColor, DbType.String);
				p.Add("@DiscountFixedAmount", request.DiscountFixedAmount, DbType.Decimal);
				p.Add("@DiscountNote", request.DiscountNote, DbType.String);
				p.Add("@AvailableUntil", request.AvailableUntil.HasValue
											? request.AvailableUntil.Value.ToDateTime(TimeOnly.MinValue)
											: (DateTime?)null, DbType.Date);

				// Image fields (empty when no image supplied)
				p.Add("@OfferImageName", uploadResult.FileName, DbType.String);
				p.Add("@OfferImagePath", uploadResult.WebPath != null ? $"/{uploadResult.WebPath}" : null, DbType.String);
				p.Add("@OfferThumbImageName", uploadResult.ThumbnailImageName, DbType.String);
				p.Add("@OfferThumbImagePath", uploadResult.ThumbnailWebPath != null
											? $"/{uploadResult.ThumbnailWebPath}" : null, DbType.String);

				p.Add("@SortOrder", request.SortOrder, DbType.Int32);

				// Serialise terms list → JSON array
				var termsJson = request.Terms != null && request.Terms.Count > 0
					? JsonSerializer.Serialize(request.Terms)
					: null;
				p.Add("@TermsJson", termsJson, DbType.String);
				p.Add("@CreatedBy", request.CreatedBy, DbType.String);

				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();

				// QueryFirstOrDefault to capture the SELECT of the new OfferPublicId
				var row = await conn.QueryFirstOrDefaultAsync<dynamic>("Sp_Yafa_Admin_Offers", p, commandType: CommandType.StoredProcedure);

				int result = p.Get<int?>("@Result") ?? -1;
				string message = p.Get<string>("@Message") ?? "Unknown error.";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminUpdateOfferAsync(AdminUpdateOfferRequestDto request, ImageUploadResultDTO uploadResult)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "UPDATE_OFFER");
				p.Add("@OfferPublicId", request.OfferPublicId, DbType.Guid);
				p.Add("@Title", request.Title?.Trim(), DbType.String);
				p.Add("@PriceLabel", request.PriceLabel, DbType.String);
				p.Add("@Description", request.Description, DbType.String);
				p.Add("@BadgeText", request.BadgeText, DbType.String);
				p.Add("@BadgeColor", request.BadgeColor, DbType.String);
				p.Add("@DiscountFixedAmount", request.DiscountFixedAmount, DbType.Decimal);
				p.Add("@DiscountNote", request.DiscountNote, DbType.String);
				p.Add("@AvailableUntil", request.AvailableUntil.HasValue
											? request.AvailableUntil.Value.ToDateTime(TimeOnly.MinValue)
											: (DateTime?)null, DbType.Date);

				// Only overwrite image columns when a new image was uploaded
				bool hasNewImage = !string.IsNullOrWhiteSpace(uploadResult.FileName);
				p.Add("@OfferImageName", hasNewImage ? uploadResult.FileName : null, DbType.String);
				p.Add("@OfferImagePath", hasNewImage && uploadResult.WebPath != null
											? $"/{uploadResult.WebPath}" : null, DbType.String);
				p.Add("@OfferThumbImageName", hasNewImage ? uploadResult.ThumbnailImageName : null, DbType.String);
				p.Add("@OfferThumbImagePath", hasNewImage && uploadResult.ThumbnailWebPath != null
											? $"/{uploadResult.ThumbnailWebPath}" : null, DbType.String);

				p.Add("@SortOrder", request.SortOrder, DbType.Int32);

				// null → keep existing terms;  empty array "[]" → delete all terms
				string? termsJson = request.Terms != null
					? JsonSerializer.Serialize(request.Terms)
					: null;
				p.Add("@TermsJson", termsJson, DbType.String);
				p.Add("@UpdatedBy", request.UpdatedBy, DbType.String);

				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_Yafa_Admin_Offers", p, commandType: CommandType.StoredProcedure);

				int result = p.Get<int?>("@Result") ?? -1;
				string message = p.Get<string>("@Message") ?? "Unknown error.";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminDeleteOfferAsync(Guid offerPublicId, string deletedBy)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "DELETE_OFFER");
				p.Add("@OfferPublicId", offerPublicId, DbType.Guid);
				p.Add("@UpdatedBy", deletedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_Yafa_Admin_Offers", p, commandType: CommandType.StoredProcedure);

				int result = p.Get<int?>("@Result") ?? -1;
				string message = p.Get<string>("@Message") ?? "Unknown error.";

				if (result <= 0) throw new InvalidOperationException(message);
				return (result, message);
			}
			catch (SqlException ex) { await _logService.LogExceptionErrorAsync(ex); throw; }
		}

		public async Task<(int Result, string Message)> AdminToggleOfferActiveAsync(Guid offerPublicId, string updatedBy)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "TOGGLE_ACTIVE");
				p.Add("@OfferPublicId", offerPublicId, DbType.Guid);
				p.Add("@UpdatedBy", updatedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_Yafa_Admin_Offers", p, commandType: CommandType.StoredProcedure);

				int result = p.Get<int?>("@Result") ?? -1;
				string message = p.Get<string>("@Message") ?? "Unknown error.";

				if (result <= 0) throw new InvalidOperationException(message);
				return (result, message);
			}
			catch (SqlException ex) { await _logService.LogExceptionErrorAsync(ex); throw; }
		}

		public async Task<(int Result, string Message, IEnumerable<OfferListItemDto>)> AdminGetAllOffersAsync(int pageNumber, int pageSize)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "ADMIN_GET_ALL_OFFER");
				p.Add("@PageNumber", pageNumber, DbType.Int32);
				p.Add("@PageSize", pageSize, DbType.Int32);
				p.Add("@Result", dbType: DbType.Int32, size: -1, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				var List = await conn.QueryAsync<OfferListItemDto>(
					"Sp_Yafa_Admin_Offers",
					p,
					commandType: CommandType.StoredProcedure
				);

				int result = p.Get<int?>("@Result") ?? 0;
				string message = p.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, List);
			}
			catch (SqlException ex) { await _logService.LogExceptionErrorAsync(ex); throw; }
		}

		public async Task<(int Result, string Message, OfferDetailDto? Data)> AdminGetOfferByIdAsync(Guid offerPublicId)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "ADMIN_GET_BY_ID");
				p.Add("@OfferPublicId", offerPublicId, DbType.Guid);
				p.Add("@Result", dbType: DbType.Int32, size: -1, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await conn.OpenAsync();

				// Two result sets: offer header + terms
				using var multi = await conn.QueryMultipleAsync(
					"Sp_Yafa_Admin_Offers", p, commandType: CommandType.StoredProcedure);

				var offer = await multi.ReadFirstOrDefaultAsync<OfferDetailDto>();
				var terms = (await multi.ReadAsync<OfferTermDto>()).ToList();

				int result = p.Get<int?>("@Result") ?? -1;
				string message = p.Get<string>("@Message") ?? "Unknown error.";

				if (result <= 0) throw new InvalidOperationException(message);

				if (offer != null)
					offer.Terms = terms;

				return (result, message, offer);
			}
			catch (SqlException ex) { await _logService.LogExceptionErrorAsync(ex); throw; }
		}

		#endregion

		#region :: Admin Fees Management

		public async Task<IEnumerable<FeesTypeDTO>> AdminGetAllFeeTypesAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "AdminGetAllFeeTypes", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var List = await sqlConnection.QueryAsync<FeesTypeDTO>(
					"Sp_Yafa_Admin_FeeManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return List;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminAddFeeAsync(AdminAddFeeRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@FeeName", request.FeeName, DbType.String);
				parameters.Add("@FeeDescription", request.FeeDescription, DbType.String);
				parameters.Add("@FeeCategory", request.FeeCategory, DbType.String);
				parameters.Add("@FeeValue", request.FeeValue, DbType.Decimal);
				parameters.Add("@ActionBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "AdminAddFeeType", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_FeeManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminUpdateFeeAsync(AdminUpdateFeeTypeDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@FeePublicId", request.FeePublicId, DbType.Guid);
				parameters.Add("@FeeName", request.FeeName, DbType.String);
				parameters.Add("@FeeDescription", request.FeeDescription, DbType.String);
				parameters.Add("@FeeCategory", request.FeeCategory, DbType.String);
				parameters.Add("@FeeValue", request.FeeValue, DbType.Decimal);
				parameters.Add("@IsMandatory", request.IsMandatory, DbType.Boolean);
				parameters.Add("@ActionBy", request.UpdatedBy, DbType.String);
				parameters.Add("@Mode", "AdminUpdateFeeType", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_FeeManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AdminToggleFeeTypeAsync(Guid feePublicId, bool isActive, string updatedBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@FeePublicId", feePublicId, DbType.Guid);
				parameters.Add("@IsActive", isActive, DbType.Boolean);
				parameters.Add("@ActionBy", updatedBy, DbType.String);
				parameters.Add("@Mode", "AdminToggleFeeType", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Admin_FeeManagement",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin Booking Calendar Management

		public async Task<(int Result, string Message, IEnumerable<CalendarBookingDto>)> GetBookingsOnCalenderAsync(CalendarBookingRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@StartDate", request.startDate, DbType.String);
				parameters.Add("@EndDate", request.endDate, DbType.String);
				parameters.Add("@PropertyPublicId", request.PropertyPublicId == Guid.Empty ? DBNull.Value : request.PropertyPublicId, DbType.Guid);
				parameters.Add("@Mode", "GetBookingsInCalender", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var bookings = await sqlConnection.QueryAsync<CalendarBookingDto>(
					"Sp_Yafa_Calender",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message, bookings);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region:: Admin SEO Management

		public async Task<(int Result, string Message, IEnumerable<SeoContentDto>)> GetSeoContentListAsync(int pageNumber, int pageSize)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();
				p.Add("@Mode", "LIST");
				p.Add("@PageNumber", pageNumber, DbType.Int32);
				p.Add("@PageSize", pageSize, DbType.Int32);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				var List = await conn.QueryAsync<SeoContentDto>(
					"Sp_SeoStorefrontContent",
					p,
					commandType: CommandType.StoredProcedure
				);

				int result = p.Get<int?>("@Result") ?? 0;
				string message = p.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, List);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddSeoContentAsync(SeoContentAddRequestDto request)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "ADD");
				p.Add("@PageKeyID", request.PageKeyID, DbType.Int32);
				p.Add("@MetaTitle", request.MetaTitle, DbType.String);
				p.Add("@MetaDescription", request.MetaDescription, DbType.String);
				p.Add("@MetaKeywords", request.MetaKeywords, DbType.String);
				p.Add("@SefUrl", request.SefUrl, DbType.String);
				p.Add("@OgTitle", request.OgTitle, DbType.String);
				p.Add("@OgDescription", request.OgDescription, DbType.String);
				p.Add("@CanonicalUrl", request.CanonicalUrl, DbType.String);
				p.Add("@UserID", request.CreatedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoStorefrontContent", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UpdateSeoContentAsync(SeoContentUpdateRequestDto request)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "UPDATE");
				p.Add("@SeoContentID", request.SeoContentID, DbType.Int32);
				p.Add("@MetaTitle", request.MetaTitle, DbType.String);
				p.Add("@MetaDescription", request.MetaDescription, DbType.String);
				p.Add("@MetaKeywords", request.MetaKeywords, DbType.String);
				p.Add("@SefUrl", request.SefUrl, DbType.String);
				p.Add("@OgTitle", request.OgTitle, DbType.String);
				p.Add("@OgDescription", request.OgDescription, DbType.String);
				p.Add("@CanonicalUrl", request.CanonicalUrl, DbType.String);
				p.Add("@UserID", request.ModifiedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoStorefrontContent", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> DeleteSeoContentAsync(int seoContentId, string userId)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "DELETE");
				p.Add("@SeoContentID", seoContentId, DbType.Int32);
				p.Add("@UserID", userId, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoStorefrontContent", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		// ---------- SeoPageMaster ----------

		public async Task<(int Result, string Message, IEnumerable<SeoPageMasterDto>)> GetSeoPageMasterListAsync(int pageNumber, int pageSize)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();
				p.Add("@Mode", "LIST");
				p.Add("@PageNumber", pageNumber, DbType.Int32);
				p.Add("@PageSize", pageSize, DbType.Int32);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				var List = await conn.QueryAsync<SeoPageMasterDto>(
					"Sp_SeoPageMaster",
					p,
					commandType: CommandType.StoredProcedure
				);

				int result = p.Get<int?>("@Result") ?? 0;
				string message = p.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, List);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddSeoPageMasterAsync(SeoPageMasterAddRequestDto request)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "ADD");
				p.Add("@PageKey", request.PageKey?.Trim(), DbType.String);
				p.Add("@PageName", request.PageName?.Trim(), DbType.String);
				p.Add("@UserID", request.CreatedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoPageMaster", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UpdateSeoPageMasterAsync(SeoPageMasterUpdateRequestDto request)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "UPDATE");
				p.Add("@PageKeyID", request.PageKeyID, DbType.Int32);
				p.Add("@PageKey", request.PageKey?.Trim(), DbType.String);
				p.Add("@PageName", request.PageName?.Trim(), DbType.String);
				p.Add("@UserID", request.ModifiedBy, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoPageMaster", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> DeleteSeoPageMasterAsync(int pageKeyId, string userId)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				var p = new DynamicParameters();

				p.Add("@Mode", "DELETE");
				p.Add("@PageKeyID", pageKeyId, DbType.Int32);
				p.Add("@UserID", userId, DbType.String);
				p.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				p.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await conn.OpenAsync();
				await conn.ExecuteAsync("Sp_SeoPageMaster", p, commandType: CommandType.StoredProcedure);

				return (p.Get<int?>("@Result") ?? -1, p.Get<string>("@Message") ?? "Unknown error.");
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Admin Dashboard Management

		private sealed class DashboardCardRow
		{
			public string CardKey { get; set; } = string.Empty;
			public decimal Value { get; set; }
			public decimal CurrentPeriodValue { get; set; }
			public decimal PreviousPeriodValue { get; set; }
			public decimal? ChangePercent { get; set; }
			public string ChangeDirection { get; set; } = "flat";
			public string ComparisonPeriod { get; set; } = "month";
		}

		public async Task<AdminDashboardDto> AdminGetDashboardSummaryAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "Summary", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Admin_Dashboard",
					parameters,
					commandType: CommandType.StoredProcedure);

				var rows = (await multi.ReadAsync<DashboardCardRow>()).ToDictionary(r => r.CardKey);
				var statuses = (await multi.ReadAsync<BookingStatusItemDto>()).ToList();

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message);

				DashboardCardRow Row(string key) => rows.TryGetValue(key, out var r) ? r : new DashboardCardRow { CardKey = key };

				CountStatCardDto CountCard(string key)
				{
					var r = Row(key);
					return new CountStatCardDto
					{
						Value = (int)r.Value,
						CurrentPeriodValue = (int)r.CurrentPeriodValue,
						PreviousPeriodValue = (int)r.PreviousPeriodValue,
						ChangePercent = r.ChangePercent,
						ChangeDirection = r.ChangeDirection,
						ComparisonPeriod = r.ComparisonPeriod
					};
				}

				var revenue = Row("Revenue");

				return new AdminDashboardDto
				{
					TotalProperties = CountCard("TotalProperties"),
					TotalBookings = CountCard("TotalBookings"),
					PendingBookings = CountCard("PendingBookings"),
					TotalUsers = CountCard("TotalUsers"),
					Revenue = new AmountStatCardDto
					{
						Value = revenue.Value,
						CurrentPeriodValue = revenue.CurrentPeriodValue,
						PreviousPeriodValue = revenue.PreviousPeriodValue,
						ChangePercent = revenue.ChangePercent,
						ChangeDirection = revenue.ChangeDirection,
						ComparisonPeriod = revenue.ComparisonPeriod
					},
					BookingStatus = statuses
				};
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<List<RevenueTrendItemDto>> AdminGetRevenueTrendAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "RevenueTrend", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var items = (await sqlConnection.QueryAsync<RevenueTrendItemDto>(
					"Sp_Yafa_Admin_Dashboard",
					parameters,
					commandType: CommandType.StoredProcedure)).ToList();

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message);

				return items;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#endregion

		#region :: User

		public async Task<int> RegisterAsync(RegisterRequestDTO model, ImageUploadResultDTO uploadResult, string UserId, string UserDetailsId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@RoleId", model.RoleId, DbType.String);
				parameters.Add("@Email", model.Email.ToLower(), DbType.String);
				parameters.Add("@Password", model.Password, DbType.String);
				parameters.Add("@AdminNotes", model.AdminNotes, DbType.String);
				parameters.Add("@CreatedBy", model.CreatedBy, DbType.String);
				parameters.Add("@UserDetailsId", UserDetailsId, DbType.String);
				parameters.Add("@Salutation", model.Salutation, DbType.String);
				parameters.Add("@FirstName", model.FirstName, DbType.String);
				parameters.Add("@MiddleName", model.MiddleName, DbType.String);
				parameters.Add("@LastName", model.LastName, DbType.String);
				parameters.Add("@Gender", model.Gender, DbType.String);
				parameters.Add("@Address1", model.Address1, DbType.String);
				parameters.Add("@Address2", model.Address2, DbType.String);
				parameters.Add("@City", model.City, DbType.String);
				parameters.Add("@State", model.State, DbType.String);
				parameters.Add("@County", model.County, DbType.String);
				parameters.Add("@Country", model.Country, DbType.String);
				parameters.Add("@ZIP", model.ZIP, DbType.String);
				parameters.Add("@Lat", model.Lat, DbType.String);
				parameters.Add("@Lang", model.Lang, DbType.String);
				parameters.Add("@PhoneNo", model.PhoneNo, DbType.String);
				parameters.Add("@PhoneNo2", model.PhoneNo2, DbType.String);
				parameters.Add("@DOB", model.DOB, DbType.String);
				parameters.Add("@ProfileImage", "", DbType.String);
				parameters.Add("@LocalPath", "", DbType.String);
				parameters.Add("@ProfileImageName", "", DbType.String);
				parameters.Add("@ThumbImage", "", DbType.String);
				parameters.Add("@ThumbImageName", "", DbType.String);
				parameters.Add("@AboutUs", model.AboutUs, DbType.String);
				parameters.Add("@Mode", "REGISTRATION", DbType.String);


				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> ForgotPasswordRequestAsync(string email, string otp, DateTime OtpExpireTime)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				// Input params
				parameters.Add("@EMAIL_ID", email, DbType.String);
				parameters.Add("@OTP", otp, DbType.String);
				parameters.Add("@OTPExpiryTime", OtpExpireTime, DbType.DateTime);
				parameters.Add("@Mode", "ForgotPasswordRequest", DbType.String);


				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				//if (result <= 0)
				//    throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> VerifyForgotPasswordOtpAsync(string email, string otp)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@EMAIL_ID", email, DbType.String);
				parameters.Add("@OTP", otp, DbType.String);
				parameters.Add("@Mode", "VerifyForgotPasswordOtp", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> StoreResetTokenAsync(string email, string resetToken, DateTime ResetTokenExpireTime)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@EMAIL_ID", email, DbType.String);
				parameters.Add("@ResetToken", resetToken, DbType.String);
				parameters.Add("@TokenExpiryTime", ResetTokenExpireTime, DbType.DateTime);
				parameters.Add("@Mode", "StoreResetToken", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> ResetPasswordAsync(string resetToken, string newPassword)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@ResetToken", resetToken, DbType.String);
				parameters.Add("@PASSWORD", newPassword, DbType.String);
				parameters.Add("@Mode", "ResetPassword", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> CheckResendOTPRateLimitAsync(string email)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@EMAIL_ID", email, DbType.String);
				parameters.Add("@Mode", "CheckResendOTPRateLimit", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> ResendOTPAsync(string email, string otp, DateTime OtpExpireTime)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@EMAIL_ID", email, DbType.String);
				parameters.Add("@OTP", otp, DbType.String);
				parameters.Add("@OTPExpiryTime", OtpExpireTime, DbType.DateTime);
				parameters.Add("@Mode", "ResendOTP", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> UpdateUserProfileImageAsync(string userId, ImageUploadResultDTO uploadResult)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@V_USER_ID", userId, DbType.String);
				parameters.Add("@ProfileImage", uploadResult.FileName, DbType.String);
				parameters.Add("@LocalPath", $"{uploadResult.LocalPath}", DbType.String);
				parameters.Add("@ProfileImageName", $"{uploadResult.WebPath}", DbType.String);
				parameters.Add("@ThumbImage", $"{uploadResult.ThumbnailImageName}", DbType.String);
				parameters.Add("@ThumbImageName", $"{uploadResult.ThumbnailWebPath}", DbType.String);
				parameters.Add("@Mode", "UpdateUserProfileImage", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");
				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserDetailsResponceDTO> GetUserDetailsByUserPublicIdAsync(Guid userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserPublicId", userId, DbType.Guid);
				parameters.Add("@mode", "UserDetailsById", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var user = await sqlConnection.QueryFirstOrDefaultAsync<UserDetailsResponceDTO>(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return user ?? new UserDetailsResponceDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<LoginResponseUserDetailsDTO> SocialLoginAsync(string email, string socialId, string firstName, string lastName, string provider, string? deviceToken, string? devicePlatform)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@mode", "SOCIAL_LOGIN");
				parameters.Add("@Email", email, DbType.String);
				parameters.Add("@ProviderUserId", socialId, DbType.String);
				parameters.Add("@FirstName", firstName, DbType.String);
				parameters.Add("@LastName", lastName, DbType.String);
				parameters.Add("@Provider", provider, DbType.String);
				parameters.Add("@DeviceToken", deviceToken, DbType.String);
				parameters.Add("@DeviceTokenPlatform", devicePlatform, DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var user = await sqlConnection.QueryFirstOrDefaultAsync<LoginResponseUserDetailsDTO>(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return user ?? new LoginResponseUserDetailsDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}

		}

		public async Task<(int Result, string Message)> ChangePasswordAsync(ChangePasswordRequestDto request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@PASSWORD", request.CurrentPassword, DbType.String);
				parameters.Add("@NewPassword", request.NewPassword, DbType.String);
				parameters.Add("@mode", "ChangePassword", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> DeactivateDeviceTokenAsync(LogoutRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@DeviceToken", request.DeviceToken, DbType.String);
				parameters.Add("@mode", "USER_LOGOUT", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Authentication",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message ?? "Unknown Error");

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}


		#endregion

		#region :: Property

		public async Task<(int Result, string Message, Guid propertyId)> AddPropertyFirstStepAsync(UserAddFirstStepPropDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", request.PropertyPublicId ?? Guid.NewGuid(), DbType.Guid);
				parameters.Add("@PropertyName", request.PropertyName, DbType.String);
				parameters.Add("@PropertyType", request.PropertyType, DbType.String);
				parameters.Add("@ShortDescription", request.ShortDescription, DbType.String);
				parameters.Add("@Description", request.Description, DbType.String);
				parameters.Add("@Address", request.Address, DbType.String);
				parameters.Add("@City", request.City, DbType.String);
				parameters.Add("@State", request.State, DbType.String);
				parameters.Add("@Country", request.Country, DbType.String);
				parameters.Add("@ZipCode", request.ZipCode, DbType.String);
				parameters.Add("@Latitude", request.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", request.Longitude, DbType.Decimal);
				parameters.Add("@TotalBedrooms", request.TotalBedrooms, DbType.Int32);
				parameters.Add("@TotalBathrooms", request.TotalBathrooms, DbType.Int32);
				parameters.Add("@MaxPersons", request.MaxPersons, DbType.Int32);
				parameters.Add("@DiscountPercentage", request.DiscountPercentage, DbType.Decimal);
				parameters.Add("@ContactFirstName", request.ContactFirstName, DbType.String);
				parameters.Add("@ContactLastName", request.ContactLastName, DbType.String);
				parameters.Add("@ContactEmail", request.ContactEmail, DbType.String);
				parameters.Add("@ContactPhone", request.ContactPhone, DbType.String);
				parameters.Add("@CreatedBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "FirstStepAddProperties", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				Guid propertyId = await sqlConnection.QuerySingleAsync<Guid>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message, propertyId);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddPropertySecondStepAsync(UserAddSecondStepPropDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", request.PropertyPublicId, DbType.Guid);
				parameters.Add("@CheckInTime", request.CheckInTime, DbType.Time);
				parameters.Add("@CheckOutTime", request.CheckOutTime, DbType.Time);
				parameters.Add("@CheckInInstructions", request.CheckInInstructions, DbType.String);
				parameters.Add("@AmenityIds", request.AmenityIds, DbType.String);
				parameters.Add("@AdditionalServiceIds", request.ServiceIDs, DbType.String);
				parameters.Add("@ServicePrices", request.ServicePrices, DbType.String);
				parameters.Add("@CreatedBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "SecondStepAddProperties", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddPropertyThirdStepAsync(UserAddThirdStepPropDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", request.PropertyPublicId, DbType.Guid);
				parameters.Add("@HouseRuleIds", request.HouseRuleIds, DbType.String);
				parameters.Add("@RefundPolicyDescription", request.RefundPolicyDescription, DbType.String);
				parameters.Add("@RefundPercentage", request.RefundPercentage, DbType.Decimal);
				parameters.Add("@RefundDaysBeforeCheckIn", request.RefundDaysBeforeCheckIn, DbType.Int32);
				parameters.Add("@MemberPrice", request.MemberPrice, DbType.Decimal);
				parameters.Add("@NonMemberPrice", request.NonMemberPrice, DbType.Decimal);
				parameters.Add("@CreatedBy", request.CreatedBy, DbType.String);
				parameters.Add("@Mode", "ThirdStepAddProperties", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddPropertyFourthStepAsync(Guid propPublicId, string jsonImages)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@PropertyImagesJson", jsonImages, DbType.String);
				parameters.Add("@Mode", "FourthStepAddProperties", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> FinalSubmitPropertyAsync(Guid propPublicId, string createdBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", createdBy, DbType.String);
				parameters.Add("@Mode", "FinalSubmit", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<AmenitiesResponceDTO>> GetAllAmenitiesAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllAmenities", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var amenitiesList = await sqlConnection.QueryAsync<AmenitiesResponceDTO>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return amenitiesList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<AdditionalServicesResponceDTO>> GetAllAdditionalServicesAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllAdditionalServices", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var servicesList = await sqlConnection.QueryAsync<AdditionalServicesResponceDTO>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return servicesList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<HouseRulesResponceDTO>> GetAllHouseRulesAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllHouseRules", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var houseRulesList = await sqlConnection.QueryAsync<HouseRulesResponceDTO>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return houseRulesList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserFirstStepPropResponceDTO> GetPropertyDetailsFirstStepAsync(string createdBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@CreatedBy", createdBy, DbType.String);
				parameters.Add("@Mode", "GetAlreadyAddedFirstStepDetails", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var propertyDetails = await sqlConnection.QueryFirstOrDefaultAsync<UserFirstStepPropResponceDTO>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				return propertyDetails ?? new UserFirstStepPropResponceDTO();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserSecondStepPropResponceDTO?> GetPropertyDetailsSecondStepAsync(Guid propertyPublicId, string createdBy)
		{
			try
			{

				using var connection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@mode", "GetAlreadyAddedSecondStepDetails");
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", createdBy, DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await connection.OpenAsync();

				using var multi = await connection.QueryMultipleAsync(
					"[Sp_Yafa_User_Properties]",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				var basicDetails = await multi.ReadFirstOrDefaultAsync<PropertyBasicDetailsDto>();
				var amenities = (await multi.ReadAsync<PropertyAmenityDto>()).ToList();
				var services = (await multi.ReadAsync<PropertyAdditionalServiceDto>()).ToList();

				return new UserSecondStepPropResponceDTO
				{
					BasicDetails = basicDetails ?? new(),
					Amenities = amenities ?? new(),
					AdditionalServices = services ?? new()
				};
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserThirdStepPropResponceDTO?> GetPropertyDetailsThirdStepAsync(Guid propertyPublicId, string createdBy)
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@mode", "GetAlreadyAddedThirdStepDetails");
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", createdBy, DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await connection.OpenAsync();
				using var multi = await connection.QueryMultipleAsync(
					"[Sp_Yafa_User_Properties]",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				var rules = (await multi.ReadAsync<PropertyRules>()).ToList();
				var refundPolicy = await multi.ReadFirstOrDefaultAsync<PropertyRefundPolicies>();
				var pricingTiers = (await multi.ReadAsync<PropertyPricingTiers>()).ToList();
				return new UserThirdStepPropResponceDTO
				{
					RefundPolicyDetails = refundPolicy ?? new(),
					Rules = rules ?? new(),
					PriceingTiers = pricingTiers ?? new()
				};
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<UserFourthStepPropResponceDTO?> GetPropertyDetailsFourthStepAsync(Guid propertyPublicId, string createdBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", createdBy, DbType.String);
				parameters.Add("@Mode", "GetAlreadyAddedFourthStepDetails", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				//var propertyDetails = await sqlConnection.QueryFirstOrDefaultAsync<UserFourthStepPropResponceDTO>(
				//    "Sp_Yafa_User_Properties",
				//    parameters,
				//    commandType: CommandType.StoredProcedure
				//);

				var images = (await sqlConnection.QueryAsync<PropertyImages>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				)).ToList();

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return new UserFourthStepPropResponceDTO
				{
					Images = images
				};
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<PropSearchResponseDTO>> SearchPropertiesAsync(PropSearchRequestDTO request, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@Latitude", request.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", request.Longitude, DbType.Decimal);
				parameters.Add("@Guests", request.Guests, DbType.Int32);
				parameters.Add("@AmenityIds", request.AmenityIds, DbType.String);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", request.PageSize, DbType.Int32);
				parameters.Add("@mode", "GetPropertiesDetailsByUserSearch", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var propertyList = await sqlConnection.QueryAsync<PropSearchResponseDTO>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return propertyList;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<Properties>> GetAllPropForDeveloper()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllPropForDev", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				var propList = await sqlConnection.QueryAsync<Properties>(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return propList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<PropertyDetailsDto> GetPropertyDetailsByPublicIdAsync(Guid propertyPublicId, string userId, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@mode", "GetPropertiesDetailsByPropPublicId", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				using var multi = await sqlConnection.QueryMultipleAsync(cmd);

				var property = multi.Read<PropertyDetailsDto>().SingleOrDefault();

				// Images
				property!.Images = multi.Read<PropertyImages>().ToList();

				// Amenities
				property.Amenities = multi.Read<PropertyAmenityDto>().ToList();

				// Additional Services
				property.AdditionalServices = multi.Read<PropertyAdditionalServiceDto>().ToList();

				// House Rules
				property.HouseRules = multi.Read<PropertyRules>().ToList();

				// Refund Policies
				property.RefundPolicy = multi.Read<PropertyRefundPolicies>().ToList();

				// Pricing Tiers
				property.PricingTiers = multi.Read<PropertyPricingTiers>().ToList();

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);

				return property;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<PropRatingDetailsDTO> GetPropertyDetailsRatingsByPropPublicIdAsync(Guid propertyPublicId, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PropertyPublicId", propertyPublicId, DbType.Guid);
				parameters.Add("@mode", "GetPropertyReviews", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_User_Properties_Review_Rateing",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				using var multi = await sqlConnection.QueryMultipleAsync(cmd);

				var propertyRating = multi.Read<PropRatingDetailsDTO>().SingleOrDefault();

				// Images
				propertyRating!.propReviews = multi.Read<PropReviewsDTO>().ToList();

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);

				return propertyRating;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddPropertyAvailabilityAsync(Guid propPublicId, string createdBy)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@CreatedBy", createdBy, DbType.String);
				parameters.Add("@Mode", "Add_Property_Availability", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<PropertyUnavailableDatesDTO> GetPropertyUnavailableDatesAsync(Guid propPublicId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@mode", "GetPropertyUnavailableDates", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var raw = await sqlConnection.QueryFirstOrDefaultAsync<dynamic>(cmd);


				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return new PropertyUnavailableDatesDTO
				{
					ConfirmedDates = ParseDates((string?)raw?.ConfirmedDates),
					HoldDates = ParseDates((string?)raw?.HoldDates),
					BlockedDates = ParseDates((string?)raw?.BlockedDates)
				};
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		private static List<string> ParseDates(string? raw) =>
			string.IsNullOrWhiteSpace(raw)
				? new List<string>()
				: raw.Split(',').ToList();


		public async Task<(int Result, string Message, IEnumerable<OwnerPropertyResponceDTO>)> GetAllOwnerPropertyListAsync(string OwnerId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@mode", "GetAllOwnerPropertyList", DbType.String);
				parameters.Add("@CreatedBy", OwnerId, DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var propList = await sqlConnection.QueryAsync<OwnerPropertyResponceDTO>(
					"Sp_Yafa_User_Properties",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, propList);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UpdateFullPropertyAsync(AdminUpdateFullPropertyDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", req.PropertyPublicId, DbType.Guid);
				parameters.Add("@PropertyName", req.PropertyName, DbType.String);
				parameters.Add("@PropertyType", req.PropertyType, DbType.String);
				parameters.Add("@ShortDescription", req.ShortDescription, DbType.String);
				parameters.Add("@Description", req.Description, DbType.String);
				parameters.Add("@Address", req.Address, DbType.String);
				parameters.Add("@City", req.City, DbType.String);
				parameters.Add("@State", req.State, DbType.String);
				parameters.Add("@Country", req.Country, DbType.String);
				parameters.Add("@ZipCode", req.ZipCode, DbType.String);
				parameters.Add("@Latitude", req.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", req.Longitude, DbType.Decimal);
				parameters.Add("@TotalBedrooms", req.TotalBedrooms, DbType.Int32);
				parameters.Add("@TotalBathrooms", req.TotalBathrooms, DbType.Int32);
				parameters.Add("@MaxPersons", req.MaxPersons, DbType.Int32);
				parameters.Add("@DiscountPercentage", req.DiscountPercentage, DbType.Decimal);
				parameters.Add("@ContactFirstName", req.ContactFirstName, DbType.String);
				parameters.Add("@ContactLastName", req.ContactLastName, DbType.String);
				parameters.Add("@ContactEmail", req.ContactEmail, DbType.String);
				parameters.Add("@ContactPhone", req.ContactPhone, DbType.String);

				parameters.Add("@CheckInTime", req.CheckInTime, DbType.Time);
				parameters.Add("@CheckOutTime", req.CheckOutTime, DbType.Time);
				parameters.Add("@CheckInInstructions", req.CheckInInstructions, DbType.String);
				parameters.Add("@AmenityIds", req.AmenityIds, DbType.String);
				parameters.Add("@AdditionalServiceIds", req.ServiceIDs, DbType.String);
				parameters.Add("@ServicePrices", req.ServicePrices, DbType.String);

				parameters.Add("@HouseRuleIds", req.HouseRuleIds, DbType.String);
				parameters.Add("@RefundPolicyDescription", req.RefundPolicyDescription, DbType.String);
				parameters.Add("@RefundPercentage", req.RefundPercentage, DbType.Decimal);
				parameters.Add("@RefundDaysBeforeCheckIn", req.RefundDaysBeforeCheckIn, DbType.Int32);
				parameters.Add("@MemberPrice", req.MemberPrice, DbType.Decimal);
				parameters.Add("@NonMemberPrice", req.NonMemberPrice, DbType.Decimal);


				parameters.Add("@Mode", "UpdateProperty", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync("Sp_Yafa_User_Properties", parameters, commandType: CommandType.StoredProcedure);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<PropSearchResponseDTO>> IsPopularPropertyBasedOnCurrentLocationAsync(IsPopularPropReqDTO req, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@Latitude", req.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", req.Longitude, DbType.Decimal);
				parameters.Add("@UserId", req.LoggedInUserId, DbType.String);
				parameters.Add("@PageNumber", req.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", req.PageSize, DbType.Int32);
				parameters.Add("@mode", "TopIsPopularPropertyBasedOnCurrentLocation", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var propertyList = await sqlConnection.QueryAsync<PropSearchResponseDTO>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return propertyList;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<PropSearchResponseDTO>> IsFeaturedPropertyBasedOnCurrentLocationAsync(IsFeaturedPropReqDTO req, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@Latitude", req.Latitude, DbType.Decimal);
				parameters.Add("@Longitude", req.Longitude, DbType.Decimal);
				parameters.Add("@UserId", req.LoggedInUserId, DbType.String);
				parameters.Add("@PageNumber", req.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", req.PageSize, DbType.Int32);
				parameters.Add("@mode", "TopIsFeaturedPropertyBasedOnCurrentLocation", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);
				var cmd = new CommandDefinition(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var propertyList = await sqlConnection.QueryAsync<PropSearchResponseDTO>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return propertyList;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, IEnumerable<TopLocationDto>)> GetTopLocationsByCityAsync(string City, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@City", City, DbType.String);
				parameters.Add("@mode", "GetTopLocationsByCity", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);
				var cmd = new CommandDefinition(
					"Sp_Yafa_Properties_Search",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var List = await sqlConnection.QueryAsync<TopLocationDto>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return (result, message, List);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, IEnumerable<CouponOfferResponceDto>)> GetAvailableCouponsForOfferPageAsync(string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@mode", "GetAvailableCouponsOnOffer", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var list = await sqlConnection.QueryAsync<CouponOfferResponceDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message, list);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}


		#endregion

		#region :: Booking

		public async Task<PropertySelectInfoForBookingDTO> GetPropertySelectDetailsForBookingAsync(Guid propPublicId, string UserId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PropertyPublicId", propPublicId, DbType.Guid);
				parameters.Add("@UserId", "UserId", DbType.String);
				parameters.Add("@mode", "GetPropertySelectDetails", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var propList = await sqlConnection.QueryFirstOrDefaultAsync<PropertySelectInfoForBookingDTO>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result <= 0)
					throw new InvalidOperationException(message);
				return propList ?? new();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, BookingHoldResultDTO)> CreateBookingHoldAsync(BookingHoldRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", request.PropertyPublicId, DbType.Guid);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@CheckInDate", request.CheckInDate, DbType.Date);
				parameters.Add("@CheckOutDate", request.CheckOutDate, DbType.Date);
				parameters.Add("@NumberOfGuests", request.numberOfGuests, DbType.Int32);
				parameters.Add("@mode", "CreateBookingHold", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure);

				BookingHoldResultDTO? holdData = null;
				List<BookingFeeDTO> feeBreakdown = new();

				try
				{
					holdData = await multi.ReadFirstOrDefaultAsync<BookingHoldResultDTO>();
					feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
				}
				catch
				{
				}

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (holdData is not null)
					holdData.FeeBreakdown = feeBreakdown;

				return (result, message, holdData ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}



		//public async Task<(int Result, string Message, ConfirmPaymentResponseDto)> ConfirmPaymentAsync(ConfirmPaymentRequest request)
		//{
		//    try
		//    {
		//        using var sqlConnection = new SqlConnection(_connectionString);
		//        var parameters = new DynamicParameters();
		//        // Input params
		//        parameters.Add("@BookingPublicId", request.BookingPublicId, DbType.Guid);
		//        parameters.Add("@HoldPublicId", request.HoldPublicId, DbType.Guid);
		//        parameters.Add("@PaymentMethod", request.PaymentMethod, DbType.String);
		//        parameters.Add("@PaymentTransactionId", request.PaymentTransactionId, DbType.String);
		//        parameters.Add("@UserId", request.UserId, DbType.String);
		//        parameters.Add("@mode", "ConfirmPayment", DbType.String);

		//        // Output params
		//        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
		//        parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

		//        await sqlConnection.OpenAsync();

		//        using var multi = await sqlConnection.QueryMultipleAsync(
		//            "Sp_Yafa_Booking",
		//            parameters,
		//            commandType: CommandType.StoredProcedure);

		//        ConfirmPaymentResponseDto? PaymentCreateResponse = null;
		//        List<BookingFeeDTO> feeBreakdown = new();

		//        try
		//        {
		//            PaymentCreateResponse = await multi.ReadFirstOrDefaultAsync<ConfirmPaymentResponseDto>();
		//            feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
		//        }
		//        catch
		//        {
		//        }

		//        // Read outputs safely
		//        int result = parameters.Get<int?>("@Result") ?? -1;
		//        string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

		//        if (PaymentCreateResponse is not null)
		//            PaymentCreateResponse.FeeBreakdown = feeBreakdown;

		//        return (result, message, PaymentCreateResponse ?? new());
		//    }
		//    catch (SqlException ex)
		//    {
		//        await _logService.LogExceptionErrorAsync(ex);
		//        throw;
		//    }
		//}

		public async Task<(int Result, string Message, IEnumerable<UserBookingListItemDto>)> GetUserBookingListItemAsync(UserBookedDetailRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@SearchText", request.Search ?? string.Empty, DbType.String);
				parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", request.PageSize, DbType.Int32);
				parameters.Add("@mode", "GetUserBookings", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var AllBookedList = await sqlConnection.QueryAsync<UserBookingListItemDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message, AllBookedList);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, BookingDetailsResponceDto)> GetBookingDetailsByBookingIdAndUserIdAsync(Guid BookingPublicId, string UserId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", BookingPublicId, DbType.Guid);
				parameters.Add("@UserId", UserId, DbType.String);
				parameters.Add("@mode", "GetBookingDetails", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				BookingDetailsResponceDto? bookingdetails = null;
				List<BookingPropertyImageDto>? bookingPropertyImages = new();
				List<BookingFeeDTO>? feeBreakdown = new();
				List<BookingPaymentSlotDto>? paymentPlanSlots = new();
				//List<PaymentTransactionHistoryDto>? paymentTransactionHistory = new();

				try
				{
					bookingdetails = await multi.ReadFirstOrDefaultAsync<BookingDetailsResponceDto>();
					bookingPropertyImages = (await multi.ReadAsync<BookingPropertyImageDto>()).ToList();
					feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
					paymentPlanSlots = (await multi.ReadAsync<BookingPaymentSlotDto>()).ToList();
					//paymentTransactionHistory = (await multi.ReadAsync<PaymentTransactionHistoryDto>()).ToList();
				}
				catch
				{
				}


				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (bookingdetails is not null)
				{
					bookingdetails.Images = bookingPropertyImages;
					bookingdetails.FeeBreakdown = feeBreakdown;
					bookingdetails.PaymentPlan = paymentPlanSlots ?? new();
					//bookingdetails.PaymentHistory = paymentTransactionHistory;
				}


				return (result, message, bookingdetails ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, string? OwnerUserId, string? PropertyName)> UserCancelBookingAsync(UserCancelBookingRequestDto req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
				parameters.Add("@UserId", req.CancelBy, DbType.String);
				parameters.Add("@CancellationReason", req.CancellationReason, DbType.String);
				parameters.Add("@mode", "UserCancelBooking", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var resultData = await sqlConnection.QueryFirstOrDefaultAsync<(string OwnerUserId, string PropertyName)>(cmd);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, resultData.OwnerUserId, resultData.PropertyName);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		//public async Task<(int Result, string Message, UserExtendBookingResponseDto responseDto)> UserExtendBookingAsync(UserExtendBookingRequestDto req, DateOnly? newCheckOut)
		//{
		//	try
		//	{
		//		using var sqlConnection = new SqlConnection(_connectionString);
		//		var parameters = new DynamicParameters();
		//		// Input params
		//		parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
		//		parameters.Add("@UserId", req.ExtendBy, DbType.String);
		//		parameters.Add("@NewCheckOutDate", newCheckOut, DbType.Date);
		//		parameters.Add("@NewNumberOfGuests", req.NewNumberOfGuests, DbType.Int32);
		//		parameters.Add("@mode", "UserExtendBooking", DbType.String);
		//		// Output params
		//		parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
		//		parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
		//		await sqlConnection.OpenAsync();


		//		using var multi = await sqlConnection.QueryMultipleAsync(
		//			"Sp_Yafa_Booking",
		//			parameters,
		//			commandType: CommandType.StoredProcedure
		//		);

		//		UserExtendBookingResponseDto? extensionResponse = null;
		//		List<BookingFeeDTO>? feeBreakdown = new();

		//		try
		//		{
		//			extensionResponse = await multi.ReadFirstOrDefaultAsync<UserExtendBookingResponseDto>();
		//			feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
		//		}
		//		catch
		//		{
		//		}

		//		int result = parameters.Get<int?>("@Result") ?? 0;
		//		string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";


		//		if (extensionResponse is not null)
		//			extensionResponse.FeeBreakdown = feeBreakdown;

		//		return (result, message, extensionResponse ?? new());

		//	}
		//	catch (SqlException ex)
		//	{
		//		await _logService.LogExceptionErrorAsync(ex);
		//		throw;
		//	}
		//}

		public async Task<(int Result, string Message, string? OwnerUserId)> UserRateAndReviewAsync(PropRatingRequestDTO req)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@PropertyPublicId", req.PropertyPublicId, DbType.Guid);
				parameters.Add("@Rating", req.Rating, DbType.Decimal);
				parameters.Add("@UserId", req.UserId, DbType.String);
				parameters.Add("@BookingPublicId", req.BookingPublicId, DbType.Guid);
				parameters.Add("@Comment", req.Comment, DbType.String);
				parameters.Add("@CheckInDate", req.CheckInDate, DbType.Date);
				parameters.Add("@StayDuration", req.StayDuration, DbType.Int32);
				parameters.Add("@mode", "AddUserReview", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_User_Properties_Review_Rateing",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var OwnerUserId = await sqlConnection.QueryFirstOrDefaultAsync<string?>(cmd);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return (result, message, OwnerUserId);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, IEnumerable<CheckoutCouponDto>)> GetAvailableCouponsForCheckoutAsync(Guid holdPublicId, string userId, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@HoldPublicId", holdPublicId, DbType.Guid);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@mode", "GetAvailableCoupons", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);
				var cmd = new CommandDefinition(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var list = await sqlConnection.QueryAsync<CheckoutCouponDto>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message, list);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<PaymentResultDto> PaymentResultAsync(Guid paymentPublicId, string userId, CancellationToken cancellationToken = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@PaymentPublicId", paymentPublicId, DbType.Guid);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@mode", "GetPaymentResult", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync(cancellationToken);
				var cmd = new CommandDefinition(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: cancellationToken
				);
				var paymentResult = await sqlConnection.QueryFirstOrDefaultAsync<PaymentResultDto>(cmd);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return paymentResult ?? new PaymentResultDto();
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
			catch (Exception ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, ExtensionHoldResultDTO)> CreateExtensionHoldAsync(CreateExtensionHoldRequest request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", request.BookingPublicId, DbType.Guid);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@NewCheckOutDate", request.NewCheckOutDate, DbType.Date);
				parameters.Add("@mode", "CreateExtensionHold", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure);

				ExtensionHoldResultDTO? holdData = null;
				List<BookingFeeDTO> feeBreakdown = new();

				try
				{
					holdData = await multi.ReadFirstOrDefaultAsync<ExtensionHoldResultDTO>();
					feeBreakdown = (await multi.ReadAsync<BookingFeeDTO>()).ToList();
				}
				catch
				{
				}

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (holdData is not null)
					holdData.FeeBreakdown = feeBreakdown;

				return (result, message, holdData ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

		#region :: Payment

		public async Task<(int Result, string Message, InitiatePaymentResponseDto)> InitiatePaymentAsync(InitiatePaymentRequest request, string paymentPlanType, decimal? depositPercentage)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				// Input params
				parameters.Add("@HoldPublicId", request.HoldPublicId, DbType.Guid);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@OfferPublicId", request.OfferPublicId.HasValue ? (object)request.OfferPublicId.Value : DBNull.Value, DbType.Guid);
				parameters.Add("@Currency", "INR", DbType.String);
				parameters.Add("@PaymentPlanType", paymentPlanType, DbType.String);
				parameters.Add("@DepositPercentage", depositPercentage.HasValue ? (object)depositPercentage.Value : DBNull.Value, DbType.Decimal);
				parameters.Add("@mode", "InitiatePayment", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var PaymentResponse = await sqlConnection.QueryFirstOrDefaultAsync<InitiatePaymentResponseDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				// Read outputs safely
				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message, PaymentResponse ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> UpdateStripePaymentIntentAsync(UpdateStripePaymentIntentRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@PaymentPublicId", request.PaymentPublicId, DbType.Guid);
				parameters.Add("@PaymentIntentId", request.PaymentIntentId, DbType.String);
				parameters.Add("@CustomerId", request.CustomerId, DbType.String);
				parameters.Add("@StripeStatus", request.StripeStatus, DbType.String);
				parameters.Add("@Currency", request.Currency, DbType.String);
				parameters.Add("@ResponseJson", request.ResponseJson, DbType.String);
				parameters.Add("@mode", "UpdateStripePaymentIntent", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync("Sp_Yafa_Booking", parameters, commandType: CommandType.StoredProcedure);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, StripeWebhookResponseDto)> ProcessStripeWebhookAsync(ProcessStripeWebhookRequestDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@StripeEventId", request.StripeEventId, DbType.String);
				parameters.Add("@PaymentIntentId", request.PaymentIntentId, DbType.String);
				parameters.Add("@EventType", request.EventType, DbType.String);
				parameters.Add("@PaymentIntentStatus", request.PaymentIntentStatus, DbType.String);
				parameters.Add("@CustomerId", request.CustomerId, DbType.String);
				parameters.Add("@PaymentMethodId", request.PaymentMethodId, DbType.String);
				parameters.Add("@ChargeId", request.ChargeId, DbType.String);
				parameters.Add("@Payload", request.Payload, DbType.String);
				parameters.Add("@CardBrand", request.CardBrand, DbType.String);
				parameters.Add("@CardLast4", request.CardLast4, DbType.String);
				parameters.Add("@mode", "ProcessStripeWebhook", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var stripeWebhookResponse = await sqlConnection.QueryFirstOrDefaultAsync<StripeWebhookResponseDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message, stripeWebhookResponse ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, BalancePaymentResponseDto)> InitiateBalancePaymentAsync(InitiateBalancePaymentRequest request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", request.BookingPublicId, DbType.Guid);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@Currency", "INR", DbType.String);
				parameters.Add("@mode", "InitiateBalancePayment", DbType.String);

				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var balanceResponse = await sqlConnection.QueryFirstOrDefaultAsync<BalancePaymentResponseDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message, balanceResponse ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> CancelExpiredPartialBookingsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "CancelExpiredPartialBookings", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				await sqlConnection.ExecuteAsync(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<BalancePaymentReminderDto>> GetPendingBalancePaymentRemindersAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "GetPendingBalancePaymentReminders", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var list = await sqlConnection.QueryAsync<BalancePaymentReminderDto>(cmd);

				return list;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, ExtensionPaymentResponseDto)> InitiateExtensionPaymentAsync(InitiateExtensionPaymentRequest request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@ExtensionHoldPublicId", request.ExtensionHoldPublicId, DbType.Guid);
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@Currency", "INR", DbType.String);
				parameters.Add("@mode", "InitiateExtensionPayment", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var extensionResponse = await sqlConnection.QueryFirstOrDefaultAsync<ExtensionPaymentResponseDto>(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";
				return (result, message, extensionResponse ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message, InvoiceResponseDto)> GetOrCreateInvoiceAsync(Guid bookingPublicId, string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@BookingPublicId", bookingPublicId, DbType.Guid);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@mode", "GetOrCreateInvoice", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				using var multi = await sqlConnection.QueryMultipleAsync(
					"Sp_Yafa_Booking",
					parameters,
					commandType: CommandType.StoredProcedure);

				InvoiceResponseDto? invoice = null;
				List<InvoiceLineItemDto> lineItems = new();
				List<InvoicePaymentDto> payments = new();

				try
				{
					invoice = await multi.ReadFirstOrDefaultAsync<InvoiceResponseDto>();
					lineItems = (await multi.ReadAsync<InvoiceLineItemDto>()).ToList();
					payments = (await multi.ReadAsync<InvoicePaymentDto>()).ToList();
				}
				catch
				{
				}

				int result = parameters.Get<int?>("@Result") ?? -1;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (invoice is not null)
				{
					invoice.LineItems = lineItems;
					invoice.Payments = payments;
				}

				return (result, message, invoice ?? new());
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}



		#endregion

		#region :: Wishlist
		public async Task<IEnumerable<WishListResponceDTO>> GetUserWishListPropertiesAsync(WishListRequestDTO request, CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
				parameters.Add("@PageSize", request.PageSize, DbType.Int32);
				parameters.Add("@mode", "GetUserWishlist", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Wishlist",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var wishList = await sqlConnection.QueryAsync<WishListResponceDTO>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";
				if (result <= 0)
					throw new InvalidOperationException(message);
				return wishList;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> AddRemoveWishListPropertyAsync(AddRemoveWishListPropertyDTO request)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", request.UserId, DbType.String);
				parameters.Add("@WishlistPublicId", Guid.NewGuid(), DbType.Guid);
				parameters.Add("@PropertyPublicId", request.PropertyPublicId, DbType.Guid);
				parameters.Add("@mode", request.IsAdding ? "AddToWishlist" : "RemoveFromWishlist", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();
				await sqlConnection.ExecuteAsync(
					"Sp_Yafa_Wishlist",
					parameters,
					commandType: CommandType.StoredProcedure
					);
				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}



		#endregion

		#region :: Notification

		public async Task<IEnumerable<ActiveDeviceTokenDTO>> GetActiveDeviceTokensAsync()
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();


				parameters.Add("@Mode", "GetActiveDeviceTokens", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var activeDeviceTokens = await sqlConnection.QueryAsync<ActiveDeviceTokenDTO>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result < 0)
					throw new InvalidOperationException(message);
				return activeDeviceTokens;

			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<int> DeactivateInvalidTokensAsync(List<string> tokens)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@TokenList", string.Join(',', tokens), DbType.String);
				parameters.Add("@Mode", "DeactivateInvalidTokens", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (result < 0)
					throw new InvalidOperationException(message);

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> InsertBulkNotificationAsync(BroadcastNotificationRequestDTO request, int sentCount, int failedCount)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Title", request.Title, DbType.String);
				parameters.Add("@Body", request.Body, DbType.String);
				parameters.Add("@SentCount", sentCount, DbType.Int32);
				parameters.Add("@FailedCount", failedCount, DbType.Int32);
				parameters.Add("@ReferencePublicId", request.ReferencePublicId, DbType.String);
				parameters.Add("@Type", NotificationType.Offer ?? "OFFER", DbType.String);
				parameters.Add("@Mode", "InsertBulkNotification", DbType.String);
				// Output params
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<List<string>> GetUserActiveDeviceTokenAsync(string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@Mode", "GetUserActiveDeviceToken", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var result = await sqlConnection.QueryAsync<string>(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return result.ToList();
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> InsertSingleNotificationAsync(string userId, string title, string message, string type, string? referenceId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@Title", title, DbType.String);
				parameters.Add("@Body", message, DbType.String);
				parameters.Add("@Type", type, DbType.String);
				parameters.Add("@ReferencePublicId", referenceId, DbType.String);
				parameters.Add("@Mode", "InsertSingleNotification", DbType.String);

				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();
				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string msg = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				return (dbResult, msg);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<long> GetUnreadNotificationCountAsync(string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetUnreadCount", DbType.String);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var result = await sqlConnection.QueryFirstOrDefaultAsync<long>(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return result;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync(string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "GetAllNotifications", DbType.String);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				var list = await sqlConnection.QueryAsync<NotificationResponseDTO>(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return list;
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> MarkNotificationReadAsync(long notificationId, string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "MarkSingleRead", DbType.String);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@NotificationId", notificationId, DbType.Int64);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return (dbResult, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> MarkAllNotificationsReadAsync(string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "MarkAllRead", DbType.String);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return (dbResult, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> DeleteNotificationAsync(long notificationId, string userId)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);

				var parameters = new DynamicParameters();
				parameters.Add("@Mode", "DeleteNotification", DbType.String);
				parameters.Add("@UserId", userId, DbType.String);
				parameters.Add("@NotificationId", notificationId, DbType.Int64);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync();

				var cmd = new CommandDefinition(
					"Sp_Yafa_Notifications",
					parameters,
					commandType: CommandType.StoredProcedure
				);

				await sqlConnection.ExecuteAsync(cmd);

				int dbResult = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown response from database.";

				if (dbResult <= 0)
					throw new InvalidOperationException(message);

				return (dbResult, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}


		#endregion

		#region :: Background Jobs

		public async Task<IEnumerable<CheckInReminderDto>> GetTomorrowCheckInsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "GetTomorrowCheckIns", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var list = await sqlConnection.QueryAsync<CheckInReminderDto>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					return Enumerable.Empty<CheckInReminderDto>();

				return list;
			}
			catch (OperationCanceledException) { throw; }
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<CheckInReminderDto>> GetTodayCheckInsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "GetTodayCheckIns", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var list = await sqlConnection.QueryAsync<CheckInReminderDto>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					return Enumerable.Empty<CheckInReminderDto>();

				return list;
			}
			catch (OperationCanceledException) { throw; }
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<IEnumerable<CheckInReminderDto>> GetTodayCheckOutsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "GetTodayCheckOuts", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				var list = await sqlConnection.QueryAsync<CheckInReminderDto>(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				if (result <= 0)
					throw new InvalidOperationException(message);

				return list;
			}
			catch (OperationCanceledException) { throw; }
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> ReleaseExpiredHoldsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "ReleaseExpiredHolds", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				await sqlConnection.ExecuteAsync(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (OperationCanceledException) { throw; }
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		public async Task<(int Result, string Message)> ReleaseExpiredExtensionHoldsAsync(CancellationToken ct = default)
		{
			try
			{
				using var sqlConnection = new SqlConnection(_connectionString);
				var parameters = new DynamicParameters();

				parameters.Add("@Mode", "ReleaseExpiredExtensionHolds", DbType.String);
				parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

				await sqlConnection.OpenAsync(ct);

				var cmd = new CommandDefinition(
					"Sp_Yafa_Background_Service",
					parameters,
					commandType: CommandType.StoredProcedure,
					cancellationToken: ct
				);

				await sqlConnection.ExecuteAsync(cmd);

				int result = parameters.Get<int?>("@Result") ?? 0;
				string message = parameters.Get<string>("@Message") ?? "Unknown error from DB";

				return (result, message);
			}
			catch (SqlException ex)
			{
				await _logService.LogExceptionErrorAsync(ex);
				throw;
			}
		}

		#endregion

	}
}
