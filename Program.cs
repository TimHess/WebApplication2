using Steeltoe.Configuration.CloudFoundry;
using Steeltoe.Configuration.CloudFoundry.ServiceBindings;
using Steeltoe.Security.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddCloudFoundryConfiguration();
builder.Configuration.AddCloudFoundryServiceBindings();

// Add services to the container.

builder.Services.AddAuthentication()
    .AddJwtBearer().ConfigureJwtBearerForCloudFoundry();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("sampleapi.read", policy => policy.RequireClaim("scope", "sampleapi.read"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/jwt", async httpContext =>
{
    httpContext.Response.StatusCode = 200;
    httpContext.Response.ContentType = "text/plain";
    await httpContext.Response.WriteAsync("JWT is valid and contains the required claim");
}).RequireAuthorization("sampleapi.read");

app.Run();
