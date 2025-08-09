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
public class AITagController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<AITag>> Get()
    {
        return Ok(ctx.AITag.Include(x => x.AIPrompt));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AITag>> GetAsync(Guid key)
    {
        var aITag = await ctx.AITag.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aITag == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(aITag);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AITag>> PostAsync(AITag aITag)
    {
        var record = await ctx.AITag.FindAsync(aITag.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        var aIPrompt = aITag.AIPrompt;
        aITag.AIPrompt = null;

        await ctx.AITag.AddAsync(aITag);

        if (aIPrompt != null)
        {
            var newValues = await ctx.AIPrompt.Where(x => aIPrompt.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            aITag.AIPrompt = [..newValues];
        }

        await ctx.SaveChangesAsync();

        return Created($"/aitag/{aITag.Id}", aITag);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AITag>> PutAsync(Guid key, AITag update)
    {
        var aITag = await ctx.AITag.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aITag == null)
        {
            return NotFound();
        }

        ctx.Entry(aITag).CurrentValues.SetValues(update);

        if (update.AIPrompt != null)
        {
            var updateValues = update.AIPrompt.Select(x => x.Id);
            aITag.AIPrompt ??= [];
            aITag.AIPrompt.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !aITag.AIPrompt.Select(y => y.Id).Contains(x));
            var newValues = await ctx.AIPrompt.Where(x => addValues.Contains(x.Id)).ToListAsync();
            aITag.AIPrompt.AddRange(newValues);
        }

        await ctx.SaveChangesAsync();

        return Ok(aITag);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AITag>> PatchAsync(Guid key, Delta<AITag> delta)
    {
        var aITag = await ctx.AITag.Include(x => x.AIPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aITag == null)
        {
            return NotFound();
        }

        delta.Patch(aITag);

        await ctx.SaveChangesAsync();

        return Ok(aITag);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid key)
    {
        var aITag = await ctx.AITag.FindAsync(key);

        if (aITag != null)
        {
            ctx.AITag.Remove(aITag);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
