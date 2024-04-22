using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPattern.EFcore.ExtraServices;
using RepositoryPatternUOW.Core.Interfaces;
using RepositoryPatternUOW.EFcore;
using RepositoryPatternUOW.EFcore.Mapperly;
using RepositoryPatternUOW.EFcore.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constr")).UseLazyLoadingProxies());
builder.Services.AddScoped<MapToModel>();
builder.Services.Configure<SenderSerivceOptions>(builder.Configuration.GetSection("MailServices"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISenderService, SenderService>();
builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection("RefreshToken"));
var jwtOptions=builder.Configuration.GetSection("JWT").Get<JwtOptions>();
builder.Services.AddScoped<IGenerateTokens, GenerateTokens>();
builder.Services.AddCors();
builder.Services.AddSingleton(jwtOptions!);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {

    };
});
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x.WithOrigins("http://localhost:3000").AllowCredentials().AllowAnyHeader().AllowAnyMethod()); ;
app.UseAuthorization();

app.MapControllers();

app.Run();
