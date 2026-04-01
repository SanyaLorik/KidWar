using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MapChanger : MonoBehaviour {
    [SerializeField] private List<GameObject> _maps;
    [SerializeField] private float _timeToChangeMaps;
    [SerializeField] private bool _changeRandomMap;
    
    
    
    private CancellationTokenSource _tokenSource;
    private int _currentMapIndex;
    
    [Inject] ThrowGameStarter _gameStarter;
    
    
    private void Start() {
        UpdateIndex();
        SetChoosedMap();
    }


    private void ChooseNextMap() {
        UpdateIndex();
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        TimerToChangeMapAsync(_tokenSource.Token).Forget();
    }

    private void UpdateIndex() {
        if (_changeRandomMap) {
            int secondIndex =  Random.Range(0, _maps.Count-1);
            _currentMapIndex = secondIndex >= _currentMapIndex ? ++secondIndex : secondIndex;
        }
        else {
            _currentMapIndex++;
            if (_currentMapIndex >= _maps.Count) {
                _currentMapIndex = 0;
            }
        }
    }

    
    private async UniTask TimerToChangeMapAsync(CancellationToken token) {
        await UniTask.WaitForSeconds(_timeToChangeMaps, cancellationToken: token);
        await UniTask.WaitWhile(() => _gameStarter.GameIsStarted, cancellationToken: token);
        SetChoosedMap();
    } 
    
    private void SetChoosedMap() {
        Debug.Log("Установка карты" + _maps[_currentMapIndex]);
        var map = _maps.Find(map => map.activeSelf);
        map.DisactiveSelf();
        _maps[_currentMapIndex].ActiveSelf();
        // Запускаем сразу выбор индекса и таску
        ChooseNextMap();
    }

    
}
