
#pragma warning disable CS1591 // The class 'HangfireExtensions' is never used
using Hangfire;
using Hangfire.SqlServer;
using Yafa.Infrastructure.Job;

namespace Yafa.Api.Extension
{
	public static class HangfireExtensions
	{
		// ── Called in builder.Services chain ────────────────────────────────
		public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration config)
		{
			services.AddHangfire(hangfireConfig => hangfireConfig
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(
					config.GetConnectionString("DefaultConnection"),
					new SqlServerStorageOptions
					{
						CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
						SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
						QueuePollInterval = TimeSpan.Zero,
						UseRecommendedIsolationLevel = true,
						DisableGlobalLocks = true
					}
				)
			);

			services.AddHangfireServer(options =>
			{
				options.WorkerCount = 2; // low — background jobs only
			});

			// Register job
			services.AddScoped<CheckInReminderJob>();
			services.AddScoped<CheckInCurrentDayReminderJob>();
			services.AddScoped<CheckOutReminderJob>();
			services.AddScoped<HoldExpiryJob>();
			services.AddScoped<BalanceExpiryJob>();
			services.AddScoped<BalancePaymentReminderJob>();

			return services;
		}

		// ── Called in app pipeline ────────────────────────────────────────
		public static WebApplication UseHangfire(this WebApplication app)
		{
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization = new[] { new HangfireAdminAuthFilter() }
			});

			var dubaiZone = TimeZoneInfo.FindSystemTimeZoneById(
				OperatingSystem.IsWindows()
					? "Arab Standard Time"   // Windows
					: "Asia/Dubai"           // Linux (server/Docker)
			);

			// 8:00 AM Dubai → CheckIn reminder
			RecurringJob.AddOrUpdate<CheckInReminderJob>(
				"check-in-reminders",
				job => job.SendCheckInRemindersAsync(),
				"0 8 * * *",
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			// 6:00 AM Dubai → CheckIn reminder
			RecurringJob.AddOrUpdate<CheckInCurrentDayReminderJob>(
				"check-in-current-day-reminders",
				job => job.SendCheckInRemindersInCurrentDayAsync(),
				"0 6 * * *",
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			// 7:00 AM Dubai → CheckOut reminder
			RecurringJob.AddOrUpdate<CheckOutReminderJob>(
				"check-out-reminders",
				job => job.SendCheckOutRemindersAsync(),
				"0 7 * * *",
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			// every 1 min Dubai → Hold expiry check
			RecurringJob.AddOrUpdate<HoldExpiryJob>(
				"hold-expiry",
				job => job.ReleaseExpiredHoldsAsync(),
				"* * * * *",   
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			// every 1 min Dubai → Balance expiry check
			RecurringJob.AddOrUpdate<BalanceExpiryJob>(
				"balance-expiry",
				job => job.CancelExpiredPartialBookingsAsync(),
				"0 * * * *",
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			// daily 9am Dubai → remind guests with an unpaid balance, every day until check-in
			RecurringJob.AddOrUpdate<BalancePaymentReminderJob>(
				"balance-payment-reminder",
				job => job.SendBalancePaymentRemindersAsync(),
				"0 9 * * *",
				new RecurringJobOptions { TimeZone = dubaiZone }
			);

			return app;
		}

	}
}
