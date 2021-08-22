using UnityEngine;
using System.Collections.Generic;

public class DragWindowManager : MonoBehaviour {

    private static DragWindowManager _instance;
    public static DragWindowManager Instance { get { return _instance; } }

    [SerializeField]
    private List<DragWindow> dragWindows = new List<DragWindow>();

    public Vector2Int sortOrderRange = new Vector2Int(2, 10);
    int numberOfSpaces { get { return sortOrderRange.y - sortOrderRange.x; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
    }

    public void AddWindow(DragWindow dragWindow) {
        dragWindows.Add(dragWindow);
        if (ValidateRange()) {
            dragWindow.canvas.sortingOrder = sortOrderRange.x + dragWindows.Count - 1;
        }
    }

    public void BringToFront(DragWindow dragWindow) {
        int index = dragWindows.IndexOf(dragWindow);
        if (index != -1) {
            dragWindows.RemoveAt(index);
            dragWindows.Add(dragWindow);
            
            for (int i = 0; i < dragWindows.Count; i++) {
                dragWindows[i].canvas.sortingOrder = sortOrderRange.x + i;
            }
        } else {
            Debug.LogWarning($"<b>Drag Window Manager:</b> Window '{dragWindow.gameObject.name}' not found in list!");
        }
    }

    public bool ValidateRange() {
        if (dragWindows.Count > numberOfSpaces) {
            Debug.LogWarning("<b>Drag Window Manager:</b> Too many drag windows, allocate more sort order space!");
            return false;
        } else {
            // Debug.Log($"<b>Debug Window Manger:</b> Added new Drag Window, {numberOfSpaces - dragWindows.Count} entries remaining");
            return true;
        }
    }
}