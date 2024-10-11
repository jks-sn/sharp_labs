// Participant.cs
using Hackathon.Model;

namespace Hackathon.Model
{
    public abstract class Participant
    {
        public string Name { get; set; }
        public List<string> WishList { get; set; }
        public string AssignedPartner { get; set; }
        public int SatisfactionIndex { get; set; }

        public void CalculateSatisfactionIndex()
        {
            int position = WishList.IndexOf(AssignedPartner);
            if (position >= 0)
            {
                SatisfactionIndex = 20 - position;
            }
            else 
            {
                Console.WriteLine($"Ошибка: Assigned partner {AssignedPartner} не найден в списке предпочтений участника {Name}.");
                Console.WriteLine($"Список предпочтений: {string.Join(", ", WishList)}");

                throw new InvalidOperationException($"Assigned partner {AssignedPartner} not found in the wishlist.");
            }
        }
    }
}