using System.ComponentModel.DataAnnotations;

namespace OrderApi.Data.Models;

public class User
{
    public Guid Id { get; set; }

    [Required] [MaxLength(200)] 
    public string Name { get; set; }

    public ICollection<Order> Orders { get; set; }
}