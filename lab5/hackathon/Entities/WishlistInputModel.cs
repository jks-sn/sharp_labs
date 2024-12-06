using System.ComponentModel.DataAnnotations;

public class WishlistInputModel
{
    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public string ParticipantTitle { get; set; } // Строковое представление, будет конвертировано в Enum

    [Required]
    public List<int> DesiredParticipants { get; set; } = new List<int>();
}