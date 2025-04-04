using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data; // Подключаем контекст базы данных
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// ✅ Добавляем CORS перед `builder.Build()`
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Добавляем поддержку LocalDB (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Добавляем поддержку контроллеров (для API)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Включаем CORS перед маршрутизацией
app.UseCors("AllowAll");

// ✅ Настройка Swagger (UI для тестирования API)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Включаем маршрутизацию и поддержку API-контроллеров
app.UseRouting();
app.UseAuthorization();
app.MapControllers(); // Добавляем контроллеры

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "SavedTests")),
    RequestPath = "/savedtests"
});




app.Run();
