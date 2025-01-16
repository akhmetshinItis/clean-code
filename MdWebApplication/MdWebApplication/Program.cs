using Application.Interfaces.Services;
using DataAccess;
using DataAccess.Repositories;
using Infrastructure;
using MdWebApplication.Interfaces.Repositories;
using MdWebApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PI.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();
services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString("AppDbContext"));
    }
);
services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IUsersService,UserService>();
services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.AddApiAuthentification(configuration);
builder.Services.AddSingleton<MinioService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapCustomRoutes();

app.MapControllers();
app.Run();