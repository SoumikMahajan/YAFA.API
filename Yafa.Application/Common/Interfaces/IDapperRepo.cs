using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.Payments;
using Yafa.Domain.Dtos.Property;
using Yafa.Domain.Dtos.User;
using Yafa.Domain.Models;

namespace Yafa.Application.Common.Interfaces
{
	public interface IDapperRepo
	{
		#region :: Admin

		#region :: Auth

		Task<int> AdminRegisterAsync(AdminRegisterRequestDTO model, ImageUploadResultDTO uploadResult, string UserId, string UserDetailsId);

		Task<LoginResponseUserDetailsDTO> AdminLoginAsync(LoginRequestDTO model);
		Task<LoginResponseUserDetailsDTO> UserLoginAsync(LoginRequestDTO model);
		Task<(int Result, string Message, string)> DecryptPassAsync(string Email);
		#endregion

		#region :: Admin User Management
		Task<IEnumerable<UserDetailsDTO>> GetAllUserListAsync(int pageNumber, int pageSize, string? searchKeyword);
		Task<UserDetailsDTO> GetUserDetailsByIdAsync(string UserId);
		Task<int> AdminAddNewUserAsync(AddUserRequestDTO model, string UserId, string UserDetailsId);
		Task<int> AdminUpdateUserAsync(UpdateUserRequestDTO Updatemodel);
		Task<int> AdminNewUserUpdateUserProfileImageAsync(string userId, ImageUploadResultDTO uploadResult);
		Task<int> AdminUserDeleteAsycn(string UserId, string updatedBy);
		#endregion

		#region :: Admin Prop Management
		Task<(int Result, string Message)> AddBlockPropertyDatesAsync(BlockPropRequestDTO request);
		Task<(int Result, string Message)> UnBlockPropertyDatesDatesAsync(Guid UnavailablePublicId);
		Task<IEnumerable<BlockPropDatesDetailsDTO>> BlockPropDatesDetailsAsync(Guid PropertyPublicId);
		Task<IEnumerable<FinalSubmitPropertyResponceDTO>> GetAllFinalSubmitPropertiesAsync(AdminPropertySearchRequestDTO request);
		Task<(int Result, string Message, AllBookedDetailResponseDTO)> AdminBookingListItemAsync(AllBookedDetailRequestDTO request);
		Task<(int Result, string Message, AdminBookingDetailsDto)> AdminBookingDeatilsByIdAsync(Guid BookingPublicId);
		Task<(int Result, string Message, AdminAddNewBookingResponseDto)> AdminAddNewBookingAsync(AdminAddNewBookingRequestDTO req);
		Task<(int Result, string Message)> AdminPanelConfirmBookingAsync(Guid BookingPublicId, string CreatedBy);
		Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCheckInAsync(Guid BookingPublicId, string CreatedBy);
		Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCheckOutAsync(Guid BookingPublicId, string CreatedBy);
		Task<(int Result, string Message, string? UserId, string? PropertyName)> AdminPanelUpdateBookingToCancelBookingAsync(CancelBookingRequestDTO req);
		Task<(int Result, string Message)> AdminPanelUpdateBookingToProcessRefundAsync(RefundBookingRequestDTO req);
		Task<(int Result, string Message, string? UserId, string? PropertyName, DateOnly? NewCheckOutDate)> AdminPanelExtendBookingAsync(ExtendBookingRequestDTO req);
		Task<(int Result, string Message)> AdminPanelUpdatePropertyIsPopularAsync(Guid propertyPublicId, bool isPopular);
		Task<(int Result, string Message)> AdminPanelUpdatePropertyIsFeaturedAsync(Guid propertyPublicId, bool isFeatured);
		Task<IEnumerable<PropertyImagesOnAdminPanelDTO>> AdminGetAllPropertyImagesAsync(Guid propertyPublicId);
		Task<(int Result, string Message)> AdminDeletePropertyImageAsync(Guid imagePublicId, string DeletedBy);
		Task<(int Result, string Message, int LatestDisplayOrder)> GetLatestPropertyImageDisplayOrderAsync(Guid PropertyPublicId);
		Task<(int Result, string Message)> AddNewPropertyImagesAsync(Guid propPublicId, string CreatedBy, string jsonImages);
		Task<(int Result, string Message)> UpdatePropertyCoverImageAsync(Guid propertyPublicId, Guid coverImagePublicId, string updateBy);
		Task<(int Result, string Message)> InsertFullPropertyAsync(AdminInsertFullPropertyDTO req, Guid PropertyPublicId);
		Task<CheckoutDetailsResponseDto> AdminGetCheckOutDetailsAsync(Guid BookingPublicId);
		Task<(int Result, string Message)> AdminAddExtraFeeAsync(AddExtraFeeRequestDto request);
		Task<(int Result, string Message)> AdminRemoveExtraFeeAsync(RemoveExtraFeeRequestDto request);
		Task<IEnumerable<DropDown>> AdminPanelListOfSourceTypesAsync();
		Task<IEnumerable<DropDown>> AdminPanelOwnerListAsync();
		Task<AdminPropertyViewDetailsDto?> GetPropertyViewDetailsAsync(Guid propertyPublicId, CancellationToken ct);
		Task<(int Result, string Message, InvoiceResponseDto)> AdminGetOrCreateInvoiceAsync(Guid bookingPublicId, string IssueBy);
		Task<(int Result, string Message, AdminBookingPriceSummery)> AdminBookingPriceSummeryAsync(Guid propertyPublicId, DateOnly CheckInDate, DateOnly CheckOutDate);
		Task<(int Result, string Message)> AdminAddManualPaymentAsync(AddManualPaymentRequestDto request);
		#endregion

