using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;

public class ShieldAnimationLikeBubble : MonoBehaviour {
    [SerializeField] private PairedValue<float> _shieldShowDurations;
    [SerializeField] private PairedValue<Ease> _shieldShowEase;
    
    
    public void ShieldEnableAnimate(bool enable, Transform shield) {
        if (enable) {
            ShowShieldAnimation(shield);
        }
        else {
            HideShieldAnimation(shield);
        }
    }

    private void ShowShieldAnimation(Transform shield) {
        shield.gameObject.ActiveSelf();
        shield.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.Append(shield
            .DOScale(1f, _shieldShowDurations.From)
            .SetEase(_shieldShowEase.From)
        );
    }
    
    private void HideShieldAnimation(Transform shield) {
        Sequence seq = DOTween.Sequence();
        shield.localScale = Vector3.one;
        seq.Append(shield
            .DOScale(0f, _shieldShowDurations.To)
            .SetEase(_shieldShowEase.To)
            .OnComplete(() => shield.gameObject.DisactiveSelf())
        );
    }
}