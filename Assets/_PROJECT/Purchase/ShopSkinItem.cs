using MirraSDK_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShopSkinItem : MonoBehaviour  {
    [SerializeField] private SkinItemConfig _skinItemConfig;
    [SerializeField] private Button _getButton;
    [SerializeField] private Button _selectedButton;
    [SerializeField] private Button _canButton;
    [SerializeField] private TextMeshProUGUI _priceText;

    private int _countsToShowAdv = 0;
    private ShopSkinsIniter _initer;
    
    [Inject] PlayerSkinInventory _playerSkinInventory;
    [Inject] PlayerBank _playerBank;
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetization;
    [Inject] private NumberFormatter _formatter;

    
    private void Awake() {
        if (_skinItemConfig.IsAdv) {
            _getButton.onClick.AddListener(WatchAdv);
        }
        else {
            _getButton.onClick.AddListener(BuySkin);
        }
        _canButton.onClick.AddListener(WearSkin);
    }

    public void Initialize(ShopSkinsIniter initer) {
        _initer = initer;
        if (_skinItemConfig.IsAdv) {
            _priceText.text = $"0/{_skinItemConfig.CountAdvToUnlock}";
        }
        else {
            _priceText.text = _formatter.ValuteFormatter(_skinItemConfig.Price);
        }
    }
    
    private void WatchAdv() {
        _advertisingMonetization.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    _countsToShowAdv++;
                    if (_countsToShowAdv == _skinItemConfig.CountAdvToUnlock) {
                        _playerSkinInventory.UnlockSkin(_skinItemConfig);
                        SetCan();
                    }
                    else {
                        _priceText.text = $"{_countsToShowAdv}/{_skinItemConfig.CountAdvToUnlock}";
                    }
                }
            }
        );
    }


    public void CheckStatus() {
        if (_playerSkinInventory.CurrentSkinId == _skinItemConfig.Id) {
            SetSelected();
        }
        else if (_playerSkinInventory.SkinIsBought(_skinItemConfig.Id)) {
            SetCan();
        }
        else {
            SetGet();
        }
    }

        
    private void SetGet() {
        ShowButton(_getButton);
        HideButton(_selectedButton);
        HideButton(_canButton);
    }

    private void SetSelected() {
        HideButton(_getButton);
        ShowButton(_selectedButton);
        HideButton(_canButton);
    }

    private void SetCan() {
        HideButton(_getButton);
        HideButton(_selectedButton);
        ShowButton(_canButton);
    }

    private void HideButton(Button button) {
        if (button.gameObject.activeSelf) {
            button.DisactiveSelf();
        }
    }
    
    private void ShowButton(Button button) {
        if (!button.gameObject.activeSelf) {
            button.ActiveSelf();
        }
    }

    
    private void BuySkin() {
        if (!_playerBank.CanBuy(_skinItemConfig.Price)) return;
        _playerSkinInventory.UnlockSkin(_skinItemConfig);
        _playerBank.SpendMoney(_skinItemConfig.Price);
        SetCan();
    }


    private void WearSkin() {
        _playerSkinInventory.EquipSkin(_skinItemConfig);
        SetSelected();
        _initer.CheckAllStatuses();
    }

    
}