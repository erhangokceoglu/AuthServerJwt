using SharedLibrary.Options;
using SharedLibrary.Extensions;
using MiniApp1.Api.ClaimsRequirements;
using Microsoft.AspNetCore.Authorization;
using static MiniApp1.Api.ClaimsRequirements.BirthDateRequirement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
var tokenOption = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
builder.Services.AddCustomTokenAuth(tokenOption!);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthorizationHandler, BirthDateRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("City", "ankara");
    });
    options.AddPolicy("AgePolicy", policy =>
    {
        policy.Requirements.Add(new BirthDateRequirement(18));
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
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
