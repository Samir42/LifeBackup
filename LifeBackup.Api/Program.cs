using Amazon.S3;
using LifeBackup.Core.Interfaces;
using LifeBackup.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// Add Amazon Services
builder.Services.AddAWSService<IAmazonS3>(builder.Configuration.GetAWSOptions());

builder.Services.AddSingleton<IBucketRepository, BucketRepository>();
builder.Services.AddSingleton<IFileRepository, FileRepository>();

builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseExceptionHandler(a => a.Run(async context =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;

    var result = JsonConvert.SerializeObject(new { error = exception?.Message });
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(result);
}));



app.UseMvc();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


public partial class Program { }