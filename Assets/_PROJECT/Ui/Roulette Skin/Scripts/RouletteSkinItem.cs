using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteSkinItem : MonoBehaviour
{
    [field: Header("Logic")]
    [field: SerializeField] public RectTransform Rect { get; private set; }

    [field: Header("View")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _damageText;

    public void SetInfo(InfoThrowableObject infoThrowableObject)
    {
        _icon.sprite = infoThrowableObject.Icon;
        _damageText.text = infoThrowableObject.Damage.ToString();
    }
}
