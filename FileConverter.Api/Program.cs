using FileConverter.Api.DependencyInjection;
using Python.Runtime;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Initialize the embedded Python runtime ONCE, before anything else
// (DI container, Swagger reflection, etc.) can touch Python.Runtime.
Runtime.PythonDLL = @"C:\Users\Webhouse\AppData\Local\Programs\Python\Python314\python314.dll";
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddUseCase().AddInfrastructureServices().AddRepositories();

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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

// Ensure the Python runtime shuts down cleanly when the app stops,
// so a restart doesn't try to re-initialize an already-running engine.
app.Lifetime.ApplicationStopping.Register(() =>
{
    PythonEngine.Shutdown();
});

app.Run();