using RoguelikeGame.Items;

namespace RoguelikeGame.Interaction
{
    public interface IInteractable
    {
        bool CanPickup(ICarryable item);
        bool CanPlace(ICarryable item);
        void Place(ICarryable item);
        ICarryable Take();
    }
}