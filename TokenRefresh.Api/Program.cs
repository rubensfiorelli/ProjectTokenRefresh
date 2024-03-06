using TokenRefresh.Api.ApiConfig;
using TokenRefresh.Api.Authentication;
using TokenRefresh.Api.DataConfig;
using TokenRefresh.Api.IdentityConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

builder.Services
    .AddDataConfig(builder.Configuration)
    .AddIdentityConfiguration(builder.Configuration)
    .AddApiConfig();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
