using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EventEaseApp;
using EventEaseApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure services with performance optimizations
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IEventService, EventService>(); // Singleton for better performance with static data
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();

// Add new services for user management and attendance
builder.Services.AddSingleton<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

// Build and start the application
var host = builder.Build();

// Initialize performance optimizations
try
{
    var performanceService = host.Services.GetRequiredService<IPerformanceService>();
    await performanceService.PreloadCriticalDataAsync();
    performanceService.WarmupCaches();
}
catch (Exception ex)
{
    Console.WriteLine($"Performance initialization warning: {ex.Message}");
}

await host.RunAsync();
