using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Umbrella.Core.Events;
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

        public void OnPostTurnOn()
        {
            _eventsService.Publish(EventNames.LightChangeState, new LightChangeStateEvent("light.hue.test", turnedOn: true));
        }
        
        public void OnPostTurnOff()
        {
            _eventsService.Publish(EventNames.LightChangeState, new LightChangeStateEvent("light.hue.test", turnedOn: false));
        }
    }
}