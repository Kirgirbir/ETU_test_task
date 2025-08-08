using ETU_test_task;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;

string? path = (string?)AppContext.BaseDirectory;
Console.WriteLine(path);

var XMLpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.xml");
var EquipmentLogic = new EquipmentLogic(XMLpath);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Equipment Data Access API", Version = "v1" });
});

var app = builder.Build();

app.UseStaticFiles();

//Простая страничка для тестирования АПИ
app.MapGet("/home", async context =>
{
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
});

app.MapGet("/api/equipment", async (HttpContext context, [FromHeader(Name = "path")] string encoded_path) =>
{
    //т.к. кириллицу нельзя нормально послать в запросе, её сначала надо расшифровать
    string path = WebUtility.UrlDecode(encoded_path);

    if (string.IsNullOrWhiteSpace(path))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "Путь не был передан" });
        return;
    }

    string? id = EquipmentLogic.IdFromPath(path);

    if (id == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsJsonAsync(new { error = "Найти id не вышло" });
        return;
    }

    await context.Response.WriteAsJsonAsync(new { id = id });
})
.WithName("GetEquipmentByPath")
.WithOpenApi();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Equipment Data Access API v1");
    c.RoutePrefix = "api/equipment/swagger";
});

app.Run("https://localhost:7001");
