using Chatify;
using Chatify.Services;
using Chatify.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPattern.EFcore;
using RepositoryPattern.EFcore.ExtraServices;
using RepositoryPattern.EFcore.Repositories;
using RepositoryPatternUOW.EFcore;
using RepositoryPatternUOW.EFcore.Mapperly;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constr")).UseLazyLoadingProxies());
builder.Services.AddScoped<MapToModel>();
builder.Services.Configure<SenderSerivceOptions>(builder.Configuration.GetSection("MailServices"));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISenderService, SenderService>();
builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection("RefreshToken"));
var jwtOptions=builder.Configuration.GetSection("JWT").Get<JwtOptions>();
builder.Services.AddScoped<IGenerateTokens, GenerateTokens>();
builder.Services.AddSignalR();
builder.Services.AddCors();
builder.Services.AddSingleton(jwtOptions!);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
    };
    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = e =>
        {
            if(e.Request.Cookies.TryGetValue("jwt",out string? val))
            {
                e.Token=val;
            }
            return Task.CompletedTask;
        }
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
app.UseMiddleware<RedirectionMiddleware>();
app.MapHub<ChatHub>("/chat");
app.UseStaticFiles();
app.MapControllers();

app.Run();
