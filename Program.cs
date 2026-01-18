using AwsApi.Services;
using Amazon.EC2;
using Amazon.RDS;
using AwsApi.Services.Interfaces;
using Amazon.Lambda;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddAutoMapper(typeof(AwsApi.Mappers.ContractMapping));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonEC2>();
builder.Services.AddAWSService<IAmazonRDS>();
builder.Services.AddAWSService<IAmazonLambda>();
builder.Services.AddTransient<IEc2Service, Ec2Service>();
builder.Services.AddTransient<IRdsService, RdsService>();
builder.Services.AddTransient<ILambdaService, LambdaService>();
builder.Services.AddTransient<ICostsService, CostsService>();
builder.Services.AddAWSService<Amazon.CostExplorer.IAmazonCostExplorer>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Auth0", options =>
{
    options.Authority = configuration["Auth0:Authority"] ?? Environment.GetEnvironmentVariable("Auth0.Authority");
    options.Audience = configuration["Auth0:Audience"] ?? Environment.GetEnvironmentVariable("Auth0.Audience");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Auth0:Authority"] ?? Environment.GetEnvironmentVariable("Auth0.Authority")
    };
})
.AddJwtBearer("Auth0App2", options =>
{
    options.Authority = configuration["Auth0App2:Authority"] ?? Environment.GetEnvironmentVariable("Auth0App2.Authority");
    options.Audience = configuration["Auth0App2:Audience"] ?? Environment.GetEnvironmentVariable("Auth0App2.Audience");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Auth0App2:Authority"] ?? Environment.GetEnvironmentVariable("Auth0App2.Authority")
    };
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecificOrigin", policy => {
        policy.WithOrigins("https://front.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

// Swagger available in all environments
app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();