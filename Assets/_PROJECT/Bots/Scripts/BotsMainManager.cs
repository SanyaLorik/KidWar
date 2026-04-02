using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

public class BotsMainManager : IInitializable, IDisposable {
    private readonly List<BotStateManager> _bots;
    private readonly List<SkinItemConfig> _skins;
    private readonly GameData _gameData;
    private readonly PlayerCopyBotController _playerCopyBot;

    
    private CancellationTokenSource _tokenSource;
    private bool _stopBotSpeaking;
    private List<BotStateManager> _speakingBots = new();

    public BotsMainManager(List<BotStateManager> bots, 
        GameData gameData, 
        List<SkinItemConfig> skins,
        PlayerCopyBotController playerCopyBot) 
    {
        _bots = bots;
        _skins = skins;
        _gameData = gameData;
        _playerCopyBot = playerCopyBot;
    }

    
    public void Initialize() {
        _tokenSource = new CancellationTokenSource();
        BotSpeakCycleAsync(_tokenSource.Token).Forget();
        foreach (var bot in _bots) {
            bot.SetBotSkin(_skins.GetRandomElement());
            bot.InitAnimator();
            // Установка ников
        }
    }
    
    public void SetBotSkin(BotStateManager bot, string id) {
        SkinItemConfig skin = _skins.Find(s => s.Id == id);
        bot.SetBotSkin(skin);
        bot.InitAnimator();
    }
    
    public BotStateManager GetRandomBotToBattle(bool playerCopy) {
        if (playerCopy) {
            return _playerCopyBot.GetBotToBattle();
        }
        
        int countIters = 500;
        while (countIters > 0) {
            countIters--;
            var bot = _bots.GetRandomElement();
            if (!bot.IsPlaying && !bot.IsPlayerCopy) {
                bot.SetPlayStatusSilent(true);
                bot.ObjectThrower.SetNickname(bot.Nickname);
                return bot;
            }
        }
        Debug.LogError("Все боты для игры заняты, бот не нашелся");
        return null;
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
    }
}
