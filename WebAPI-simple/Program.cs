using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// ===== Thêm CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger",
        policy =>
        {
            policy.WithOrigins("http://localhost:5177", "https://localhost:7146") // URL của Swagger UI nếu khác thì đổi
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSwagger");

app.UseAuthorization();

app.MapControllers();

app.Run();
