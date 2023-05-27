namespace Peticom.Core.Entities;

public class Ad : BaseEntity
{
    public string UserId { get; set; }
    public string Slogan { get; set; }
    public string About { get; set; }
    public double Price { get; set; }

    public ICollection<Star> Stars { get; set; }
}