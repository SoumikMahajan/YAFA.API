using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yafa.Domain.Dtos.User;

namespace Yafa.Domain.Validator.User
{
	public static class UserValidator
	{
		public static bool BeTrimmed(string? value)
		{
			if (string.IsNullOrWhiteSpace(value)) return false;
			return value == value.Trim();
		}

		public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
		{
			public RegisterRequestValidator()
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

		public class ForgetPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
		{
			public ForgetPasswordRequestValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");
			}
		}

		public class validateOTPRequestValidator : AbstractValidator<VerifyOTPDto>
		{
			public validateOTPRequestValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");
				RuleFor(x => x.OTP)
					.NotEmpty().WithMessage("OTP is required")
					.MinimumLength(6).WithMessage("OTP must be min 6 characters")
					.Must(BeTrimmed).WithMessage("OTP must not contain leading or trailing spaces");
			}
		}

		public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordDto>
		{
			public ResetPasswordRequestValidator()
			{
				RuleFor(x => x.ResetToken)
					.NotEmpty().WithMessage("ResetToken is required")
					.Must(BeTrimmed).WithMessage("ResetToken must not contain leading or trailing spaces");

				RuleFor(x => x.NewPassword)
					.NotEmpty().WithMessage("NewPassword is required")
					.MinimumLength(6).WithMessage("NewPassword must be at least 6 characters")
					.MaximumLength(100).WithMessage("NewPassword must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("NewPassword must not contain leading or trailing spaces");
			}

		}

		public class ResendOTPRequestValidator : AbstractValidator<ResendOTPDTO>
		{
			public ResendOTPRequestValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required")
					.EmailAddress().WithMessage("Invalid email format")
					.MaximumLength(100).WithMessage("Email must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");
			}
		}

		public class UpdateProfileImageValidator : AbstractValidator<UpdateProfileImageDTO>
		{
			public UpdateProfileImageValidator()
			{
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.Must(BeTrimmed).WithMessage("Email must not contain leading or trailing spaces");
			}
		}

		public class SocialLoginRequestValidator : AbstractValidator<SocialLoginRequestDTO>
		{
			public SocialLoginRequestValidator()
			{
				RuleFor(x => x.IdToken)
					.NotEmpty().WithMessage("Token is required")
					.NotNull().WithMessage("Token is required")
					.Must(BeTrimmed).WithMessage("Token must not contain leading or trailing spaces");

				RuleFor(x => x.provider)
					.NotEmpty().WithMessage("provider is required")
					.NotNull().WithMessage("provider is required")
					.Must(BeTrimmed).WithMessage("provider must not contain leading or trailing spaces");
			}
		}

		public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
		{
			public ChangePasswordRequestValidator()
			{
				RuleFor(x => x.NewPassword)
					.NotEmpty().WithMessage("NewPassword is required")
					.MinimumLength(6).WithMessage("NewPassword must be at least 6 characters")
					.MaximumLength(100).WithMessage("NewPassword must not exceed 100 characters")
					.Must(BeTrimmed).WithMessage("NewPassword must not contain leading or trailing spaces");
				RuleFor(x => x.ConfirmPassword)
					.NotEmpty().WithMessage("ConfirmPassword is required")
					.Equal(x => x.NewPassword).WithMessage("ConfirmPassword must match NewPassword")
					.Must(BeTrimmed).WithMessage("ConfirmPassword must not contain leading or trailing spaces");

				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("UserId is required")
					.NotNull().WithMessage("UserId is required")
					.Must(BeTrimmed).WithMessage("UserId must not contain leading or trailing spaces");

				RuleFor(x => x).Custom((request, context) =>
				{
					if(request.NewPassword != request.ConfirmPassword)
					{
						context.AddFailure("ConfirmPassword", "ConfirmPassword must match NewPassword");
					}
				});

				RuleFor(x => x).Custom((request, context) =>
				{
					if (request.NewPassword == request.CurrentPassword)
					{
						context.AddFailure("NewPassword", "NewPassword must be different from CurrentPassword");
					}
				});
			}
		}

	}
}