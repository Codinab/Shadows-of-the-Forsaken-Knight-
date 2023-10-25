using System.Collections;

namespace Interfaces
{
    public interface IAttacks
    { 
        bool CanAttack();
        IEnumerator Attack();
    }
}