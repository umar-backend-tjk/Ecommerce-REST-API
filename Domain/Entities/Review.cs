using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    [Range(1, 5)] 
    public int Stars { get; set; }
}