		#region :: Admin Top Loaction
		Task<(int Result, string Message)> AdminAddLocationAsync(Guid LocationPublicId, ImageUploadResultDTO imageUploadResult, AddTopLocationRequestDto request);
		Task<(int Result, string Message)> AdminUpdateLocationAsync(ImageUploadResultDTO imageUploadResult, UpdateTopLocationRequestDto request);
		Task<(int Result, string Message)> AdminDeleteLocationAsync(Guid LocationPublicId, string UpdatedBy);
		Task<(int Result, string Message)> AdminToggleActiveAsync(Guid LocationPublicId, string UpdatedBy);
		Task<(int Result, string Message, IEnumerable<AdminTopLocationDto>)> AdminGetAllLocationsAsync(int PageNumber, int PageSize);

		#endregion

		#region :: Admin Offer Management
		Task<(int Result, string Message)> AdminAddOfferAsync(AdminAddOfferRequestDto request, ImageUploadResultDTO uploadResult, Guid offerPublicId);
		Task<(int Result, string Message)> AdminUpdateOfferAsync(AdminUpdateOfferRequestDto request, ImageUploadResultDTO uploadResult);
		Task<(int Result, string Message)> AdminDeleteOfferAsync(Guid offerPublicId, string deletedBy);
		Task<(int Result, string Message)> AdminToggleOfferActiveAsync(Guid offerPublicId, string updatedBy);
		Task<(int Result, string Message, IEnumerable<OfferListItemDto>)> AdminGetAllOffersAsync(int pageNumber, int pageSize);
		Task<(int Result, string Message, OfferDetailDto? Data)> AdminGetOfferByIdAsync(Guid offerPublicId);
		#endregion

		#region :: Admin Fees Management

		Task<IEnumerable<FeesTypeDTO>> AdminGetAllFeeTypesAsync();
		Task<(int Result, string Message)> AdminAddFeeAsync(AdminAddFeeRequestDto request);
		Task<(int Result, string Message)> AdminUpdateFeeAsync(AdminUpdateFeeTypeDTO request);
		Task<(int Result, string Message)> AdminToggleFeeTypeAsync(Guid feePublicId, bool isActive, string updatedBy);

		#endregion

		#region :: Admin Booking Calendar Management

		Task<(int Result, string Message, IEnumerable<CalendarBookingDto>)> GetBookingsOnCalenderAsync(CalendarBookingRequestDTO request);

		#endregion

		#region:: Admin SEO Management

		Task<(int Result, string Message, IEnumerable<SeoContentDto>)> GetSeoContentListAsync(int pageNumber, int pageSize);
		Task<(int Result, string Message)> AddSeoContentAsync(SeoContentAddRequestDto request);
		Task<(int Result, string Message)> UpdateSeoContentAsync(SeoContentUpdateRequestDto request);
		Task<(int Result, string Message)> DeleteSeoContentAsync(int seoContentId, string userId);
		Task<(int Result, string Message, IEnumerable<SeoPageMasterDto>)> GetSeoPageMasterListAsync(int pageNumber, int pageSize);
		Task<(int Result, string Message)> AddSeoPageMasterAsync(SeoPageMasterAddRequestDto request);
		Task<(int Result, string Message)> UpdateSeoPageMasterAsync(SeoPageMasterUpdateRequestDto request);
		Task<(int Result, string Message)> DeleteSeoPageMasterAsync(int pageKeyId, string userId);

		#endregion

		#region :: Admin Dashboard Management
		Task<AdminDashboardDto> AdminGetDashboardSummaryAsync();
		Task<List<RevenueTrendItemDto>> AdminGetRevenueTrendAsync();
		#endregion

		#endregion

		#region :: User

