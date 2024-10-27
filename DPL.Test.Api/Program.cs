using System;
using System.Runtime.Intrinsics.X86;
using DPL.Test.Api.ObjectModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
// Create a directory for application data if it doesn't exist
var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DPLTest.Api");
if (!Directory.Exists(appDataPath))
{
    Directory.CreateDirectory(appDataPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Use InMemory context for testing purposes (e.g., for unit tests)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));


builder.Services.AddControllers();
    
builder.Services.ConfigureHttpJsonOptions(options =>
{

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(client => client.AddPolicy("allow_all", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
var app = builder.Build();
app.UseCors("allow_all");


app.MapPost("/api/auth/login",  async ([FromBody] AppUser user,HttpContext httpContext)  =>
{
    var signed = user.Email == "abc@dpl.com" && user.Password == "123";
    if (signed)
    {
        return Results.Ok((signed ? new { token = "_SinDPLT132##@TOK" } : new { }));
    }
    else
    {
        return Results.Unauthorized();
    }
    
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

