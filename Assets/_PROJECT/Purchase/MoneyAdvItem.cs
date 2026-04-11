using MirraSDK_M;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoneyAdvItem : MonoBehaviour {
    [SerializeField] private Button _getButton;
    [SerializeField] private TextMeshProUGUI _getButtonText;
    [SerializeField] private int _money;
    
    [Inject] private PlayerBank _bank;
    [Inject] private LocalizationData _localization;
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetization;

    private void OnEnable() {
        _getButton.onClick.AddListener(WatchAdv);
    }

    private void Start() {
        _getButtonText.text = _localization.GetButton;
    }

    private void WatchAdv() {
        _advertisingMonetization.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    AddMoney();
                }
            }
        );
    }

    private void AddMoney() {
        _bank.AddMoney(_money);
    }
    
    
}