		Task<int> RegisterAsync(RegisterRequestDTO model, ImageUploadResultDTO uploadResult, string UserId, string UserDetailsId);
		Task<int> ForgotPasswordRequestAsync(string email, string otp, DateTime OtpExpireTime);
		Task<int> VerifyForgotPasswordOtpAsync(string email, string otp);
		Task<int> StoreResetTokenAsync(string email, string resetToken, DateTime ResetTokenExpireTime);
		Task<int> ResetPasswordAsync(string resetToken, string newPassword);
		Task<int> CheckResendOTPRateLimitAsync(string email);
		Task<int> ResendOTPAsync(string email, string otp, DateTime OtpExpireTime);
		Task<int> UpdateUserProfileImageAsync(string userId, ImageUploadResultDTO uploadResult);
		Task<UserDetailsResponceDTO> GetUserDetailsByUserPublicIdAsync(Guid userId);
		Task<LoginResponseUserDetailsDTO> SocialLoginAsync(string email, string socialId, string firstName, string lastName, string provider, string? deviceToken, string? devicePlatform);
		Task<(int Result, string Message)> ChangePasswordAsync(ChangePasswordRequestDto request);
		Task<(int Result, string Message)> DeactivateDeviceTokenAsync(LogoutRequestDTO request);

		#endregion

		#region :: Property

		Task<(int Result, string Message, Guid propertyId)> AddPropertyFirstStepAsync(UserAddFirstStepPropDTO request);
		Task<(int Result, string Message)> AddPropertySecondStepAsync(UserAddSecondStepPropDTO request);
		Task<(int Result, string Message)> AddPropertyThirdStepAsync(UserAddThirdStepPropDTO request);
		Task<(int Result, string Message)> AddPropertyFourthStepAsync(Guid propPublicId, string jsonImages);
		Task<(int Result, string Message)> FinalSubmitPropertyAsync(Guid propPublicId, string createdBy);
		Task<IEnumerable<AmenitiesResponceDTO>> GetAllAmenitiesAsync();
		Task<IEnumerable<AdditionalServicesResponceDTO>> GetAllAdditionalServicesAsync();
		Task<IEnumerable<HouseRulesResponceDTO>> GetAllHouseRulesAsync();
		Task<UserFirstStepPropResponceDTO> GetPropertyDetailsFirstStepAsync(string createdBy);
		Task<UserSecondStepPropResponceDTO?> GetPropertyDetailsSecondStepAsync(Guid propertyPublicId, string createdBy);
		Task<UserThirdStepPropResponceDTO?> GetPropertyDetailsThirdStepAsync(Guid propertyPublicId, string createdBy);
		Task<UserFourthStepPropResponceDTO?> GetPropertyDetailsFourthStepAsync(Guid propertyPublicId, string createdBy);
		Task<IEnumerable<PropSearchResponseDTO>> SearchPropertiesAsync(PropSearchRequestDTO request, CancellationToken ct = default);
		Task<IEnumerable<Properties>> GetAllPropForDeveloper();
		Task<PropertyDetailsDto> GetPropertyDetailsByPublicIdAsync(Guid propertyPublicId, string userId, CancellationToken cancellationToken = default);
		Task<PropRatingDetailsDTO> GetPropertyDetailsRatingsByPropPublicIdAsync(Guid propertyPublicId, CancellationToken cancellationToken = default);
		Task<(int Result, string Message)> AddPropertyAvailabilityAsync(Guid propPublicId, string createdBy);
		Task<PropertyUnavailableDatesDTO> GetPropertyUnavailableDatesAsync(Guid propPublicId);
		Task<(int Result, string Message, IEnumerable<OwnerPropertyResponceDTO>)> GetAllOwnerPropertyListAsync(string OwnerId);
		Task<(int Result, string Message)> UpdateFullPropertyAsync(AdminUpdateFullPropertyDTO req);
		Task<IEnumerable<PropSearchResponseDTO>> IsPopularPropertyBasedOnCurrentLocationAsync(IsPopularPropReqDTO req, CancellationToken cancellationToken = default);
		Task<IEnumerable<PropSearchResponseDTO>> IsFeaturedPropertyBasedOnCurrentLocationAsync(IsFeaturedPropReqDTO req, CancellationToken cancellationToken = default);
		Task<(int Result, string Message, IEnumerable<TopLocationDto>)> GetTopLocationsByCityAsync(string City, CancellationToken cancellationToken = default);
		Task<(int Result, string Message, IEnumerable<CouponOfferResponceDto>)> GetAvailableCouponsForOfferPageAsync(string userId);

		#endregion

		#region :: Booking
		Task<PropertySelectInfoForBookingDTO> GetPropertySelectDetailsForBookingAsync(Guid propPublicId, string UserId);
		Task<(int Result, string Message, BookingHoldResultDTO)> CreateBookingHoldAsync(BookingHoldRequestDTO request);

