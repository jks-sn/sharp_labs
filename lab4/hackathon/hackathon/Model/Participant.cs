// Model/Participant.cs

using System;
using System.Collections.Generic;

namespace Hackathon.Model;
public abstract class Participant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Preference> Preferences { get; set; } = new();
    public string? AssignedPartner { get; set; }
    public int SatisfactionIndex { get; set; }
    
    public int HackathonEventId { get; set; }
    public HackathonEvent HackathonEvent { get; set; }

    public void CalculateSatisfactionIndex()
    {
        var assignedPreference = Preferences.FirstOrDefault(p => p.PreferredName == AssignedPartner);
        if (assignedPreference != null)
        {
            SatisfactionIndex = 20 - assignedPreference.Rank;
        }
        else 
        {
            throw new InvalidOperationException($"Assigned partner {AssignedPartner} not found in the wishlist.");
        }
    }
    
    public List<string> GetPreferredNames()
    {
        return Preferences.OrderBy(p => p.Rank).Select(p => p.PreferredName).ToList();
    }
}