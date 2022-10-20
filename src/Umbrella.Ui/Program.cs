using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.Core.Extensions;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;
using Umbrella.Extensions.Hue;
using Umbrella.Extensions.Xiaomi;
using Umbrella.Ui.Extensions;
using Umbrella.Ui.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(m => m.AsScoped(), typeof(Program));

//builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IExtension, HueExtension>();
builder.Services.AddSingleton<IExtension, XiaomiExtension>();

builder.Services.AddSingleton<IEventsService, EventsService>();
builder.Services.AddSingleton<IEntitiesStateService, EntitiesStateService>();
builder.Services.AddSingleton<ICoreService, CoreService>();
builder.Services.AddSingleton<IExtensionsService, ExtensionsService>();
builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
builder.Services.AddSingleton<IEntitiesService, EntitiesService>();
builder.Services.AddSingleton<IAreasService, AreasService>();
builder.Services.AddSingleton<IGroupsService, GroupsService>();

builder.Services.AddSingleton<IExtensionRepository, ExtensionRepository>();
builder.Services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddSingleton<IEntitiesRepository, EntitiesRepository>();
builder.Services.AddSingleton<IAreasRepository, AreasRepository>();
builder.Services.AddSingleton<IGroupsRepository, GroupsRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseRouting();

app.MediateGet<GetExtensionsRequest>("/api/extensions");
app.MediatePost<RegisterExtensionRequest>("/api/extensions/{id}");
app.MediateDelete<UnregisterExtensionRequest>("/api/extensions/{id}");
app.MediateGet<GetEntitiesRequest>("/api/entities");
app.MediateGet<GetEntitiesStatesRequest>("/api/entities/states");
app.MediateGet<GetAreasRequest>("/api/areas");
app.MediateGet<GetGroupsRequest>("/api/groups");

app.MapFallbackToFile("index.html");

var coreService = app.Services.GetService<ICoreService>();
await coreService!.StartAsync();

app.Run();

await coreService!.StopAsync();
