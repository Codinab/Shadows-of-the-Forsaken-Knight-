using System.Collections;

namespace Interfaces
{
    public interface IAttacks
    {
        int Damage { get; }
        float AttackSpeed { get; }
        int PushPower { get; }
        float AttackRange { get; }
        
        bool CanAttack();
        IEnumerator Attack();
    }
}