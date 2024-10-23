//Builder/TeamLeadBuilder.cs

using Hackathon.Model;

namespace Hackathon.Tests.Builder;

public class TeamLeadBuilder
{
    private string _name = "TeamLead";
    private List<string> _wishList = new List<string>();

    public TeamLeadBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TeamLeadBuilder WithWishList(List<string> wishList)
    {
        _wishList = wishList;
        return this;
    }

    public TeamLead Build()
    {
        return new TeamLead
        {
            Name = _name,
            WishList = _wishList
        };
    }
}