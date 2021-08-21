using UnityEngine;
using DG.Tweening;

public class TooltipSystem : MonoBehaviour {

    public Tooltip tooltip;
    private static TooltipSystem current;

    public void Awake() {
        current = this;
        Hide();
    }

    public static void Show(string content, string header = "") {
        current.tooltip.canvasGroup.DOFade(1, current.tooltip.transitionDuration);
        current.tooltip.SetText(content, header);
    }

    public static void Hide() {
        current.tooltip.canvasGroup.DOFade(0, current.tooltip.transitionDuration);
    }
}
