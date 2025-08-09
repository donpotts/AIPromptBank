using System.ComponentModel.DataAnnotations;

namespace MyAIPrompt.Shared.Models;

public class ApplicationUserWithRolesDto : ApplicationUserDto
{
    public List<string>? Roles { get; set; }
}
