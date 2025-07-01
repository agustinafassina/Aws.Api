using AwsApi.Services;
using Amazon.EC2;
using Amazon.RDS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonEC2>();
builder.Services.AddAWSService<IAmazonRDS>();
builder.Services.AddTransient<IEc2Service, Ec2Service>();
builder.Services.AddTransient<IRdsService, RdsService>();

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