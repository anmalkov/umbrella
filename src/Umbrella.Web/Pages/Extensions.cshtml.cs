using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Umbrella.Core.Extensions;
using Umbrella.Core.Services;

namespace Umbrella.Web.Pages
{
    public class ExtensionsModel : PageModel
    {
        public record ExtensionViewModel (string Id, string? DisplayName, string? Image, string? HtmlForRegistration, bool Registered, int EntitiesCount);
        
        private readonly IExtensionsService _extensionsService;
        private readonly IEntitiesService _entitiesService;

        public List<ExtensionViewModel>? Extensions { get; private set; } = null;
        public string? Error { get; private set; }

        public ExtensionsModel(IExtensionsService extensionsService, IEntitiesService entitiesService)
        {
            _extensionsService = extensionsService;
            _entitiesService = entitiesService;
        }
        
        public async Task OnGetAsync(string? error)
        {
            Error = error;
            var registeredExtensions = await _extensionsService.GetRegisteredAsync();
			Extensions = new List<ExtensionViewModel>();
            foreach (var extension in await _extensionsService.GetAllAsync())
            {
                var registered = registeredExtensions.Any(r => r.Id == extension.Id);
                var entitiesCount = registered ? await _entitiesService.GetCount(extension.Id) : 0;
                Extensions.Add(new ExtensionViewModel(
                    extension.Id,
                    extension.DisplayName,
                    extension.Image,
                    extension.HtmlForRegistration,
                    registered,
                    entitiesCount
                    )
                );
			}
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            var extensionId = Request.Form["extension-id"];
            if (string.IsNullOrWhiteSpace(extensionId))
            {
                return RedirectToPage("Extensions", new { error = $"Select an extension to register" });
            }
            var parameters = Request.Form.Where(p => !p.Key.StartsWith("__") && p.Key != "extension-id").ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());
            
            try
            {
                await _extensionsService.RegisterAsync(extensionId!, parameters);
            }
            catch (Exception ex)
            {
                return RedirectToPage("Extensions", new { error = ex.Message });
            }
            
            return RedirectToPage("Extensions");
        }

        public async Task<IActionResult> OnPostUnregisterAsync()
        {
            var extensionId = Request.Form["extension-id"];
            if (string.IsNullOrWhiteSpace(extensionId))
            {
                return RedirectToPage("Extensions", new { error = $"Select an extension to unregister" });
            }

            try
            {
                await _extensionsService.UnregisterAsync(extensionId!);
            }
            catch (Exception ex)
            {
                return RedirectToPage("Extensions", new { error = ex.Message });
            }

            return RedirectToPage("Extensions");
        }
    }
}
