using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect SQL Server
DotNetEnv.Env.Load(); // use DotNetEnv to load .env
var connectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING_LOCAL");
builder.Services.AddDbContext<WebDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add SignalR
builder.Services.AddSignalR();

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
