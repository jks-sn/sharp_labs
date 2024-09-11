using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Junior(string name)
    {
        public string Name { get; set; } = name;
        public List<string> Wishlist { get; set; } = [];

        public void GenerateRandomWishlist(List<string> teamLeads)
        {
            var random = new Random();
            Wishlist = teamLeads.OrderBy(x => random.Next()).ToList();
        }
    }
}
