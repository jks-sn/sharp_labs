//Tests/Builder/JuniorBuilder.cs

using Hackathon.Model;

namespace Hackathon.Tests.Builder;

public class JuniorBuilder
{
    private string _name = "Junior";
    private List<string> _wishList = new List<string>();
    private int _satisfactionIndex = 0;
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
    
    public JuniorBuilder WithSatisfactionIndex(int index)
    {
        _satisfactionIndex = index;
        return this;
    }
    public Junior Build()
    {
        return new Junior
        {
            Name = _name,
            WishList = _wishList,
            SatisfactionIndex = _satisfactionIndex
        };
    }
}