using Umbrella.Core.Extensions;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;
using Umbrella.Extensions.Hue;
using Umbrella.Extensions.Xiaomi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//builder.Services.AddHttpClient();

builder.Services.AddSingleton<IExtension, HueExtension>();
builder.Services.AddSingleton<IExtension, XiaomiExtension>();

builder.Services.AddSingleton<IEventsService, EventsService>();
builder.Services.AddSingleton<IEntitiesStateService, EntitiesStateService>();
builder.Services.AddSingleton<ICoreService, CoreService>();
builder.Services.AddSingleton<IExtensionsService, ExtensionsService>();
builder.Services.AddSingleton<IExtensionRepository, ExtensionRepository>();
builder.Services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
builder.Services.AddSingleton<IEntitiesService, EntitiesService>();
builder.Services.AddSingleton<IEntitiesRepository, EntitiesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var coreService = app.Services.GetService<ICoreService>();
await coreService!.StartAsync();

app.Run();

await coreService!.StopAsync();

