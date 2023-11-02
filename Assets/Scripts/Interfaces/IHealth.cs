namespace Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }

        public void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            DamagedAnimation();
        }

        void DamagedAnimation();

        public void Heal(int amount)
        {
            CurrentHealth += amount;
        }

        public bool IsDead() => CurrentHealth <= 0;
    }
}