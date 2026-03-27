public interface IDamageable {
    public int MaxHp { get; }
    public int CurrentHp { get; }
    public bool IsInvinsible { get; }
    public void TakeDamage(int hp);
    public void AddHp(int hp);
    public void SetInvinsible(bool state);
    public void SetMaxHp();
}