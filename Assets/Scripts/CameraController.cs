using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public Camera mainCamera;
    public Transform xRotationTransform;
    public Transform yRotationTransform;
    public Vector3 positionOffset;
    public Vector2 rotationOffset;
    public LayerMask ignoredLayers;
    float mainSpeed = 100.0f;

    private Vector3 lastMouse = new Vector3(255, 255, 255);
    private bool isPaused;
    private Unit oldUnit;
    private City oldCity;

    void Start() {
        GameManager.Instance.pauseGame += Pause;
        GameManager.Instance.resumeGame += Resume;
    }

    void Pause() {
        isPaused = true;
    }

    void Resume() {
        isPaused = false;
    }

    void Update() {
        if (!isPaused) {
            Vector3 p = GetBaseInput();
            p = p * mainSpeed * Time.deltaTime;
            yRotationTransform.Translate(p);
            yRotationTransform.eulerAngles += GetYRotation();
            xRotationTransform.eulerAngles += GetXRotation();
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, ignoredLayers)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(hit.transform.position.x / GameManager.Instance.grid.tileWidth), Mathf.RoundToInt(hit.transform.position.z / GameManager.Instance.grid.tileHeight));
                    if (gridPos.x < GameManager.Instance.grid.width && gridPos.y < GameManager.Instance.grid.height && gridPos.x > 0 && gridPos.y > 0) {
                        UIData.Instance.mouseOverTile = GameManager.Instance.grid.grid[gridPos.x, gridPos.y];
                    }
                }
            }
            if (Input.GetMouseButtonDown(0)) {
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (hit.transform.tag == "Unit") {
                            Unit hitUnit = hit.transform.parent.gameObject.GetComponent<Unit>();
                            if (GameManager.Instance.GetCurrentPlayer().playerUnits.Contains(hitUnit)) {
                                UIData.Instance.currentUnit = hitUnit;
                                Focus(hitUnit.gameObject.transform.position);
                                Debug.Log("<b>Camera Controller:</b> Found Unit");
                            }
                        } else {
                            UIData.Instance.currentUnit = null;
                        }
                        if (hit.transform.tag == "City") {
                            City hitCity = hit.transform.gameObject.GetComponent<City>();
                            if (GameManager.Instance.GetCurrentPlayer().playerCities.Contains(hitCity)) {
                                UIData.Instance.currentCity = hitCity;
                                Focus(hitCity.gameObject.transform.position);
                                Debug.Log("<b>Camera Controller:</b> Found City");
                            }
                        } else {
                            UIData.Instance.currentCity = null;
                        }
                    }
                }
            }
        }
        if (UIData.Instance.currentCity != null) {
            if (oldCity != UIData.Instance.currentCity) {
                oldCity = UIData.Instance.currentCity;
                oldUnit = null;
                Focus(oldCity.gameObject.transform.position);
            }
        } else if (UIData.Instance.currentUnit != null) {
            if (oldUnit != UIData.Instance.currentUnit) {
                oldUnit = UIData.Instance.currentUnit;
                oldCity = null;
                Focus(oldUnit.gameObject.transform.position);
            }
        }
    }

    private Vector3 GetBaseInput() {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            p_Velocity += new Vector3(0, 0, 0.5f);
        }
        if (Input.GetKey(KeyCode.S)) {
            p_Velocity += new Vector3(0, 0, -0.5f);
        }
        if (Input.GetKey(KeyCode.A)) {
            p_Velocity += new Vector3(-0.5f, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            p_Velocity += new Vector3(0.5f, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space)) {
            p_Velocity += new Vector3(0, 0.5f, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            p_Velocity += new Vector3(0, -0.5f, 0);
        }
        return p_Velocity;
    }

    private Vector3 GetYRotation() {
        Vector3 p_Rotation = new Vector3();
        if (Input.GetKey(KeyCode.Q)) {
            p_Rotation += new Vector3(0, -0.5f, 0);
        }
        if (Input.GetKey(KeyCode.E)) {
            p_Rotation += new Vector3(0, 0.5f, 0);
        }
        return p_Rotation;
    }

    private Vector3 GetXRotation() {
        Vector3 p_Rotation = new Vector3();
        if (Input.GetKey(KeyCode.R)) {
            p_Rotation += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.F)) {
            p_Rotation += new Vector3(-1, 0, 0);
        }
        return p_Rotation;
    }

    public void Focus(Vector3 pos) {
        yRotationTransform.position = pos + positionOffset;
        yRotationTransform.eulerAngles = new Vector3(0, rotationOffset.y, 0);
        xRotationTransform.eulerAngles = new Vector3(rotationOffset.x, 0, 0);
    }
}