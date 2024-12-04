using Entities;
using Repositories.Interface;

namespace Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context)
    {
        _context = context;
    }

    public Wishlist GetWishlistById(int id)
    {
        return _context.Wishlists.Find(id);
    }

    public IEnumerable<Wishlist> GetAllWishlists()
    {
        return _context.Wishlists.ToList();
    }

    public void AddWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        _context.SaveChanges();
    }

    public void AddWishlists(IEnumerable<Wishlist> wishlists)
    {
        _context.Wishlists.AddRange(wishlists);
        _context.SaveChanges();
    }

    public void UpdateWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Update(wishlist);
        _context.SaveChanges();
    }
}