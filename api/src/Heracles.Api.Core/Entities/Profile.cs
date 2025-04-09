using System.ComponentModel.DataAnnotations;

namespace Heracles.Core.Entities;

public class Profile
{
    public Profile() { }

    public Profile(Guid userId)
    {
        UserId = userId;
    }

    [Key]
    public Guid UserId { get; set; }
}
