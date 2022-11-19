using DG.Tweening;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    [SerializeField] private float duration;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        _canvasGroup.DOFade(1, duration);

        _canvasGroup.blocksRaycasts = true;
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(0, duration);

        _canvasGroup.blocksRaycasts = false;
    }
}
