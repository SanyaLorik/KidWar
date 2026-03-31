using UnityEngine;

[CreateAssetMenu(fileName = "ModifierItemSO", menuName = "Configs/ModifierItemSO")]
public class ModifierItemSO : ScriptableObject {
    [field: SerializeField] public string Id { get; private set; }
    [SerializeReference, SubclassSelector] public IThrowableModifier Modifier;
}