using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;

public class ShieldVisual : MonoBehaviour {
    [SerializeField] private Transform _shield;
    
    [Header("Анимация щита")]
    [SerializeField] private PairedValue<float> _shieldShowDurations;
    [SerializeField] private PairedValue<Ease> _shieldShowEase;
    
    [Header("Бар")]
    [SerializeField] private RectTransform _bar;
    [SerializeField] private RectTransform _barParent;
    [SerializeField] private float _changeBarDuration = 1f;
    [SerializeField] private TextMeshProUGUI _shieldHp;

    private CancellationTokenSource _tokenSource;
    
    public void SetShieldPercentage(float percentage, int hp) {
        percentage = Mathf.Clamp01(percentage);
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ChangeShieldPercentageAsync(percentage, _tokenSource.Token).Forget();
        _shieldHp.text = hp.ToString();
    }
    
    private async UniTask ChangeShieldPercentageAsync(float percentage, CancellationToken token) {
        float elapsedTime = 0f;

        Vector2 initPos = _bar.offsetMax;
        Vector2 targetPos = new Vector2(GetXPoseByPercent(percentage, _barParent), 0);
        
        // Debug.Log("percentage = " + percentage);
        // Debug.Log("initPos = " + initPos);
        // Debug.Log("targetPos = " + targetPos);
        
        
        while (!token.IsCancellationRequested && elapsedTime < _changeBarDuration) {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _changeBarDuration;
            Vector2 interp = Vector2.Lerp(initPos, targetPos, progress);
            _bar.offsetMax = interp;
            // Debug.Log("interp = " + interp);
            await UniTask.Yield();
        }
        _bar.offsetMax = targetPos;
        if (percentage == 0f) {
            HideShieldAnimation();
        }
    }
    
    
    public void ShieldEnableAnimate(bool enable, int hp) {
        if (enable) {
            ShowShieldAnimation(hp);
        }
        else {
            HideShieldAnimation();
        }
    }

    private void ShowShieldAnimation(int hp) {
        SetShieldPercentage(1f, hp);
        _shield.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.Append(_shield
            .DOScale(1f, _shieldShowDurations.From)
            .SetEase(_shieldShowEase.From)
        );
    }
    
    private void HideShieldAnimation() {
        Sequence seq = DOTween.Sequence();
        _shield.localScale = Vector3.one;
        seq.Append(_shield
            .DOScale(0f, _shieldShowDurations.To)
            .SetEase(_shieldShowEase.To)
            .OnComplete(gameObject.DisactiveSelf)
        );
    }
    
    /// <summary>
    /// есть в RectTransformHelper просто приватное, саня верни доступ(((
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="xEnd"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private static float GetXPoseByPercent(float percent, RectTransform parent)
    {
        float xEnd =  parent.rect.width;
        if (xEnd < 0)
        {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }

    private void OnDisable() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}