using Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blazored.LocalStorage;
using Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<BarcodeScannerService>();



var baseAddress = builder.Configuration.GetValue<string>("BaseApi");
if (string.IsNullOrEmpty(baseAddress))
{
    throw new ArgumentNullException("BaseAddress cannot be null or empty.");
}

// ثبت سرویس HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

// ثبت سرویس ApiClient
builder.Services.AddScoped<ApiClient>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new ApiClient(baseAddress);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
