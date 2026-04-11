using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class ShopMoneyIniter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI[] _moneyTexts;
    
    [Inject] private LocalizationData _localization;
    
    private void Start() {
        _moneyTexts.ForEach(m => m.text = _localization.Money);
    }
}