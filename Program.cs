using Microsoft.EntityFrameworkCore;
using TrainingCenterApi.Data;
using TrainingCenterApi.Services;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using TrainingCenterApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();


builder.Services.AddDbContext<TrainingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
// Authentication: how to check the JWT in every request
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            RoleClaimType = ClaimTypes.Role
        };
    });

// Authorization: who can do what
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseAuthentication();   // 1. who are you?
app.UseAuthorization();    // 2. what can you do?

app.UseExceptionHandler();

// Our first middleware: logs every request and its response
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

    await next(); // Pass the request to the next middleware

    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TrainingCenterApi v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
