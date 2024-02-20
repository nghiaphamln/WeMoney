using WeMoney.Middleware;
using WeMoney.Models.Constants;
using WeMoney.Services;

var builder = WebApplication.CreateBuilder(args);

// IOptions AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

#region Dependency Injection
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<PasswordHasher>();
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseMiddleware<RequestTimingMiddleware>();


app.Run();