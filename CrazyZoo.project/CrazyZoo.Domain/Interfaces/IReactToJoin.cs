using CrazyZoo.Domain.Models;

namespace CrazyZoo.Domain.Interfaces
{
    public interface IReactToJoin
    {
        void OnAnimalJoined(object sender, Animal joined);
    }
}
