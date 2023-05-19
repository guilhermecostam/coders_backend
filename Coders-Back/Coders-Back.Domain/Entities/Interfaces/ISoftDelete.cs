namespace Coders_Back.Domain.Entities.Interfaces;

public interface ISoftDelete        
{
    public bool IsDeleted { get; set; }
}