using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Umbrella.Core.Events;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Web.Pages
{
    public class IndexModel : PageModel
    {
        public LightEntityState? State { get; private set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly IEventsService _eventsService;
        private readonly IEntitiesService _entitiesService;
        
        public IndexModel(ILogger<IndexModel> logger, IEventsService eventsService, IEntitiesService entitiesService)
        {
            _logger = logger;
            _eventsService = eventsService;
            _entitiesService = entitiesService;
        }

        public void OnGet()
        {
            var state = _entitiesService.GetState("light.hue.test");
            State = state as LightEntityState;
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
            _eventsService.Publish(new ChangeEntityStateEvent("light.hue.test", state));
        }
    }
}
