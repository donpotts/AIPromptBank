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
public class AIPromptController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<AIPrompt>> Get()
    {
        return Ok(ctx.AIPrompt.Include(x => x.AITag).Include(x => x.AISystemPrompt));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIPrompt>> GetAsync(Guid key)
    {
        var aIPrompt = await ctx.AIPrompt.Include(x => x.AITag).Include(x => x.AISystemPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aIPrompt == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(aIPrompt);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AIPrompt>> PostAsync(AIPrompt aIPrompt)
    {
        var record = await ctx.AIPrompt.FindAsync(aIPrompt.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        var aITag = aIPrompt.AITag;
        aIPrompt.AITag = null;

        var aISystemPrompt = aIPrompt.AISystemPrompt;
        aIPrompt.AISystemPrompt = null;

        await ctx.AIPrompt.AddAsync(aIPrompt);

        if (aITag != null)
        {
            var newValues = await ctx.AITag.Where(x => aITag.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            aIPrompt.AITag = [..newValues];
        }

        if (aISystemPrompt != null)
        {
            var newValues = await ctx.AISystemPrompt.Where(x => aISystemPrompt.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            aIPrompt.AISystemPrompt = [..newValues];
        }

        await ctx.SaveChangesAsync();

        return Created($"/aiprompt/{aIPrompt.Id}", aIPrompt);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIPrompt>> PutAsync(Guid key, AIPrompt update)
    {
        var aIPrompt = await ctx.AIPrompt.Include(x => x.AITag).Include(x => x.AISystemPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aIPrompt == null)
        {
            return NotFound();
        }

        ctx.Entry(aIPrompt).CurrentValues.SetValues(update);

        if (update.AITag != null)
        {
            var updateValues = update.AITag.Select(x => x.Id);
            aIPrompt.AITag ??= [];
            aIPrompt.AITag.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !aIPrompt.AITag.Select(y => y.Id).Contains(x));
            var newValues = await ctx.AITag.Where(x => addValues.Contains(x.Id)).ToListAsync();
            aIPrompt.AITag.AddRange(newValues);
        }

        if (update.AISystemPrompt != null)
        {
            var updateValues = update.AISystemPrompt.Select(x => x.Id);
            aIPrompt.AISystemPrompt ??= [];
            aIPrompt.AISystemPrompt.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !aIPrompt.AISystemPrompt.Select(y => y.Id).Contains(x));
            var newValues = await ctx.AISystemPrompt.Where(x => addValues.Contains(x.Id)).ToListAsync();
            aIPrompt.AISystemPrompt.AddRange(newValues);
        }

        await ctx.SaveChangesAsync();

        return Ok(aIPrompt);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIPrompt>> PatchAsync(Guid key, Delta<AIPrompt> delta)
    {
        var aIPrompt = await ctx.AIPrompt.Include(x => x.AITag).Include(x => x.AISystemPrompt).FirstOrDefaultAsync(x => x.Id == key);

        if (aIPrompt == null)
        {
            return NotFound();
        }

        delta.Patch(aIPrompt);

        await ctx.SaveChangesAsync();

        return Ok(aIPrompt);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid key)
    {
        var aIPrompt = await ctx.AIPrompt.FindAsync(key);

        if (aIPrompt != null)
        {
            ctx.AIPrompt.Remove(aIPrompt);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
