using Microsoft.AspNetCore.Mvc;
using ProjectMuseum.Services.StorySceneService;

namespace ASP.NetCore7.ProjectMuseum.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly IStorySceneService _storySceneService;

    public StoryController(IStorySceneService storySceneService)
    {
        _storySceneService = storySceneService;
    }
    [HttpGet("GetStoryScene/{sceneNo}")]
    public async Task<IActionResult> GetStoryScene(int sceneNo)
    {
        var storyScene = await _storySceneService.GetStorySceneBySceneNo(sceneNo);
        return Ok(storyScene);
    }
    [HttpGet("GetTutorialScene/{sceneNo}")]
    public async Task<IActionResult> GetTutorialScene(int sceneNo)
    {
        var tutorial = await _storySceneService.GetTutorialByNumber(sceneNo);
        return Ok(tutorial);
    }
}