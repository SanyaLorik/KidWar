using UnityEngine;

[CreateAssetMenu(fileName = "BonusItemSO", menuName = "Configs/BonusItemSO")]
public class BonusItemSO : ScriptableObject {
    [field: SerializeField] public string Id { get; private set; }
    [SerializeReference, SubclassSelector] public IBonus Bonus;
    [field: SerializeField] public int Price { get; private set; }
    
}