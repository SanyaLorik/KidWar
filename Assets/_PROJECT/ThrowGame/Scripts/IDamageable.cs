public interface IDamageable {
    public int MaxHp { get; }
    public int CurrentHp { get; }
    public bool IsInvinsible { get; }
    public bool IsShielded { get; }
    public void AddDamage(int hp);
    public void AddHp(int hp);
    public void SetInvinsible(bool state);
    public void EnableShield(int hp);
    public void SetMaxHp();
}