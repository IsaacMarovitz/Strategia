using UnityEngine;

public class TooltipSystem : MonoBehaviour {

    public Tooltip tooltip;
    private static TooltipSystem current;

    public void Awake() {
        current = this;
        Hide();
    }

    public static void Show(string content, string header = "") {
        current.tooltip.image.SetActive(true);
        current.tooltip.SetText(content, header);
    }

    public static void Hide() {
        current.tooltip.image.SetActive(false);
    }
}
