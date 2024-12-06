// Entities/ParticipantInputModel.cs

using System.ComponentModel.DataAnnotations;

namespace Entities;

public class ParticipantInputModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } // Должно соответствовать значениям Enum в виде строки

    [Required]
    public string Name { get; set; }
}