using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.User;

namespace Yafa.Domain.Validator.Admin
{
	public static class AdminValidator
	{
		public static bool BeTrimmed(string? value)
		{
			if (string.IsNullOrWhiteSpace(value)) return false;
			return value == value.Trim();
		}

		public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
		{
			public LoginRequestValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");

				RuleFor(x => x.Password)
					.NotEmpty().WithMessage("Password is required")
					.MinimumLength(6).WithMessage("Password must be at least 6 characters")
					.MaximumLength(100).WithMessage("Password must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Password must not contain leading or trailing spaces");

				RuleFor(x => x.DeviceToken)
				  .NotEmpty().WithMessage("DeviceToken is required")
				  .Must(BeTrimmed).WithMessage("DeviceToken must not contain leading or trailing spaces");

				RuleFor(x => x.DeviceTokenPlatform)
				 .NotEmpty().WithMessage("DeviceToken is required")
				 .Must(BeTrimmed).WithMessage("DeviceToken must not contain leading or trailing spaces");
			}
		}

		public class RegisterRequestValidator : AbstractValidator<AdminRegisterRequestDTO>
		{
			public RegisterRequestValidator()
			{
				RuleFor(x => x.RoleId)
					.NotNull().WithMessage("RoleId is required.")
					.NotEmpty().WithMessage("RoleId is required")
					.Must(BeTrimmed).WithMessage("RoleId must not contain leading or trailing spaces");

				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");


				RuleFor(x => x.Password)
					.NotEmpty().WithMessage("Password is required")
					.MinimumLength(6).WithMessage("Password must be at least 6 characters")
					.MaximumLength(100).WithMessage("Password must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Password must not contain leading or trailing spaces");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");

				RuleFor(x => x.FirstName)
				   .NotEmpty().WithMessage("FirstName is required")
				   .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("FirstName must not contain leading or trailing spaces");

				RuleFor(x => x.LastName)
				   .NotEmpty().WithMessage("LastName is required")
				   .MaximumLength(100).WithMessage("LastName must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("LastName must not contain leading or trailing spaces");
			}
		}

