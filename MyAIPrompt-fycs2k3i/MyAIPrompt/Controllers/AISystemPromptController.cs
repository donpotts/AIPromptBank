using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MyAIPrompt.Data;
using MyAIPrompt.Shared.Models;

namespace MyAIPrompt.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Fixed")]
public class AISystemPromptController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<AISystemPrompt>> Get()
    {
        return Ok(ctx.AISystemPrompt.Include(x => x.AIPrompt));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AISystemPrompt>> GetAsync(Guid key)
    {
        var aISystemPrompt = await ctx.AISystemPrompt.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aISystemPrompt == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(aISystemPrompt);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AISystemPrompt>> PostAsync(AISystemPrompt aISystemPrompt)
    {
        var record = await ctx.AISystemPrompt.FindAsync(aISystemPrompt.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        var aIPrompt = aISystemPrompt.AIPrompt;
        aISystemPrompt.AIPrompt = null;

        await ctx.AISystemPrompt.AddAsync(aISystemPrompt);

        if (aIPrompt != null)
        {
            var newValues = await ctx.AIPrompt.Where(x => aIPrompt.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            aISystemPrompt.AIPrompt = [..newValues];
        }

        await ctx.SaveChangesAsync();

        return Created($"/aisystemprompt/{aISystemPrompt.Id}", aISystemPrompt);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AISystemPrompt>> PutAsync(Guid key, AISystemPrompt update)
    {
        var aISystemPrompt = await ctx.AISystemPrompt.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aISystemPrompt == null)
        {
            return NotFound();
        }

        ctx.Entry(aISystemPrompt).CurrentValues.SetValues(update);

        if (update.AIPrompt != null)
        {
            var updateValues = update.AIPrompt.Select(x => x.Id);
            aISystemPrompt.AIPrompt ??= [];
            aISystemPrompt.AIPrompt.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !aISystemPrompt.AIPrompt.Select(y => y.Id).Contains(x));
            var newValues = await ctx.AIPrompt.Where(x => addValues.Contains(x.Id)).ToListAsync();
            aISystemPrompt.AIPrompt.AddRange(newValues);
        }

        await ctx.SaveChangesAsync();

        return Ok(aISystemPrompt);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AISystemPrompt>> PatchAsync(Guid key, Delta<AISystemPrompt> delta)
    {
        var aISystemPrompt = await ctx.AISystemPrompt.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aISystemPrompt == null)
        {
            return NotFound();
        }

        delta.Patch(aISystemPrompt);

        await ctx.SaveChangesAsync();

        return Ok(aISystemPrompt);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid key)
    {
        var aISystemPrompt = await ctx.AISystemPrompt.FindAsync(key);

        if (aISystemPrompt != null)
        {
            ctx.AISystemPrompt.Remove(aISystemPrompt);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
