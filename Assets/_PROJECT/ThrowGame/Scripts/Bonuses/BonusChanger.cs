using Architecture_M;
using TMPro;
using UnityEngine;
using Zenject;

public class BonusChanger : UsableItemBase {
    [field: SerializeField] public BonusItemSO BonusItem { get; private set; }
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private TextMeshProUGUI _bonusNameText;
    
    private int _bonusCounts;
    public IBonus Bonus => BonusItem.Bonus;
    public int BonusCount => _bonusCounts;
    
    [Inject] private BonusManager _bonusManager;
    [Inject] private DiContainer _diContainer;
    [Inject] private IGameSave _saver; 
    [Inject] LocalizationData _localization;
    
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(Bonus);
    }
    
    
    private void Start() {
        ChangeVisualCount();
        _bonusNameText.text =
            _localization.GetTranslatedText(BonusItem.Id, _localization.BonusesTranslates);
    }


    public override void TryUse() {
        if (!IsAvailable) {
            Debug.Log("Бонус на перезарядке именно что");
            return;
        }

        if (_bonusCounts == 0) {
            Debug.Log("Бонусов нема");
            return;
        }
        _bonusManager.UseBonusByClick(BonusItem.Bonus, this);
    }

    public void CheckAvailable() {
        if (_bonusCounts == 0) {
            SetUnvailable();
        }
    }

    
    public void AddBonusCount(int newBonusCount) {
        _bonusCounts += newBonusCount;
        ChangeVisualCount();
    }

    public void SetBonusCount(int newBonusCount) {
        _bonusCounts = newBonusCount;
        ChangeVisualCount();
    }

    
    public void GetOneBonus(bool useSaves = false) {
        if (_bonusCounts != 0) {
            _bonusCounts--;
            if (useSaves) {
                _saver.GetSave<GameSave>().SetMinusOneBonus(BonusItem.Id);
                _saver.Save();
            }
        }
        ChangeVisualCount();
    }

    
    private void ChangeVisualCount() {
        _countText.text = _bonusCounts.ToString();
        CheckAvailable();
    }
    
}