		public class BlockPropertyRequesrValidator : AbstractValidator<BlockPropRequestDTO>
		{
			public BlockPropertyRequesrValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.BlockDates)
				  .NotEmpty().WithMessage("BlockDates is required")
				  .NotNull().WithMessage("BlockDates is required")
				  .Must(BeTrimmed).WithMessage("BlockDates must not contain leading or trailing spaces");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");

			}
		}

		public class AllBookedDetailRequesrValidator : AbstractValidator<AllBookedDetailRequestDTO>
		{
			public AllBookedDetailRequesrValidator()
			{

				RuleFor(x => x.PageNumber)
				   .NotEmpty().WithMessage("PageNumber is required")
				   .NotNull().WithMessage("PageNumber is required");

				RuleFor(x => x.PageSize)
				   .NotEmpty().WithMessage("PageSize is required")
				   .NotNull().WithMessage("PageSize is required");
			}
		}

		public class CancelBookingRequestValidator : AbstractValidator<CancelBookingRequestDTO>
		{
			public CancelBookingRequestValidator()
			{
				RuleFor(x => x.BookingPublicId)
				   .NotEqual(Guid.Empty)
				   .WithMessage("BookingPublicId is required");
				RuleFor(x => x.CancellationReason)
					 .NotEmpty().WithMessage("CancellationReason is required")
					 .Must(BeTrimmed).WithMessage("CancellationReason must not contain leading or trailing spaces");
				RuleFor(x => x.CreatedBy)
					 .NotEmpty().WithMessage("CreatedBy is required")
					 .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class RefundBookingRequestValidator : AbstractValidator<RefundBookingRequestDTO>
		{
			public RefundBookingRequestValidator()
			{
				RuleFor(x => x.BookingPublicId)
				   .NotEqual(Guid.Empty)
				   .WithMessage("BookingPublicId is required");
				RuleFor(x => x.RefundAmount)
					 .GreaterThan(0).WithMessage("RefundAmount must be greater than 0");
				RuleFor(x => x.CreatedBy)
					 .NotEmpty().WithMessage("CreatedBy is required")
					 .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class ExtendBookingRequestValidator : AbstractValidator<ExtendBookingRequestDTO>
		{
			public ExtendBookingRequestValidator()
			{
				RuleFor(x => x.BookingPublicId)
				   .NotEqual(Guid.Empty)
				   .WithMessage("BookingPublicId is required");
				RuleFor(x => x.CheckOutDate)
					 .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("CheckOutDate must be a future date");
				RuleFor(x => x.CreatedBy)
					 .NotEmpty().WithMessage("CreatedBy is required")
					 .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminAddLocationRequestValidator : AbstractValidator<AddTopLocationRequestDto>
		{
			public AdminAddLocationRequestValidator()
			{
				RuleFor(x => x.City)
					.NotEmpty().WithMessage("City is required")
					.MaximumLength(100).WithMessage("City must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("City must not contain leading or trailing spaces");
				RuleFor(x => x.LocationName)
					.NotEmpty().WithMessage("LocationName is required")
					.MaximumLength(100).WithMessage("LocationName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("LocationName must not contain leading or trailing spaces");
				RuleFor(x => x.Description)
					.MaximumLength(500).WithMessage("Description must not exceed 500 characters")
					.Must(BeTrimmed).WithMessage("Description must not contain leading or trailing spaces");
				RuleFor(x => x.ProfileImageBase64)
					.NotEmpty().WithMessage("ProfileImage is required")
					.Must(BeTrimmed).WithMessage("ProfileImage must not contain leading or trailing spaces");
				RuleFor(x => x.CreateBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminUpdateLocationRequestValidator : AbstractValidator<UpdateTopLocationRequestDto>
		{
			public AdminUpdateLocationRequestValidator()
			{
				RuleFor(x => x.LocationPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("LocationPublicId is required");
				RuleFor(x => x.UpdatedBy)
				   .NotEmpty().WithMessage("UpdatedBy is required")
				   .MinimumLength(6).WithMessage("UpdatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("UpdatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("UpdatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminAddOfferRequestValidator : AbstractValidator<AdminAddOfferRequestDto>
		{
			public AdminAddOfferRequestValidator()
			{
				RuleFor(x => x.Title)
					.NotEmpty().WithMessage("Offer title is required.")
					.MaximumLength(500).WithMessage("Title must not exceed 500 characters.")
					.Must(BeTrimmed).WithMessage("Title must not have leading/trailing spaces.");

				RuleFor(x => x.PriceLabel)
					.MaximumLength(100).WithMessage("PriceLabel must not exceed 100 characters.")
					.When(x => x.PriceLabel != null);

				RuleFor(x => x.Description)
					.MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.")
					.When(x => x.Description != null);

				RuleFor(x => x.BadgeText)
					.MaximumLength(100).WithMessage("Badge text must not exceed 100 characters.")
					.When(x => x.BadgeText != null);

				RuleFor(x => x.BadgeColor)
					.MaximumLength(50).WithMessage("Badge color must not exceed 50 characters.")
					.When(x => x.BadgeColor != null);

				RuleFor(x => x.DiscountFixedAmount)
					 .GreaterThanOrEqualTo(0).WithMessage("Discount fixed amount must be 0 or greater.");

				RuleFor(x => x.AvailableUntil)
					.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
					.WithMessage("AvailableUntil must be today or a future date.")
					.When(x => x.AvailableUntil.HasValue);

				RuleFor(x => x.OfferImageExtension)
					.Must(ext => new[] { "jpg", "jpeg", "png" }.Contains(ext.ToLower()))
					.WithMessage("Image extension must be jpg, jpeg, or png.")
					.When(x => !string.IsNullOrWhiteSpace(x.OfferImageBase64));

				RuleFor(x => x.SortOrder)
					.GreaterThanOrEqualTo(0).WithMessage("SortOrder must be 0 or greater.");

				RuleFor(x => x.Terms)
					.Must(t => t.Count <= 20).WithMessage("A maximum of 20 terms are allowed.")
					.When(x => x.Terms != null);

				RuleForEach(x => x.Terms)
					.NotEmpty().WithMessage("A term entry must not be empty.")
					.MaximumLength(500).WithMessage("Each term must not exceed 500 characters.")
					.When(x => x.Terms != null && x.Terms.Count > 0);

				RuleFor(x => x.CreatedBy)
					.NotEmpty().WithMessage("CreatedBy is required.")
					.MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
					.MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminUpdateOfferRequestValidator : AbstractValidator<AdminUpdateOfferRequestDto>
		{
			public AdminUpdateOfferRequestValidator()
			{
				RuleFor(x => x.OfferPublicId)
					.NotEmpty().WithMessage("OfferPublicId is required.")
					.Must(g => g != Guid.Empty).WithMessage("OfferPublicId must be a valid GUID.");

				RuleFor(x => x.Title)
					.MaximumLength(500).WithMessage("Title must not exceed 500 characters.")
					.Must(BeTrimmed).WithMessage("Title must not have leading/trailing spaces.")
					.When(x => x.Title != null);

				RuleFor(x => x.PriceLabel)
					.MaximumLength(100).WithMessage("PriceLabel must not exceed 100 characters.")
					.When(x => x.PriceLabel != null);

				RuleFor(x => x.Description)
					.MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.")
					.When(x => x.Description != null);

				RuleFor(x => x.BadgeText)
					.MaximumLength(100).WithMessage("Badge text must not exceed 100 characters.")
					.When(x => x.BadgeText != null);

				RuleFor(x => x.DiscountFixedAmount)
					 .GreaterThanOrEqualTo(0).WithMessage("Discount fixed amount must be 0 or greater.");

				RuleFor(x => x.AvailableUntil)
					.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
					.WithMessage("AvailableUntil must be today or a future date.")
					.When(x => x.AvailableUntil.HasValue);

				RuleFor(x => x.OfferImageExtension)
					.Must(ext => new[] { "jpg", "jpeg", "png" }.Contains(ext.ToLower()))
					.WithMessage("Image extension must be jpg, jpeg, or png.")
					.When(x => !string.IsNullOrWhiteSpace(x.OfferImageBase64));

				RuleFor(x => x.SortOrder)
					.GreaterThanOrEqualTo(0).WithMessage("SortOrder must be 0 or greater.")
					.When(x => x.SortOrder.HasValue);

				RuleFor(x => x.Terms)
					.Must(t => t!.Count <= 20).WithMessage("A maximum of 20 terms are allowed.")
					.When(x => x.Terms != null);

				RuleForEach(x => x.Terms)
					.NotEmpty().WithMessage("A term entry must not be empty.")
					.MaximumLength(500).WithMessage("Each term must not exceed 500 characters.")
					.When(x => x.Terms != null && x.Terms.Count > 0);

				RuleFor(x => x.UpdatedBy)
					.NotEmpty().WithMessage("UpdatedBy is required.");
			}
		}

		public class AdminAddPropertyImagesRequestValidator : AbstractValidator<AdminAddPropertyImagesDTO>
		{
			public AdminAddPropertyImagesRequestValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
				RuleFor(x => x.PropertyImages)
					.NotEmpty().WithMessage("At least one property image is required.")
					.Must(images => images.All(img => !string.IsNullOrWhiteSpace(img.ProfileImageBase64)))
					.WithMessage("Each image must have a valid Base64 string and extension.");
			}
		}

		public class AdminAddFeeRequestValidator : AbstractValidator<AdminAddFeeRequestDto>
		{
			public AdminAddFeeRequestValidator()
			{
				RuleFor(x => x.FeeName)
					.NotEmpty().WithMessage("FeeName is required")
					.MaximumLength(100).WithMessage("FeeName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("FeeName must not contain leading or trailing spaces");
				RuleFor(x => x.FeeDescription)
					.MaximumLength(500).WithMessage("FeeDescription must not exceed 500 characters")
					.Must(BeTrimmed).WithMessage("FeeDescription must not contain leading or trailing spaces");
				RuleFor(x => x.FeeCategory)
					.NotEmpty().WithMessage("FeeCategory is required")
					.Must(fc => new[] { "FLAT", "PERCENTAGE", "PER_NIGHT" }.Contains(fc.ToUpper()))
					.WithMessage("FeeCategory must be one of the following: FLAT, PERCENTAGE, PER_NIGHT")
					.Must(BeTrimmed).WithMessage("FeeCategory must not contain leading or trailing spaces");
				RuleFor(x => x.FeeValue)
					.GreaterThanOrEqualTo(0).WithMessage("FeeValue must be greater than or equal to 0");
				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminUpdateFeeRequestValidator : AbstractValidator<AdminUpdateFeeTypeDTO>
		{
			public AdminUpdateFeeRequestValidator()
			{
				RuleFor(x => x.FeePublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("FeePublicId is required");
				RuleFor(x => x.UpdatedBy)
				   .NotEmpty().WithMessage("UpdatedBy is required")
				   .MinimumLength(6).WithMessage("UpdatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("UpdatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("UpdatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminAddUserRequestValidator : AbstractValidator<AddUserRequestDTO>
		{
			public AdminAddUserRequestValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");
				RuleFor(x => x.Password)
					.NotEmpty().WithMessage("Password is required")
					.MinimumLength(6).WithMessage("Password must be at least 6 characters")
					.MaximumLength(100).WithMessage("Password must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Password must not contain leading or trailing spaces");
				RuleFor(x => x.FirstName)
					.NotEmpty().WithMessage("FirstName is required")
					.MaximumLength(100).WithMessage("FirstName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("FirstName must not contain leading or trailing spaces");
				RuleFor(x => x.LastName)
					.NotEmpty().WithMessage("LastName is required")
					.MaximumLength(100).WithMessage("LastName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("LastName must not contain leading or trailing spaces");
				RuleFor(x => x.Gender)
					.NotEmpty().WithMessage("Gender is required");
				RuleFor(x => x.Address1)
					.NotEmpty().WithMessage("Address1 is required")
					.MaximumLength(200).WithMessage("Address1 must not exceed 200 characters")
					.Must(BeTrimmed).WithMessage("Address1 must not contain leading or trailing spaces");
				RuleFor(x => x.PhoneNo)
					.NotEmpty().WithMessage("PhoneNo is required")
					.MaximumLength(20).WithMessage("PhoneNo must not exceed 20 characters")
					.Must(BeTrimmed).WithMessage("PhoneNo must not contain leading or trailing spaces");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}
		public class AdminEditUserRequestValidator : AbstractValidator<UpdateUserRequestDTO>
		{
			public AdminEditUserRequestValidator()
			{
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required");
				RuleFor(x => x.Password)
					.NotEmpty().WithMessage("Password is required")
					.MinimumLength(6).WithMessage("Password must be at least 6 characters")
					.MaximumLength(100).WithMessage("Password must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Password must not contain leading or trailing spaces");
				RuleFor(x => x.FirstName)
					.NotEmpty().WithMessage("FirstName is required")
					.MaximumLength(100).WithMessage("FirstName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("FirstName must not contain leading or trailing spaces");
				RuleFor(x => x.LastName)
					.NotEmpty().WithMessage("LastName is required")
					.MaximumLength(100).WithMessage("LastName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("LastName must not contain leading or trailing spaces");
				RuleFor(x => x.Gender)
					.NotEmpty().WithMessage("Gender is required");
				RuleFor(x => x.Address1)
					.NotEmpty().WithMessage("Address1 is required")
					.MaximumLength(200).WithMessage("Address1 must not exceed 200 characters")
					.Must(BeTrimmed).WithMessage("Address1 must not contain leading or trailing spaces");
				RuleFor(x => x.PhoneNo)
					.NotEmpty().WithMessage("PhoneNo is required")
					.MaximumLength(20).WithMessage("PhoneNo must not exceed 20 characters")
					.Must(BeTrimmed).WithMessage("PhoneNo must not contain leading or trailing spaces");

				RuleFor(x => x.UpdateBy)
				   .NotEmpty().WithMessage("UpdateBy is required")
				   .MinimumLength(6).WithMessage("UpdateBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("UpdateBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("UpdateBy must not contain leading or trailing spaces");
			}
		}

		//public class AdminAddpropertyValidator : AbstractValidator<AdminInsertFullPropertyDTO>
		//{
		//	public AdminAddpropertyValidator()
		//	{
		//		RuleFor(x => x.PropertyName)
		//			 .NotEmpty().WithMessage("PropertyName is required")
		//			 .NotNull().WithMessage("PropertyName is required")
		//			 .MaximumLength(200).WithMessage("PropertyName must not exceed 200 characters")
		//			 .Must(BeTrimmed).WithMessage("PropertyName must not contain leading or trailing spaces");

		//		RuleFor(x => x.PropertyType)
		//			.NotEmpty().WithMessage("PropertyType is required")
		//			.NotNull().WithMessage("PropertyType is required")
		//			.MaximumLength(100).WithMessage("PropertyType must not exceed 100 characters")
		//			.Must(BeTrimmed).WithMessage("PropertyType must not contain leading or trailing spaces");

		//		RuleFor(x => x.ShortDescription)
		//			.NotEmpty().WithMessage("ShortDescription is required")
		//			.NotNull().WithMessage("ShortDescription is required")
		//			.MaximumLength(500).WithMessage("ShortDescription must not exceed 500 characters")
		//			.Must(BeTrimmed).WithMessage("ShortDescription must not contain leading or trailing spaces");

		//		RuleFor(x => x.Description)
		//			.NotEmpty().WithMessage("Description is required")
		//			.NotNull().WithMessage("Description is required")
		//			.Must(BeTrimmed).WithMessage("Description must not contain leading or trailing spaces");

		//		RuleFor(x => x.Address)
		//			.NotEmpty().WithMessage("Address is required")
		//			.NotNull().WithMessage("Address is required")
		//			.MaximumLength(300).WithMessage("Address must not exceed 300 characters")
		//			.Must(BeTrimmed).WithMessage("Address must not contain leading or trailing spaces");

		//		RuleFor(x => x.Latitude)
		//			.InclusiveBetween(-90, 90)
		//			.WithMessage("Latitude must be between -90 and 90");

		//		RuleFor(x => x.Longitude)
		//			.InclusiveBetween(-180, 180)
		//			.WithMessage("Longitude must be between -180 and 180");

		//		RuleFor(x => x)
		//			.Must(x => !(x.Latitude == 0 && x.Longitude == 0))
		//			.WithMessage("Latitude and Longitude cannot both be 0");

		//		RuleFor(x => x.TotalBedrooms)
		//			.GreaterThan(0)
		//			.WithMessage("TotalBedrooms must be greater than 0");

		//		RuleFor(x => x.TotalBathrooms)
		//			.GreaterThan(0)
		//			.WithMessage("TotalBathrooms must be greater than 0");

		//		RuleFor(x => x.MaxPersons)
		//			.GreaterThan(0)
		//			.WithMessage("MaxPersons must be greater than 0");

		//		RuleFor(x => x.ContactFirstName)
		//			.NotEmpty().WithMessage("ContactFirstName is required")
		//			.NotNull().WithMessage("ContactFirstName is required")
		//			.MaximumLength(200).WithMessage("ContactFirstName must not exceed 100 characters")
		//			.Must(BeTrimmed).WithMessage("ContactFirstName must not contain leading or trailing spaces");

		//		RuleFor(x => x.ContactLastName)
		//			.NotEmpty().WithMessage("ContactLastName is required")
		//			.NotNull().WithMessage("ContactLastName is required")
		//			.MaximumLength(200).WithMessage("ContactLastName must not exceed 100 characters")
		//			.Must(BeTrimmed).WithMessage("ContactLastName must not contain leading or trailing spaces");

		//		RuleFor(x => x.ContactEmail)
		//			.NotEmpty().WithMessage("ContactEmail is required")
		//			.NotNull().WithMessage("ContactEmail is required")
		//			.EmailAddress().WithMessage("ContactEmail must be a valid email address")
		//			.MaximumLength(200).WithMessage("ContactEmail must not exceed 100 characters")
		//			.Must(BeTrimmed).WithMessage("ContactEmail must not contain leading or trailing spaces");

		//		RuleFor(x => x.ContactPhone)
		//			.NotEmpty().WithMessage("ContactPhone is required")
		//			.NotNull().WithMessage("ContactPhone is required")
		//			.MaximumLength(20).WithMessage("ContactPhone must not exceed 20 characters")
		//			.Must(BeTrimmed).WithMessage("ContactPhone must not contain leading or trailing spaces");

		//		RuleFor(x => x.CheckInTime)
		//			.NotEmpty().WithMessage("CheckInTime is required")
		//			.NotNull().WithMessage("CheckInTime is required");

		//		RuleFor(x => x.CheckOutTime)
		//			.NotEmpty().WithMessage("CheckOutTime is required")
		//			.NotNull().WithMessage("CheckOutTime is required");

		//		RuleFor(x => x.AmenityIds)
		//			.NotEmpty().WithMessage("AmenityIds is required");

		//		//RuleFor(x => x.ServiceIDs)
		//		//	.NotEmpty().WithMessage("ServiceIDs is required");

		//		//RuleFor(x => x.ServicePrices)
		//		//	.NotEmpty().WithMessage("ServicePrices is required");

		//		//RuleFor(x => x.RefundPolicyDescription)
		//		//.NotEmpty().WithMessage("RefundPolicyDescription is required")
		//		//.NotNull().WithMessage("RefundPolicyDescription is required")
		//		//.Must(BeTrimmed).WithMessage("RefundPolicyDescription must not contain leading or trailing spaces");
		//		//RuleFor(x => x.RefundPercentage)
		//		//	.InclusiveBetween(0, 100)
		//		//	.WithMessage("RefundPercentage must be between 0 and 100");
		//		//RuleFor(x => x.RefundDaysBeforeCheckIn)
		//		//	.GreaterThanOrEqualTo(0)
		//		//	.WithMessage("RefundDaysBeforeCheckIn must be greater than or equal to 0");
		//		RuleFor(x => x.MemberPrice)
		//			.GreaterThan(0)
		//			.WithMessage("MemberPrice must be greater than 0");
		//		RuleFor(x => x.NonMemberPrice)
		//			.GreaterThan(0)
		//			.WithMessage("NonMemberPrice must be greater than 0");

		//		RuleFor(x => x.PropertyImages)
		//			.NotNull().WithMessage("PropertyImages is required")
		//			.NotEmpty().WithMessage("At least one property image is required");

		//		RuleForEach(x => x.PropertyImages).ChildRules(images =>
		//		{
		//			images.RuleFor(i => i.ProfileImageBase64)
		//				.NotEmpty().WithMessage("ProfileImageBase64 is required");

		//			images.RuleFor(i => i.ProfileImageExtension)
		//				.NotEmpty().WithMessage("ProfileImageExtension is required");
		//		});

		//		RuleFor(x => x.CreatedBy)
		//		   .NotEmpty().WithMessage("CreatedBy is required")
		//		   .NotNull().WithMessage("CreatedBy is required")
		//		   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
		//		   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
		//		   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
		//	}
		//}

		public class AdminAddNewBookingRequestValidator : AbstractValidator<AdminAddNewBookingRequestDTO>
		{
			public AdminAddNewBookingRequestValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.CheckInDate)
					.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
					.WithMessage("CheckInDate must be today or a future date");
				RuleFor(x => x.CheckOutDate)
					.GreaterThan(x => x.CheckInDate)
					.WithMessage("CheckOutDate must be after CheckInDate");
				RuleFor(x => x.NumberOfGuests)
					.GreaterThan(0)
					.WithMessage("NumberOfGuests must be greater than 0");
				RuleFor(x => x.BookingSourceType)
					.NotEqual(Guid.Empty)
					.WithMessage("BookingSourceType is required");
				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminAddExtraBookingFeeValidator : AbstractValidator<AddExtraFeeRequestDto>
		{
			public AdminAddExtraBookingFeeValidator()
			{
				RuleFor(x => x.BookingPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("BookingPublicId is required");
				RuleFor(x => x.FeePublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("FeePublicId is required");
				RuleFor(x => x.CreatedBy)
					.NotEmpty().WithMessage("CreatedBy is required")
					.MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
					.MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminAddSEOValidator : AbstractValidator<SeoContentAddRequestDto>
		{
			public AdminAddSEOValidator()
			{
				RuleFor(x => x.PageKeyID)
					.GreaterThan(0).WithMessage("PageKeyID is required");
				RuleFor(x => x.MetaTitle)
					.NotEmpty().WithMessage("MetaTitle is required")
					.MaximumLength(200).WithMessage("MetaTitle must not exceed 200 characters")
					.Must(BeTrimmed).WithMessage("MetaTitle must not contain leading or trailing spaces");
				RuleFor(x => x.MetaDescription)
					.NotEmpty().WithMessage("MetaDescription is required")
					.MaximumLength(500).WithMessage("MetaDescription must not exceed 500 characters")
					.Must(BeTrimmed).WithMessage("MetaDescription must not contain leading or trailing spaces");
				RuleFor(x => x.MetaKeywords)
					.NotEmpty().WithMessage("MetaKeywords is required")
					.MaximumLength(300).WithMessage("MetaKeywords must not exceed 300 characters")
					.Must(BeTrimmed).WithMessage("MetaKeywords must not contain leading or trailing spaces");
				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminUpdateSEOValidator : AbstractValidator<SeoContentUpdateRequestDto>
		{
			public AdminUpdateSEOValidator()
			{
				RuleFor(x => x.SeoContentID)
					.GreaterThan(0).WithMessage("SeoContentID is required");
				RuleFor(x => x.MetaTitle)
					.NotEmpty().WithMessage("MetaTitle is required")
					.MaximumLength(200).WithMessage("MetaTitle must not exceed 200 characters")
					.Must(BeTrimmed).WithMessage("MetaTitle must not contain leading or trailing spaces");
				RuleFor(x => x.MetaDescription)
					.NotEmpty().WithMessage("MetaDescription is required")
					.MaximumLength(500).WithMessage("MetaDescription must not exceed 500 characters")
					.Must(BeTrimmed).WithMessage("MetaDescription must not contain leading or trailing spaces");
				RuleFor(x => x.MetaKeywords)
					.NotEmpty().WithMessage("MetaKeywords is required")
					.MaximumLength(300).WithMessage("MetaKeywords must not exceed 300 characters")
					.Must(BeTrimmed).WithMessage("MetaKeywords must not contain leading or trailing spaces");
				RuleFor(x => x.ModifiedBy)
				   .NotEmpty().WithMessage("ModifiedBy is required")
				   .MinimumLength(6).WithMessage("ModifiedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("ModifiedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("ModifiedBy must not contain leading or trailing spaces");
			}
		}

		public class AdminPropertySearchRequestValidator : AbstractValidator<AdminPropertySearchRequestDTO>
		{
			public AdminPropertySearchRequestValidator()
			{
				RuleFor(x => x.PageNumber)
				   .NotEmpty().WithMessage("PageNumber is required")
				   .NotNull().WithMessage("PageNumber is required");
				RuleFor(x => x.PageSize)
				   .NotEmpty().WithMessage("PageSize is required")
				   .NotNull().WithMessage("PageSize is required");
				RuleFor(x => x.SearchText)
				   .MaximumLength(200).WithMessage("SearchText must not exceed 200 characters")
				   .Must(BeTrimmed).WithMessage("SearchText must not contain leading or trailing spaces")
				   .When(x => !string.IsNullOrWhiteSpace(x.SearchText));
			}
		}
	}

}
