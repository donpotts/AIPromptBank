using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MyAIPrompt.Shared.Models;

[DataContract]
public class AIPrompt
{
    [Key]
    [DataMember]
    public Guid? Id { get; set; }

    [DataMember]
    public string? Title { get; set; }

    [DataMember]
    public string? Content { get; set; }

    [DataMember]
    public DateTimeOffset? CreateDate { get; set; }

    [DataMember]
    public DateTimeOffset? ModifiedDate { get; set; }

    [DataMember]
    public bool IsSynced { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember]
    public string? Hash { get; set; } 

    [DataMember]
    public List<AITag>? AITag { get; set; }

    [DataMember]
    public List<AISystemPrompt>? AISystemPrompt { get; set; }
}
