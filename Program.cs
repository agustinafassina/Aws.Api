using AwsApi.Services;
using Amazon.EC2;
using Amazon.RDS;
using AwsApi.Services.interfaces;
using Amazon.Lambda;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AwsApi.Mappers.ContractMapping));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonEC2>();
builder.Services.AddAWSService<IAmazonRDS>();
builder.Services.AddAWSService<IAmazonLambda>();
builder.Services.AddTransient<IEc2Service, Ec2Service>();
builder.Services.AddTransient<IRdsService, RdsService>();
builder.Services.AddTransient<ILambdaService, LambdaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();