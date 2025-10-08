using TubeShopBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton<CartService>();

// НАСТРОЙКА CORS ДЛЯ TELEGRAM MINI APP
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTelegram", policy =>
    {
        policy.AllowAnyOrigin()  // Разрешаем все источники для разработки
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ИСПОЛЬЗУЕМ CORS ПРАВИЛЬНО
app.UseCors("AllowTelegram");
app.UseRouting();
app.MapControllers();

app.Run();