		//Task<(int Result, string Message, ConfirmPaymentResponseDto)> ConfirmPaymentAsync(ConfirmPaymentRequest request);
		Task<(int Result, string Message, IEnumerable<UserBookingListItemDto>)> GetUserBookingListItemAsync(UserBookedDetailRequestDTO request);
		Task<(int Result, string Message, BookingDetailsResponceDto)> GetBookingDetailsByBookingIdAndUserIdAsync(Guid BookingPublicId, string UserId);
		Task<(int Result, string Message, string? OwnerUserId, string? PropertyName)> UserCancelBookingAsync(UserCancelBookingRequestDto req);
		//Task<(int Result, string Message, UserExtendBookingResponseDto responseDto)> UserExtendBookingAsync(UserExtendBookingRequestDto req, DateOnly? newCheckOut);
		Task<(int Result, string Message, string? OwnerUserId)> UserRateAndReviewAsync(PropRatingRequestDTO req);
		Task<(int Result, string Message, IEnumerable<CheckoutCouponDto>)> GetAvailableCouponsForCheckoutAsync(Guid holdPublicId, string userId, CancellationToken cancellationToken = default);
		Task<PaymentResultDto> PaymentResultAsync(Guid paymentPublicId, string userId, CancellationToken cancellationToken = default);
		Task<(int Result, string Message, ExtensionHoldResultDTO)> CreateExtensionHoldAsync(CreateExtensionHoldRequest request);

		#endregion

		#region :: Payment
		Task<(int Result, string Message, InitiatePaymentResponseDto)> InitiatePaymentAsync(InitiatePaymentRequest request, string paymentPlanType, decimal? depositPercentage);
		Task<(int Result, string Message)> UpdateStripePaymentIntentAsync(UpdateStripePaymentIntentRequestDTO request);
		Task<(int Result, string Message, StripeWebhookResponseDto)> ProcessStripeWebhookAsync(ProcessStripeWebhookRequestDTO request);
		Task<(int Result, string Message, BalancePaymentResponseDto)> InitiateBalancePaymentAsync(InitiateBalancePaymentRequest request);
		Task<(int Result, string Message)> CancelExpiredPartialBookingsAsync(CancellationToken ct = default);
		Task<IEnumerable<BalancePaymentReminderDto>> GetPendingBalancePaymentRemindersAsync(CancellationToken ct = default);
		Task<(int Result, string Message, ExtensionPaymentResponseDto)> InitiateExtensionPaymentAsync(InitiateExtensionPaymentRequest request);
		Task<(int Result, string Message, InvoiceResponseDto)> GetOrCreateInvoiceAsync(Guid bookingPublicId, string userId);
		#endregion

		#region :: Wishlist
		Task<IEnumerable<WishListResponceDTO>> GetUserWishListPropertiesAsync(WishListRequestDTO request, CancellationToken cancellationToken = default);
		Task<(int Result, string Message)> AddRemoveWishListPropertyAsync(AddRemoveWishListPropertyDTO request);
		#endregion

		#region :: Notifications

		Task<IEnumerable<ActiveDeviceTokenDTO>> GetActiveDeviceTokensAsync();
		Task<int> DeactivateInvalidTokensAsync(List<string> tokens);
		Task<(int Result, string Message)> InsertBulkNotificationAsync(BroadcastNotificationRequestDTO request, int sentCount, int failedCount);

		Task<List<string>> GetUserActiveDeviceTokenAsync(string userId);
		Task<(int Result, string Message)> InsertSingleNotificationAsync(string userId, string title, string message, string type, string? referenceId);

		Task<long> GetUnreadNotificationCountAsync(string userId);
		Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync(string userId);
		Task<(int Result, string Message)> MarkNotificationReadAsync(long notificationId, string userId);
		Task<(int Result, string Message)> MarkAllNotificationsReadAsync(string userId);
		Task<(int Result, string Message)> DeleteNotificationAsync(long notificationId, string userId);

		#endregion

		#region :: BackGround Jobs

		Task<IEnumerable<CheckInReminderDto>> GetTomorrowCheckInsAsync(CancellationToken ct = default);
		Task<IEnumerable<CheckInReminderDto>> GetTodayCheckInsAsync(CancellationToken ct = default);
		Task<IEnumerable<CheckInReminderDto>> GetTodayCheckOutsAsync(CancellationToken ct = default);
		Task<(int Result, string Message)> ReleaseExpiredHoldsAsync(CancellationToken ct = default);
		Task<IEnumerable<GetUserListForAdminPanelDto>> GetUserListForAdminPanelAsync();
		Task<(int Result, string Message)> DeletePropertyAsync(Guid PropertyPublicId, string ModifiedBy);
		Task<(int Result, string Message)> ReleaseExpiredExtensionHoldsAsync(CancellationToken ct = default);
		#endregion

	}
}
