using Ffmpeg.Command;
using FFmpeg.API.Endpoints;
using FFmpeg.Core.Interfaces;
using FFmpeg.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
//builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure IIS and Kestrel for larger file uploads
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 104857600; // 100 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.Services.AddSingleton<Ffmpeg.Command.ILogger, Logger>();

// Register FFmpeg Services
builder.Services.AddScoped<IFFmpegServiceFactory>(provider =>
{
    var logger = provider.GetRequiredService<Ffmpeg.Command.ILogger>();
    return new FFmpegServiceFactory(builder.Configuration, logger);
});

// Add file service for handling temporary files
builder.Services.AddScoped<IFileService, FileService>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapEndpoints();

app.MapGet("/", () => { return "FFmpeg API is running"; });
app.Run();

