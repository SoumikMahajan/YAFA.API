using Yafa.Api.Extension;

var builder = WebApplication.CreateBuilder(args);

// QuestPDF requires an explicit license type — Community is free for organizations under
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Apply Extensions
builder.Services
    .ConfigureKestrel()
    .ConfigureDatabase(builder.Configuration)
    .ConfigureJwtAuthentication(builder.Configuration)
    .ConfigureSwagger()
    .RegisterApplicationServices(builder.Configuration)
    .ConfigureHangfire(builder.Configuration);


var app = builder.Build();

// Scalar UI (both dev & prod)
app.ConfigurScalarUI();

// Apply Middleware Pipeline
app.ConfigureMiddlewarePipeline();
app.UseHangfire();

app.Run();
