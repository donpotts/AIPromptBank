using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MyAIPrompt.Shared.Models;

[DataContract]
public class AITag
{
    [Key]
    [DataMember]
    public Guid? Id { get; set; }

    [DataMember]
    public string? Name { get; set; }

    [DataMember]
    public string? Description { get; set; }

    [DataMember]
    public string? Color { get; set; }

    [DataMember]
    public DateTimeOffset? CreateDate { get; set; }

    [DataMember]
    public DateTimeOffset? ModifiedDate { get; set; }

    [DataMember]
    public List<AIPrompt>? AIPrompt { get; set; }
}
