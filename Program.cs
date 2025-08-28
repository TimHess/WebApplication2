using Steeltoe.Common;
using Microsoft.AspNetCore.HttpOverrides;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Security.Authentication.CloudFoundry;

var builder = WebApplication.CreateBuilder(args);

builder.AddCloudFoundryConfiguration();
// Add services to the container.

builder.Services.AddAuthentication()
    .AddCloudFoundryJwtBearer(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("sampleapi.read", policy => policy.RequireClaim("scope", "sampleapi.read"));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

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
