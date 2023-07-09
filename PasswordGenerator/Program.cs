using PasswordGenerator.BLL.Models;
using PasswordGenerator.BLL.Services.Implementation;
using PasswordGenerator.BLL.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PasswordConfiguration>(options =>
{
    options.SecretKey = builder.Configuration["PasswordSettings:SecretKey"];
    options.ValidityTime = builder.Configuration["PasswordSettings:ValidityTime"];
});

builder.Services.AddScoped<IPasswordHandler, PasswordHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
