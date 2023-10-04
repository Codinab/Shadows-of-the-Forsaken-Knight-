namespace Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }

        void TakeDamage(int amount);
        void Heal(int amount);
        bool IsDead { get; }
    }
}