using AwsApi.Services;
using Amazon.EC2;
using Amazon.RDS;
using AwsApi.Services.interfaces;
using Amazon.Lambda;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();