using ChatAssignmentApp.Core;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory;
using ChatAssignmentApp.Queuing;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var config = new Configuration();
builder.Configuration.Bind(config);

builder.Services.AddSingleton(config);

// Add services to the container.
builder.Services.InjectMemory();
builder.Services.InjectQueuing();
builder.Services.InjectCore();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
