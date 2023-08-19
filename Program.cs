using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using personal_project.Data;
using personal_project.Hubs;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });
  options.OperationFilter<SecurityRequirementsOperationFilter>();
});
// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Connect SQL Server
DotNetEnv.Env.Load(); // use DotNetEnv to load .env
var connectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING_LOCAL");
builder.Services.AddDbContext<WebDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add SignalR
builder.Services.AddSignalR();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidIssuer = System.Environment.GetEnvironmentVariable("JWT_ISSUER"),
      ValidateAudience = true,
      ValidAudience = System.Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
      ValidateLifetime = true,
      // make sure expired on time
      ClockSkew = TimeSpan.Zero,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Environment.GetEnvironmentVariable("JWT_KEY")))
    };
  });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<VideoChatHub>("/videoChatHub");
app.MapHub<VideoHub>("/videoHub");
app.MapHub<TestHub>("/testHub");

app.Run();
