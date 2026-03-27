using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class HpView : MonoBehaviour {
    [SerializeField] private RectTransform _leftHp;
    [SerializeField] private RectTransform _parentLeftHp;
    [SerializeField] private RectTransform _rightHp;
    [SerializeField] private RectTransform _parentRightHp;
    
    
    [Inject] private BattleManager _battleManager;
    [Inject] private GameData _gameData;
    private int MaxHp => _gameData.PlayerMaxHp;
    
    public void ChangeHp(int hp, bool stayInLeft) {
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
        // Чем больше left тем меньше hp
        // 0 left - 100hp
        // _parentLeftHp.width - 0hp
        SetFillAmountInRight(_leftHp, _parentLeftHp, percent);
    }
    
    private void ChangeRightPlayerHp(float percent) {
        Debug.Log("ChangeRightPlayerHp" + + percent);
        // Чем больше left тем меньше hp
        // 0 right - 100hp
        // _parentRightHp.width - 0hp
        SetFillAmountInLeft(_rightHp, _parentRightHp, percent);
    }
    
    // Перенести в RectTransformHelper, SetFillAmount стал SetFillAmountInLeft
    
    /// <summary>
    /// В левую сторону убавляется полоска от 100 процентов
    /// </summary>
    /// <param name="img">Полоска хп</param>
    /// <param name="parent">Родитель полоски хп</param>
    /// <param name="percent">Процент заполнения</param>
    public static void SetFillAmountInLeft(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var xPose = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        img.offsetMax = xPose;
    }
    
    /// <summary>
    /// В правую сторону убавляется полоска от 100 процентов
    /// </summary>
    /// <param name="img">Полоска хп</param>
    /// <param name="parent">Родитель полоски хп</param>
    /// <param name="percent">Процент заполнения</param>
    public static void SetFillAmountInRight(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var xPose = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        img.offsetMin = -xPose;
    }
    
    /// <summary>
    /// Получить процент заполнения в зависимости от процента
    /// В SetFillAmountInRight
    /// </summary>
    /// <param name="percent">Процент</param>
    /// <param name="xEnd">Ширина родителя</param>
    /// <param name="parent">Родитель</param>
    /// <returns></returns>
    private static float GetXPoseByPercent(float percent, float xEnd, RectTransform parent)
    {
        if (xEnd < 0)
        {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }
}
