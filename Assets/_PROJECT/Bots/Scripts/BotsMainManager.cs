using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

public class BotsMainManager : IInitializable, IDisposable {
    private readonly List<BotStateManager> _bots;
    private readonly List<SkinItemConfig> _skins;
    private readonly PlayerStateManager _playerStateManager;
    private readonly GameData _gameData;

    
    private CancellationTokenSource _tokenSource;
    private bool _stopBotSpeaking;
    private List<BotStateManager> _speakingBots = new();

    public BotsMainManager(List<BotStateManager> bots, 
        PlayerStateManager playerStateManager, 
        GameData gameData, 
        List<SkinItemConfig> skins) {
        _bots = bots;
        _skins = skins;
        _gameData = gameData;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerOnChangeState;
        // Debug.Log("Bot count: " + _bots.Count);
    }

    
    public void Initialize() {
        _tokenSource = new CancellationTokenSource();
        BotSpeakCycleAsync(_tokenSource.Token).Forget();
        foreach (var bot in _bots) {
            bot.SetBotSkin(_skins.GetRandomElement());
            bot.InitAnimator();
        }
    }
    
    public BotStateManager GetRandomBot() {
        // Сбросить всех
        _bots.ForEach(bot => bot.SetBotPlayStatus(false));
        // Выбор нового
        var bot = _bots.GetRandomElement();
        bot.SetBotPlayStatus(true);

        return bot;
    }

    private void PlayerOnChangeState(PlayerState state){
        
    }
    
    
    private async UniTask BotSpeakCycleAsync(CancellationToken token) {
        await UniTask.Delay(1000, cancellationToken: token);
        while (!_stopBotSpeaking) {
            float timeToSpeak = Random.Range(_gameData.TimeToSpeak.From,  _gameData.TimeToSpeak.To);
            // Debug.Log("Speaking time" + timeToSpeak);
            await BotSpeakTimerAsync(timeToSpeak, token);
        }
    } 
    
    private async UniTask BotSpeakTimerAsync(float time, CancellationToken token) {
        SetBotsSpeak();
        float elpsedTime = 0;
        while (elpsedTime < time) {
            elpsedTime += Time.deltaTime;
            await  UniTask.Yield(token);
        }
        SetBotsStfu();
    }

    private void SetBotsSpeak() {
        int countSpeakBots = GetCountSpeakingBots();
        // Debug.Log("Говорящих ботов: " + countSpeakBots);
        List<int> speakingBotsNumbers = GetNewSpeakingBotsNumbers(countSpeakBots);
        foreach (var bot in speakingBotsNumbers) {
            _speakingBots.Add(_bots[bot]);
            _bots[bot].SetBotSpeak();
        }
    }
    
    private void SetBotsStfu() {
        foreach (var bot in _speakingBots) {
            bot.SetBotStfu();
        }
        _speakingBots.Clear();
    }

    private int GetCountSpeakingBots() {
        int from = Mathf.Clamp(_gameData.CountSpeakingBotsPerTime.From, 0, _bots.Count);
        int to = Mathf.Clamp(_gameData.CountSpeakingBotsPerTime.To, 0, _bots.Count);
        
        return Random.Range(from, to);
    }
    
    private List<int> GetNewSpeakingBotsNumbers(int count)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < _bots.Count; i++) {
            if (_bots[i].IsPlaying == false) {
                available.Add(i);
            }
        }

        List<int> result = new List<int>();

        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++) {
            int r = Random.Range(0, available.Count);
            result.Add(available[r]);
            available.RemoveAt(r);
        }

        return result;
    }
    
    



    public void Dispose() {
        _playerStateManager.ChangeState -= PlayerOnChangeState;
    }
}
