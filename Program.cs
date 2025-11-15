using WeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new WeatherService("9cae67b57a369c614bfcf2657bab628e"));

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.MapGet("/", context => {
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.Run();
