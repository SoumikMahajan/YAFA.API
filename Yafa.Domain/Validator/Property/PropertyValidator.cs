using FluentValidation;
using Yafa.Domain.Dtos.Admin;
using Yafa.Domain.Dtos.Property;

namespace Yafa.Domain.Validator.Property
{
	public class PropertyValidator
	{
		public static bool BeTrimmed(string? value)
		{
			return !string.IsNullOrWhiteSpace(value) && value == value.Trim();
		}

		public class UserAddFirstStepPropValidator : AbstractValidator<UserAddFirstStepPropDTO>
		{
			public UserAddFirstStepPropValidator()
			{
				RuleFor(x => x.PropertyName)
					.NotEmpty().WithMessage("PropertyName is required")
					.NotNull().WithMessage("PropertyName is required")
					.MaximumLength(200).WithMessage("PropertyName must not exceed 200 characters")
					.Must(BeTrimmed).WithMessage("PropertyName must not contain leading or trailing spaces");

				RuleFor(x => x.PropertyType)
					.NotEmpty().WithMessage("PropertyType is required")
					.NotNull().WithMessage("PropertyType is required")
					.MaximumLength(100).WithMessage("PropertyType must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("PropertyType must not contain leading or trailing spaces");

				RuleFor(x => x.ShortDescription)
					.NotEmpty().WithMessage("ShortDescription is required")
					.NotNull().WithMessage("ShortDescription is required")
					.MaximumLength(500).WithMessage("ShortDescription must not exceed 500 characters")
					.Must(BeTrimmed).WithMessage("ShortDescription must not contain leading or trailing spaces");

				RuleFor(x => x.Description)
					.NotEmpty().WithMessage("Description is required")
					.NotNull().WithMessage("Description is required")
					.Must(BeTrimmed).WithMessage("Description must not contain leading or trailing spaces");

				RuleFor(x => x.Address)
					.NotEmpty().WithMessage("Address is required")
					.NotNull().WithMessage("Address is required")
					.MaximumLength(300).WithMessage("Address must not exceed 300 characters")
					.Must(BeTrimmed).WithMessage("Address must not contain leading or trailing spaces");

				RuleFor(x => x.Latitude)
					.InclusiveBetween(-90, 90)
					.WithMessage("Latitude must be between -90 and 90");

				RuleFor(x => x.Longitude)
					.InclusiveBetween(-180, 180)
					.WithMessage("Longitude must be between -180 and 180");

				RuleFor(x => x)
					.Must(x => !(x.Latitude == 0 && x.Longitude == 0))
					.WithMessage("Latitude and Longitude cannot both be 0");

				RuleFor(x => x.TotalBedrooms)
					.GreaterThan(0)
					.WithMessage("TotalBedrooms must be greater than 0");

				RuleFor(x => x.TotalBathrooms)
					.GreaterThan(0)
					.WithMessage("TotalBathrooms must be greater than 0");

				RuleFor(x => x.MaxPersons)
					.GreaterThan(0)
					.WithMessage("MaxPersons must be greater than 0");

				RuleFor(x => x.ContactFirstName)
					.NotEmpty().WithMessage("ContactFirstName is required")
					.NotNull().WithMessage("ContactFirstName is required")
					.MaximumLength(200).WithMessage("ContactFirstName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("ContactFirstName must not contain leading or trailing spaces");

				RuleFor(x => x.ContactLastName)
					.NotEmpty().WithMessage("ContactLastName is required")
					.NotNull().WithMessage("ContactLastName is required")
					.MaximumLength(200).WithMessage("ContactLastName must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("ContactLastName must not contain leading or trailing spaces");

				RuleFor(x=> x.ContactEmail)
					.NotEmpty().WithMessage("ContactEmail is required")
					.NotNull().WithMessage("ContactEmail is required")
					.EmailAddress().WithMessage("ContactEmail must be a valid email address")
					.MaximumLength(200).WithMessage("ContactEmail must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("ContactEmail must not contain leading or trailing spaces");

				RuleFor(x => x.ContactPhone)
					.NotEmpty().WithMessage("ContactPhone is required")
					.NotNull().WithMessage("ContactPhone is required")
					.MaximumLength(20).WithMessage("ContactPhone must not exceed 20 characters")
					.Must(BeTrimmed).WithMessage("ContactPhone must not contain leading or trailing spaces");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .NotNull().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class UserAddSecondStepPropValidator : AbstractValidator<UserAddSecondStepPropDTO>
		{
			public UserAddSecondStepPropValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEmpty().WithMessage("PropertyPublicId is required")
					.NotNull().WithMessage("PropertyPublicId is required");

				RuleFor(x => x.CheckInTime)
					.NotEmpty().WithMessage("CheckInTime is required")
					.NotNull().WithMessage("CheckInTime is required");

				RuleFor(x => x.CheckOutTime)
					.NotEmpty().WithMessage("CheckOutTime is required")
					.NotNull().WithMessage("CheckOutTime is required");
				//.GreaterThan(x => x.CheckInTime).WithMessage("CheckOutTime must be later than CheckInTime");                

				RuleFor(x => x.AmenityIds)
					.NotEmpty().WithMessage("AmenityIds is required");

				RuleFor(x => x.ServiceIDs)
					.NotEmpty().WithMessage("ServiceIDs is required");

				RuleFor(x => x.ServicePrices)
					.NotEmpty().WithMessage("ServicePrices is required");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .NotNull().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class UserAddThirdStepPropValidator : AbstractValidator<UserAddThirdStepPropDTO>
		{
			public UserAddThirdStepPropValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEmpty().WithMessage("PropertyPublicId is required")
					.NotNull().WithMessage("PropertyPublicId is required"); ;

				RuleFor(x => x.RefundPolicyDescription)
					.NotEmpty().WithMessage("RefundPolicyDescription is required")
					.NotNull().WithMessage("RefundPolicyDescription is required")
					.Must(BeTrimmed).WithMessage("RefundPolicyDescription must not contain leading or trailing spaces");
				RuleFor(x => x.RefundPercentage)
					.InclusiveBetween(0, 100)
					.WithMessage("RefundPercentage must be between 0 and 100");
				RuleFor(x => x.RefundDaysBeforeCheckIn)
					.GreaterThanOrEqualTo(0)
					.WithMessage("RefundDaysBeforeCheckIn must be greater than or equal to 0");
				RuleFor(x => x.MemberPrice)
					.GreaterThan(0)
					.WithMessage("MemberPrice must be greater than 0");
				RuleFor(x => x.NonMemberPrice)
					.GreaterThan(0)
					.WithMessage("NonMemberPrice must be greater than 0");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .NotNull().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class UserAddFourthStepPropValidator : AbstractValidator<UserAddFourthStepPropDTO>
		{
			public UserAddFourthStepPropValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.PropertyImages)
					.NotNull().WithMessage("PropertyImages is required")
					.NotEmpty().WithMessage("At least one property image is required");

				RuleForEach(x => x.PropertyImages).ChildRules(images =>
				{
					images.RuleFor(i => i.ProfileImageBase64)
						.NotEmpty().WithMessage("ProfileImageBase64 is required");

					images.RuleFor(i => i.ProfileImageExtension)
						.NotEmpty().WithMessage("ProfileImageExtension is required");
				});

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .NotNull().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(PropertyValidator.BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class PropSearchRequestValidator : AbstractValidator<PropSearchRequestDTO>
		{
			public PropSearchRequestValidator()
			{
				RuleFor(x => x.Latitude)
					.InclusiveBetween(-90, 90)
					.When(x => x.Latitude.HasValue)
					.WithMessage("Latitude must be between -90 and 90");
				RuleFor(x => x.Longitude)
					.InclusiveBetween(-180, 180)
					.When(x => x.Longitude.HasValue)
					.WithMessage("Longitude must be between -180 and 180");
				RuleFor(x => x.Guests)
					.GreaterThan(0)
					.WithMessage("Guests must be greater than 0");
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.PageNumber)
					.GreaterThan(0)
					.WithMessage("PageNumber must be greater than 0");
				RuleFor(x => x.PageSize)
					.GreaterThan(0)
					.WithMessage("PageSize must be greater than 0");
			}
		}

		public class AddRemoveWishListPropertyValidator : AbstractValidator<AddRemoveWishListPropertyDTO>
		{
			public AddRemoveWishListPropertyValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.IsAdding)
					.NotNull().WithMessage("IsAdding is required");
			}
		}

		public class UserWishListRequestValidator : AbstractValidator<WishListRequestDTO>
		{
			public UserWishListRequestValidator()
			{
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.PageNumber)
					.GreaterThan(0)
					.WithMessage("PageNumber must be greater than 0");
				RuleFor(x => x.PageSize)
					.GreaterThan(0)
					.WithMessage("PageSize must be greater than 0");
			}
		}

		public class PropHoldBookedRequestValidator : AbstractValidator<BookingHoldRequestDTO>
		{
			public PropHoldBookedRequestValidator()
			{
				RuleFor(x => x.PropertyPublicId)
				   .NotEqual(Guid.Empty).WithMessage("PropertyPublicId is required");
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.numberOfGuests)
					.GreaterThan(0)
					.WithMessage("Number of guests must be greater than 0.");

			}
		}

		public class UserBookedDetailRequestRequesrValidator : AbstractValidator<UserBookedDetailRequestDTO>
		{
			public UserBookedDetailRequestRequesrValidator()
			{
				RuleFor(x => x.UserId)
				   .NotEmpty().WithMessage("UserId is required")
				   .NotNull().WithMessage("UserId is required")
				   .MinimumLength(6).WithMessage("UserId must be at least 6 characters")
				   .MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");

				RuleFor(x => x.PageNumber)
				   .NotEmpty().WithMessage("PageNumber is required")
				   .NotNull().WithMessage("PageNumber is required");

				RuleFor(x => x.PageSize)
				   .NotEmpty().WithMessage("PageSize is required")
				   .NotNull().WithMessage("PageSize is required");
			}
		}

		public class UpdateFullPropertyValidator : AbstractValidator<UpdateFullPropertyDTO>
		{
			public UpdateFullPropertyValidator()
			{
				//RuleFor(x => x.PropertyName)
				//	.NotEmpty().WithMessage("PropertyName is required")
				//	.NotNull().WithMessage("PropertyName is required")
				//	.MaximumLength(200).WithMessage("PropertyName must not exceed 200 characters")
				//	.Must(BeTrimmed).WithMessage("PropertyName must not contain leading or trailing spaces");

				//RuleFor(x => x.PropertyType)
				//	.NotEmpty().WithMessage("PropertyType is required")
				//	.NotNull().WithMessage("PropertyType is required")
				//	.MaximumLength(100).WithMessage("PropertyType must not exceed 100 characters")
				//	.Must(BeTrimmed).WithMessage("PropertyType must not contain leading or trailing spaces");

				//RuleFor(x => x.ShortDescription)
				//	.NotEmpty().WithMessage("ShortDescription is required")
				//	.NotNull().WithMessage("ShortDescription is required")
				//	.MaximumLength(500).WithMessage("ShortDescription must not exceed 500 characters")
				//	.Must(BeTrimmed).WithMessage("ShortDescription must not contain leading or trailing spaces");

				//RuleFor(x => x.Description)
				//	.NotEmpty().WithMessage("Description is required")
				//	.NotNull().WithMessage("Description is required")
				//	.Must(BeTrimmed).WithMessage("Description must not contain leading or trailing spaces");

				//RuleFor(x => x.Address)
				//	.NotEmpty().WithMessage("Address is required")
				//	.NotNull().WithMessage("Address is required")
				//	.MaximumLength(300).WithMessage("Address must not exceed 300 characters")
				//	.Must(BeTrimmed).WithMessage("Address must not contain leading or trailing spaces");

				//RuleFor(x => x.Latitude)
				//	.InclusiveBetween(-90, 90)
				//	.WithMessage("Latitude must be between -90 and 90");

				//RuleFor(x => x.Longitude)
				//	.InclusiveBetween(-180, 180)
				//	.WithMessage("Longitude must be between -180 and 180");

				//RuleFor(x => x)
				//	.Must(x => !(x.Latitude == 0 && x.Longitude == 0))
				//	.WithMessage("Latitude and Longitude cannot both be 0");

				//RuleFor(x => x.TotalBedrooms)
				//	.GreaterThan(0)
				//	.WithMessage("TotalBedrooms must be greater than 0");

				//RuleFor(x => x.TotalBathrooms)
				//	.GreaterThan(0)
				//	.WithMessage("TotalBathrooms must be greater than 0");

				//RuleFor(x => x.MaxPersons)
				//	.GreaterThan(0)
				//	.WithMessage("MaxPersons must be greater than 0");

				//RuleFor(x => x.CheckInTime)
				//	.NotEmpty().WithMessage("CheckInTime is required")
				//	.NotNull().WithMessage("CheckInTime is required");

				//RuleFor(x => x.CheckOutTime)
				//	.NotEmpty().WithMessage("CheckOutTime is required")
				//	.NotNull().WithMessage("CheckOutTime is required");              

				//RuleFor(x => x.AmenityIds)
				//	.NotEmpty().WithMessage("AmenityIds is required");

				//RuleFor(x => x.ServiceIDs)
				//	.NotEmpty().WithMessage("ServiceIDs is required");

				//RuleFor(x => x.ServicePrices)
				//	.NotEmpty().WithMessage("ServicePrices is required");

				//RuleFor(x => x.RefundPolicyDescription)
				//	.NotEmpty().WithMessage("RefundPolicyDescription is required")
				//	.NotNull().WithMessage("RefundPolicyDescription is required")
				//	.Must(BeTrimmed).WithMessage("RefundPolicyDescription must not contain leading or trailing spaces");
				//RuleFor(x => x.RefundPercentage)
				//	.InclusiveBetween(0, 100)
				//	.WithMessage("RefundPercentage must be between 0 and 100");
				//RuleFor(x => x.RefundDaysBeforeCheckIn)
				//	.GreaterThanOrEqualTo(0)
				//	.WithMessage("RefundDaysBeforeCheckIn must be greater than or equal to 0");
				//RuleFor(x => x.MemberPrice)
				//	.GreaterThan(0)
				//	.WithMessage("MemberPrice must be greater than 0");
				//RuleFor(x => x.NonMemberPrice)
				//	.GreaterThan(0)
				//	.WithMessage("NonMemberPrice must be greater than 0");

				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");

				RuleFor(x => x.CreatedBy)
				   .NotEmpty().WithMessage("CreatedBy is required")
				   .NotNull().WithMessage("CreatedBy is required")
				   .MinimumLength(6).WithMessage("CreatedBy must be at least 6 characters")
				   .MaximumLength(100).WithMessage("CreatedBy must not exceed 100 characters")
				   .Must(BeTrimmed).WithMessage("CreatedBy must not contain leading or trailing spaces");
			}
		}

		public class UserPropertyRatingRequestValidator : AbstractValidator<PropRatingRequestDTO>
		{
			public UserPropertyRatingRequestValidator()
			{
				RuleFor(x => x.PropertyPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("PropertyPublicId is required");
				RuleFor(x => x.Rating)
					.NotNull().WithMessage("Rating is required")
					.InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.MinimumLength(6).WithMessage("UserId must be at least 6 characters")
					.MaximumLength(100).WithMessage("UserId must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");
				RuleFor(x => x.BookingPublicId)
					.NotEqual(Guid.Empty)
					.WithMessage("BookingPublicId is required");
				RuleFor(x => x.Comment)
					.MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters")
					.Must(BeTrimmed).WithMessage("Comment must not contain leading or trailing spaces");
				
				RuleFor(x => x.StayDuration)
					.GreaterThan(0)
					.WithMessage("StayDuration must be greater than 0");
			}
		}
	}
}
