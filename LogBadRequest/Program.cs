using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Errors = e.Value?.Errors.Select(x => x.ErrorMessage)
            });

        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<Program>>(); // Or use your logger

        logger.LogWarning("Model validation failed: {@Errors}", errors);
        
        return new BadRequestObjectResult(context.ModelState);
    };
});
var app = builder.Build();

app.MapControllers();

app.Run();

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    public ActionResult<Foo> Post(Foo bar)
    {
        return Ok(bar);
    }
}

public class Foo
{
    public string Bar { get; set; }
}