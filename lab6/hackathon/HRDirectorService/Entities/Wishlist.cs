//Entities/Wishlist.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRDirectorService.Entities;
using ParticipantTitle = HRDirectorService.Entities.Consts.ParticipantTitle;

namespace Entities;

public class Wishlist
{
    public int Id { get; set; }
    
    public int[] DesiredParticipants { get; set; }
    
    
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }
    
    public Wishlist() {}
}