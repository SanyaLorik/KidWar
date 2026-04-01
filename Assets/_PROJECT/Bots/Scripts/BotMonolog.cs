using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotMonolog : MonoBehaviour {
    [SerializeField] private GameObject _monologCloud;
    [SerializeField] private TMP_Text _monologText;
    [SerializeField] private TMP_Text _botNicknameText;


    public string NickName { get; private set; }


    [Inject] private LocalizationData _localization; 
    [Inject] private NicknameRandomizer _nicknameRandomizer; 
    
    private void Start() {
        Stfu();
        ChangeNickname();
    }

    public void ChangeNickname() {
        NickName = _nicknameRandomizer.GetRandomName();
        _botNicknameText.text = NickName;
    }


    public void SaySomething() {
        _monologCloud.ActiveSelf();
        _monologText.text = _localization.BotsPhrases[Random.Range(0, _localization.BotsPhrases.Length)];
    }

    public void Stfu() {
        _monologCloud.DisactiveSelf();
        _monologText.text = string.Empty;
    }
    
}
