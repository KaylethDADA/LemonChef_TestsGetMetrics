using Application.Dto_s.Recipe.Requests;
using Application.Dto_s.Recipe.Responses;
using Application.Interfaces.Services;
using LemonChefApi.TestsMetrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LemonChefApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly MetricsBase _metricsBase;

    public RecipesController(MetricsBase metricsBase)
    {
        _metricsBase = metricsBase;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] RecipeCreateRequest request, 
        [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, 
        [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUserIdAsync(Guid userId, 
        [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.GetAllByUserIdAsync(userId, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{recipeId:guid}/ingredients")]
    public async Task<IActionResult> GetIngredientsByRecipeIdAsync(Guid recipeId, 
        [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.GetRecipeIngredientsByRecipeId(recipeId, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAsync([FromBody] RecipeUpdateRequest request, [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync(Guid id, [FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.DeleteByIdAsync(id, userId, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPagedAsync([FromQuery] RecipeGetAllPagedRequest request,[FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.GetAllPagedAsync(request.PageNumber, request.PageSize,cancellationToken);
        return Ok(result);
    }

    [HttpGet("tg/{id:int}")]
    public async Task<IActionResult> GetAllByTelegramIdAsync(int id,[FromServices] IRecipeService service, CancellationToken cancellationToken)
    {
        var result = await service.GetAllByTelegramIdAsync(id,cancellationToken);
        return Ok(result);
    }
}