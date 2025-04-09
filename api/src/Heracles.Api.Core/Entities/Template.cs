using System.ComponentModel.DataAnnotations;

namespace Heracles.Core.Entities;

public class Template : PublicEntity
{
    public const int NameLength = 100;
    public const int DescriptionLength = 250;

    public Template() { }

    public Template(string name)
    {
        Name = name;
    }

    [MaxLength(NameLength)]
    public string Name { get; set; } = null!;

    [MaxLength(DescriptionLength)]
    public string? Description { get; set; }
}
