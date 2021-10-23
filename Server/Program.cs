using Microsoft.AspNetCore.ResponseCompression;
using BlazorWebAssemblySignalRApp.Server.Hubs;
using BlazorWebAssemblySignalRApp.Server;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using BlazorWebAssemblySignalRApp.Server.Repositories;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.AddJsonFile("keys.json", true).Build();

// Add services to the container.
#region snippet_ConfigureServices
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		new[] { "application/octet-stream" });
});

var connectionString = config.GetConnectionString("Default");
var dbName = config["DatabaseName"];

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseCosmos(connectionString, dbName));

builder.Services.AddTransient<IMessageRepository, DatabaseMessageRepository>();
builder.Services.AddSingleton<IGroupRepository, GroupRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
#region snippet_Configure
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToFile("index.html");

app.Run();
#endregion
