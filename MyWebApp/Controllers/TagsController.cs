using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;

namespace MyWebApp.Controllers
{
    public sealed class TagsController : Controller
    {
        private readonly TagsRepository _tagsRepository;

        public TagsController(TagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
        }

        [HttpGet]
        [Route("Tags/GetByTag/{tag}")]
        public async Task<IActionResult> GetByTag(string? tag)
        {
            return View(await _tagsRepository.GetByTag(tag));
        }
    }
}
