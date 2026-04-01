using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HpView : MonoBehaviour {
    [SerializeField] private RectTransform _leftHp;
    [SerializeField] private RectTransform _parentLeftHp;
    [SerializeField] private RectTransform _rightHp;
    [SerializeField] private RectTransform _parentRightHp;
    [SerializeField] private float _changeHpDuration = 1f;
    
    
    [Inject] private ThrowGameStarter _throwGameStarter;
    [Inject] private BattleManager _battleManager;
    [Inject] private GameData _gameData;
    
    
    private int MaxHp => _gameData.PlayerMaxHp;


    private void OnEnable() {
        _throwGameStarter.GameStarted += OnGameStarted;
    }

    private void OnGameStarted(bool started) {
        if(!_battleManager.MainPlayerPlay) return;
    }


    public void ChangeHp(float hp, bool stayInLeft) {
        float percent = (float) hp / MaxHp;
        if (stayInLeft) {
            // left
            ChangeLeftPlayerHp(percent);
        }
        else {
            // right
            ChangeRightPlayerHp(percent);
        }
    }

    private void ChangeLeftPlayerHp(float percent) {
        Debug.Log("ChangeLeftPlayerHp " + percent);
        SetFillAmountInRight(_leftHp, _parentLeftHp, percent);
    }
    
    private void ChangeRightPlayerHp(float percent) {
        Debug.Log("ChangeRightPlayerHp" + percent);
        SetFillAmountInLeft(_rightHp, _parentRightHp, percent);
    }
    
    // Перенести в RectTransformHelper, SetFillAmount стал SetFillAmountInLeft



    private CancellationTokenSource _tokenSource;
    
    
    /// <summary>
    /// В левую сторону убавляется полоска от 100 процентов
    /// </summary>
    /// <param name="img">Полоска хп</param>
    /// <param name="parent">Родитель полоски хп</param>
    /// <param name="percent">Процент заполнения</param>
    private void SetFillAmountInLeft(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var xPose = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
                
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        
        ChangeHpTask(img, img.offsetMax, xPose, true, _tokenSource.Token).Forget();
        // img.offsetMax = xPose;
    }
    
    
    /// <summary>
    /// В правую сторону убавляется полоска от 100 процентов
    /// </summary>
    /// <param name="img">Полоска хп</param>
    /// <param name="parent">Родитель полоски хп</param>
    /// <param name="percent">Процент заполнения</param>
    private void SetFillAmountInRight(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var xPose = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        
        ChangeHpTask(img, img.offsetMin, -xPose, false, _tokenSource.Token).Forget();
        // img.offsetMin = -xPose;
    }
    
    private async UniTask ChangeHpTask(RectTransform img, Vector2 currentPos, Vector2 targetPos, bool offsetMax, CancellationToken token) {
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested && elapsedTime < _changeHpDuration) {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _changeHpDuration;
            if (offsetMax) {
                img.offsetMax = Vector2.Lerp(currentPos, targetPos, progress);
            }
            else {
                img.offsetMin = Vector2.Lerp(currentPos, targetPos, progress);
            }
            await UniTask.Yield();
        }
        if (offsetMax) {
            img.offsetMax = targetPos;
        }
        else {
            img.offsetMin = targetPos;
        }
    }
    
    
    /// <summary>
    /// Получить процент заполнения в зависимости от процента
    /// В SetFillAmountInRight берем с минусом
    /// </summary>
    /// <param name="percent">Процент</param>
    /// <param name="xEnd">Ширина родителя</param>
    /// <param name="parent">Родитель</param>
    /// <returns></returns>
    private float GetXPoseByPercent(float percent, float xEnd, RectTransform parent)
    {
        if (xEnd < 0)
        {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }

    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}
