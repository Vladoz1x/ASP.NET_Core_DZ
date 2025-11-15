var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    if (!context.Request.Query.ContainsKey("value"))
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Помилка: потрібно передати параметр ?value=...");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    var valueStr = context.Request.Query["value"];
    if (!int.TryParse(valueStr, out int number))
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Помилка: введіть ціле число.");
        return;
    }

    if (number < 1 || number > 100000)
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Помилка: число має бути від 1 до 100000.");
        return;
    }

    context.Items["number"] = number;
    await next();
});

app.Use(async (context, next) =>
{
    int number = (int)context.Items["number"];
    string interpretation = $"Ви ввели число: {number}";
    interpretation += number % 2 == 0 ? " (парне)" : " (непарне)";
    context.Items["interpretation"] = interpretation;
    await next();
});

app.Run(async context =>
{
    context.Response.ContentType = "text/plain; charset=utf-8";
    string result = context.Items["interpretation"]?.ToString() ?? "Немає результату";
    await context.Response.WriteAsync(result);
});

app.Run();
