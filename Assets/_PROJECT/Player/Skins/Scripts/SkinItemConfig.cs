using UnityEngine;

public enum SkinGetType {
    Purchase,
    Adv
}


[CreateAssetMenu(fileName = "SkinItem", menuName = "Configs/SkinItem")]
public class SkinItemConfig : ScriptableObject {
    [field: SerializeField] public string Id { get; private set; } 
    [field: SerializeField] public long Price { get; private set; } 
    [field: SerializeField] public GameObject SkinPrefab { get; private set; }
    [field: SerializeField] public SkinElementsController SkinElementsController { get; private set; }
    [field: SerializeField] public Avatar Avatar { get; private set; } 
    [field: SerializeField] public SkinGetType SkinGetType { get; private set; } 

    
    public bool IsAdv => SkinGetType == SkinGetType.Adv;
    
}
