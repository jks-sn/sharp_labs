using Entities;
using Repositories;

namespace HRDirectorService;

public class HRDirectorService
{
    private List<Participant> _participants;
    
    private List<Team> _teams;
    
    private List<Wishlist> _allWishlists;
    public bool DataReceived => _teams != null && _allWishlists != null && _participants != null;
    
    public void SetParticipants(List<Participant> participants)
    {
        _participants = participants;
    }

    public void SetTeams(List<Team> teams)
    {
        _teams = teams;
    }
    
    public void SetWishlists(List<Wishlist> wishlists)
    {
        _allWishlists = wishlists;
    }

    public double CalculateMeanSatisfactionIndex()
    {
        // Собираем все участников
        var participants = _teams.SelectMany(t => new[] { t.TeamLead, t.Junior }).ToList();

        // Словарь предпочтений
        var wishlistDict = _allWishlists.ToDictionary(w => w.ParticipantId, w => w);

        double totalSatisfaction = 0;
        int totalParticipants = participants.Count;

        foreach (var participant in participants)
        {
            if (wishlistDict.TryGetValue(participant.Id, out var wishlist))
            {
                // Находим напарника
                var teammate = _teams.FirstOrDefault(t => t.TeamLead.Id == participant.Id)?.Junior
                               ?? _teams.FirstOrDefault(t => t.Junior.Id == participant.Id)?.TeamLead;

                if (teammate != null)
                {
                    // Определяем индекс удовлетворённости
                    var preferenceList = wishlist.DesiredParticipants.ToList();
                    int rank = preferenceList.IndexOf(teammate.Id);
                    int satisfaction = 0;

                    if (rank >= 0 && rank < 5)
                    {
                        satisfaction = 5 - rank;
                    }

                    totalSatisfaction += satisfaction;
                }
            }
        }
        
        return totalSatisfaction / totalParticipants;
    }

    public async Task SaveResultsAsync(AppDbContext dbContext)
    {
        var hackathon = new Hackathon
        {
            MeanSatisfactionIndex = CalculateMeanSatisfactionIndex()
        };

        // Добавляем участников в хакатон
        foreach (var participant in _participants)
        {
            participant.Hackathons.Add(hackathon);
            hackathon.Participants.Add(participant);
        }

        // Устанавливаем связи для списков предпочтений
        foreach (var wishlist in _allWishlists)
        {
            wishlist.Hackathon = hackathon;
            hackathon.Wishlists.Add(wishlist);
        }

        // Устанавливаем связи для команд
        foreach (var team in _teams)
        {
            team.Hackathon = hackathon;
            team.Participants.Add(team.TeamLead);
            team.Participants.Add(team.Junior);
            hackathon.Teams.Add(team);
            
            team.TeamLead.Teams.Add(team);
            team.Junior.Teams.Add(team);
        }

        dbContext.Hackathons.Add(hackathon);
        await dbContext.SaveChangesAsync();
    }
}