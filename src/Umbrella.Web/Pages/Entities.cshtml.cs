using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Web.Pages;

public class EntitiesModel : PageModel
{
	private readonly IEntitiesService _entitiesService;

	public List<IEntity>? Entities { get; private set; }
	

	public EntitiesModel(IEntitiesService entitiesService)
	{
		_entitiesService = entitiesService;
	}


	public async Task OnGetAsync()
	{
		Entities = await _entitiesService.GetAllAsync();
	}
}