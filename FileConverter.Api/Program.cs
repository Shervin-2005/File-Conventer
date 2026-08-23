using Amazon.S3;
using FileConverter.Api.DependencyInjection;
using FileConverter.Application.Interfaces;
using FileConverter.Application.Mappings;
using FileConverter.Application.Settings;
using FileConverter.Infrastracture.Data;
using FileConverter.Infrastructure.Settings;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Python.Runtime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable("FILE_CONVERTER")
 ?? throw new Exception("Environment variable FILE_CONVERTER is not set");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Initialize the embedded Python runtime ONCE, before anything else
// (DI container, Swagger reflection, etc.) can touch Python.Runtime.
Runtime.PythonDLL = @"C:\Users\CLINIC\AppData\Local\Programs\Python\Python314\python314.dll";
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:ServiceApiKey"];
}, typeof(MappingProfile));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddUseCase().AddInfrastructureServices();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.Configure<S3Settings>(
    builder.Configuration.GetSection("ArvanStorage")
);

builder.Services.Configure<SmsSettings>(
    builder.Configuration.GetSection("OTPOptions")
);

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var settings = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<S3Settings>>().Value;
    var credentials = new Amazon.Runtime.BasicAWSCredentials(
        settings.AccessKey,
        settings.SecretKey);

    var config = new AmazonS3Config
    {
        ServiceURL = settings.ServiceUrl,
        ForcePathStyle = true
    };

    return new AmazonS3Client(credentials, config);
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)
            ),

            ClockSkew = TimeSpan.Zero
        };

        //// Read JWT token from cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("AccessToken", out var accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });


// ─── افزایش سقف حجم آپلود (پیش‌فرض حدوداً ۳۰ مگابایت بود) ────────────────
// این محدودیت هم در سطح Kestrel و هم در سطح خودِ multipart form data چک
// می‌شود؛ برای رفع کامل مشکل باید هر دو را باز کرد، وگرنه یکی از دو لایه
// همچنان درخواست‌های بزرگ را رد می‌کند.
const long MaxUploadSize = 500L * 1024 * 1024; // 500 مگابایت — عدد را بر اساس نیاز واقعی خودت تنظیم کن

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = MaxUploadSize;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = MaxUploadSize;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// اجازه به فرانت‌اند (Vite dev server روی origin متفاوت، مثلاً 5173) برای
// زدن درخواست به این API. بدون این، مرورگر درخواست‌های fetch را با خطای
// CORS بلاک می‌کند (حتی اگر مسیر و متد درست باشند).
// ⚠️ اگر پورت فرانت در محیط توسعه‌ی شما فرق دارد، آدرس زیر را عوض کن.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

// باید قبل از UseAuthorization و MapControllers باشد.
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure the Python runtime shuts down cleanly when the app stops,
// so a restart doesn't try to re-initialize an already-running engine.
app.Lifetime.ApplicationStopping.Register(() =>
{
    PythonEngine.Shutdown();
});

app.Run();