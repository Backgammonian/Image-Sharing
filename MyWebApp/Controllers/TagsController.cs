using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Controllers
{
    public sealed class TagsController : Controller
    {
        private readonly ITagsRepository _tagsRepository;

        public TagsController(ITagsRepository tagsRepository)
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
