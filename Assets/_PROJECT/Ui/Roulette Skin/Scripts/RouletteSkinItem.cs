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

    [field: Header("Debug")]
    [field: SerializeField] public Vector3 InitialPosition { get; private set; }

    public void SetInfo(InfoThrowableObject infoThrowableObject)
    {
        _icon.sprite = infoThrowableObject.Icon;
        _damageText.text = infoThrowableObject.Damage.ToString();
    }

    public void StorePosition()
    {
        InitialPosition = Rect.anchoredPosition;
    }

    public void ResetPosition() 
    {
        Rect.anchoredPosition = InitialPosition;
    }
}
