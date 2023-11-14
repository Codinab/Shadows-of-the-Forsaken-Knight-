namespace Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }

        public void TakeDamage(int amount)
        {
            if (IsDead()) return;
            CurrentHealth -= amount;
            Damaged();
        }

        void Damaged();

        public void Heal(int amount)
        {
            CurrentHealth += amount;
        }

        public bool IsDead() => CurrentHealth <= 0;
    }
}