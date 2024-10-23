//Builder/JuniorBuilder.cs

using Hackathon.Model;

namespace Hackathon.Tests.Builder;

public class JuniorBuilder
{
    private string _name = "Junior";
    private List<string> _wishList = new List<string>();

    public JuniorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public JuniorBuilder WithWishList(List<string> wishList)
    {
        _wishList = wishList;
        return this;
    }

    public Junior Build()
    {
        return new Junior
        {
            Name = _name,
            WishList = _wishList
        };
    }
}