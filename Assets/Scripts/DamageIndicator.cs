using UnityEngine;
using UnityEditor;
using TMPro;
using DG.Tweening;

#if UNITY_EDITOR
using DG.DOTweenEditor;
#endif

public class DamageIndicator : MonoBehaviour {

    public TMP_Text damageText;
    public CanvasGroup canvasGroup;

    public void IndicateDamage(int damage) {
        damageText.text = $"{damage}";
        canvasGroup.alpha = 0;
        canvasGroup.transform.localPosition = Vector3.zero;
        canvasGroup.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(canvasGroup.DOFade(1f, 0.05f).SetEase(Ease.InOutCubic))
            .Join(canvasGroup.transform.DOLocalMoveY(1.5f, 0.05f)).SetEase(Ease.InOutCubic)
            .Join(canvasGroup.transform.DOScale(0.15f, 0.05f)).SetEase(Ease.InOutCubic)
            .Append(canvasGroup.transform.DOScale(0.1f, 0.05f)).SetEase(Ease.InOutCubic)
            .AppendInterval(2)
            .Append(canvasGroup.DOFade(0f, 0.05f))
            .OnComplete(() => {
                canvasGroup.alpha = 0;
                canvasGroup.transform.localPosition = Vector3.zero;
                canvasGroup.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

#if UNITY_EDITOR
                if (!EditorApplication.isPlaying) {
                    return;
                }
#endif

                Destroy(this.gameObject);
            });

#if UNITY_EDITOR
        if (EditorApplication.isPlaying) {
            sequence.Play();
        } else {
            DOTweenEditorPreview.PrepareTweenForPreview(sequence, false);
            DOTweenEditorPreview.Start();
        }
#endif
    }
}