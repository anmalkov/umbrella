using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Umbrella.Core.Extensions;
using Umbrella.Core.Services;

namespace Umbrella.Web.Pages
{
    public class ExtensionsModel : PageModel
    {
        public record Extension (string Id, string DisplayName, string Image, string HtmlForRegistration, bool Registered, int EntitiesCount);
        
        private readonly IEnumerable<IExtension> _extensions;
        private readonly IExtensionsService _extensionsService;
        private readonly IEntitiesService _entitiesService;

        public List<Extension> Extensions { get; private set; }
        public string? Error { get; private set; }

        public ExtensionsModel(IEnumerable<IExtension> extensions, IExtensionsService extensionsService, IEntitiesService entitiesService)
        {
            _extensions = extensions ?? new List<IExtension>();
            _extensionsService = extensionsService;
            _entitiesService = entitiesService;
        }
        
        public async Task OnGetAsync(string? error)
        {
            Error = error;
            var registeredExtensions = await _extensionsService.GetRegisteredAsync();
			Extensions = new List<Extension>();
            foreach (var extension in _extensions)
            {
                var registered = registeredExtensions.Any(r => r.Id == extension.Id);
                var entitiesCount = registered ? await _entitiesService.GetCount(extension.Id) : 0;
                Extensions.Add(new Extension(
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
            var parameters = Request.Form.Where(p => !p.Key.StartsWith("__") && p.Key != "extension-id").ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());
            var extension = _extensions.FirstOrDefault(e => e.Id == extensionId);

            if (extension is null)
            {
                return RedirectToPage("Extensions", new { error = $"Extension {extensionId} is not found" });
            }
            
            try
            {
                await _extensionsService.RegisterAsync(extension, parameters);
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
            var extension = _extensions.FirstOrDefault(e => e.Id == extensionId);

            if (extension is null)
            {
                return RedirectToPage("Extensions", new { error = $"Extension {extensionId} is not found" });
            }

            try
            {
                await _extensionsService.UnregisterAsync(extension);
            }
            catch (Exception ex)
            {
                return RedirectToPage("Extensions", new { error = ex.Message });
            }

            return RedirectToPage("Extensions");
        }
    }
}
