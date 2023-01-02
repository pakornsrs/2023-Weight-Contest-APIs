using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WeightContestService;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

//var test1 = AppDomain.CurrentDomain.BaseDirectory;
//var test2 = Assembly.GetExecutingAssembly().Location;
//var test3 = System.IO.Directory.GetCurrentDirectory();


// read .evn
dotEnvReader.Load();

// cors 
builder.Services.AddCors();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var connectionString2 = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = GlobalData.ConnectionString;
builder.Services.AddDbContext<WeightContestDBContext>(x => x.UseSqlServer(connectionString));

var app = builder.Build();

app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

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
