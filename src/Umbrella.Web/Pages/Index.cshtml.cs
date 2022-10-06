using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Umbrella.Core.Events;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IEventsService _eventsService;

        public IndexModel(ILogger<IndexModel> logger, IEventsService eventsService)
        {
            _logger = logger;
            _eventsService = eventsService;
        }

        public void OnGet()
        {

        }

        public void OnPostTestLight()
        {
            bool turnedOn = Request.Form["testLightTurnedOn"] == "on";
            byte.TryParse(Request.Form["testLightBrightness"], out byte brightness);
            var state = new LightEntityState
            {
                TurnedOn = turnedOn,
                Brightness = brightness
            };
            _eventsService.Publish(new ChangeEntityStateEvent<LightEntityState>("light.hue.test", state));
        }
    }
}
