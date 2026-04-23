using Pharmacy.Api.Data;
using Pharmacy.Api.Models; // Добавлено для доступа к классу Medication
using Microsoft.EntityFrameworkCore;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка строки подключения к БД
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                          ?? "Host=localhost;Port=5432;Database=pharmacydb;Username=postgres;Password=postgres";

// 2. Настройка подключения к Redis
string redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
                         ?? "localhost:6379,abortConnect=false";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. БЛОК МИГРАЦИЙ И ЗАПОЛНЕНИЯ ДАННЫХ (SEED)
using (IServiceScope scope = app.Services.CreateScope())
{
    try
    {
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("---> Running Migrations...");
        dbContext.Database.Migrate();

        if (!dbContext.Medications.Any())
        {
            Console.WriteLine("---> Seeding Database with initial medications...");
            dbContext.Medications.AddRange(
                new Medication { Name = "Парацетамол", Category = "Анальгетики", Price = 45, StockQuantity = 100 },
                new Medication { Name = "Ибупрофен", Category = "Анальгетики", Price = 89, StockQuantity = 60 },
                new Medication { Name = "Амоксициллин", Category = "Антибиотики", Price = 210, StockQuantity = 30 },
                new Medication { Name = "Лоратадин", Category = "Антигистаминные", Price = 120, StockQuantity = 50 }
            );
            dbContext.SaveChanges();
            Console.WriteLine("---> Seeding Complete!");
        }
        else
        {
            Console.WriteLine("---> Database already contains data, skipping seed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---> Error during startup: {ex.Message}");
    }
}

// 4. Настройка Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpMetrics();
app.MapMetrics();

app.UseAuthorization();
app.MapControllers();

app.Run();