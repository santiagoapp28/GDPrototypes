using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private int originalSiblingIndex;

    public float hoverScale = 1.2f;
    public float liftHeight = 50f;
    public float duration = 0.25f;

    private void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        originalSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();

        transform.DOComplete(); // stop any existing tweens
        transform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutBack);
        transform.DOLocalMoveY(originalPosition.y + liftHeight, duration).SetEase(Ease.OutSine);

        AudioManager.Instance.PlaySFX(Sounds.UIHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOComplete();
        transform.DOScale(originalScale, duration).SetEase(Ease.InOutSine);
        transform.DOLocalMoveY(originalPosition.y, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => transform.SetSiblingIndex(originalSiblingIndex));
    }

}
