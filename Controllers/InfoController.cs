using Microsoft.AspNetCore.Mvc;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    // Both are registered by ASP.NET Core automatically
    public InfoController(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    // GET api/info
    [HttpGet]
    public object Get()
    {
        return new
        {
            name = _config["TrainingCenter:Name"],
            maxPageSize = _config.GetValue<int>("TrainingCenter:MaxPageSize"),
            environment = _env.EnvironmentName
        };
    }
    
}