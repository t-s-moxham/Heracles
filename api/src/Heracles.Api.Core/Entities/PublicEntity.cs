namespace Heracles.Core.Entities;

public class PublicEntity : EntityBase
{
    public Guid? UserId { get; set; }
    public bool IsOfficial { get; set; }
}
