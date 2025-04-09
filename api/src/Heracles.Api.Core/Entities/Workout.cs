namespace Heracles.Core.Entities;

public class Workout : EntityBase
{
    public Guid UserId { get; set; }

    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;
}
