using Entities;

namespace Repositories.Interface;

public interface IWishlistRepository
{
    Wishlist GetWishlistById(int id);
    IEnumerable<Wishlist> GetAllWishlists();
    void AddWishlist(Wishlist wishlist);
    void AddWishlists(IEnumerable<Wishlist> wishlists);
    void UpdateWishlist(Wishlist wishlist);
}