using Chatify;
using Chatify.ApplicationStartUp;
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
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x=>x.JsonSerializerOptions.DefaultIgnoreCondition=JsonIgnoreCondition.WhenWritingNull);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constr")).UseLazyLoadingProxies());
builder.Services.AddScoped<MapToModel>();
builder.Services.Configure<SenderSerivceOptions>(builder.Configuration.GetSection("MailServices"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCompression(x =>
{
    x.EnableForHttps = true;
    x.Providers.Add<BrotliCompressionProvider>();
    x.Providers.Add<GzipCompressionProvider>();
    x.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json","application/xml","text/html","text/js"]);
      
});
builder.Services.Configure<BrotliCompressionProviderOptions>(x => x.Level=CompressionLevel.Optimal);
builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISenderService, SenderService>();
builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection("RefreshToken"));
var jwtOptions=builder.Configuration.GetSection("JWT").Get<JwtOptions>();
builder.Services.AddScoped<IGenerateTokens, GenerateTokens>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<HostedService>();
//builder.Services.AddCors();
builder.Services.Configure<RecaptchaSecret>(builder.Configuration.GetSection("RecaptchaSecret"));
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
//app.UseCors(x => x.WithOrigins("http://localhost:3000").AllowCredentials().AllowAnyHeader().AllowAnyMethod()); ;
app.UseAuthorization();
app.UseMiddleware<RedirectionMiddleware>();
app.MapHub<ChatHub>("/chat");
app.UseStaticFiles();
app.UseResponseCompression();
app.MapGet("/api/ping", () =>
{
    return Results.Ok();
});
app.MapControllers();


app.Run();
