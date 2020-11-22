using UnityEngine;

public class UIData : MonoBehaviour {

    private static UIData _instance;
    public static UIData Instance { get { return _instance; } }

    public Unit currentUnit;
    public City currentCity;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(_instance);
    }

    public void Move(int dir) {
        if (currentUnit != null) {
            currentUnit.Move(dir);
        }
    }
}