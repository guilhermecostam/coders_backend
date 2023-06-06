namespace Coders_Back.Domain.DataAbstractions;

public interface ISoftDelete        
{
    public bool IsDeleted { get; set; }
}