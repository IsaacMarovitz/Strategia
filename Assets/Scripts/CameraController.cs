using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public Camera mainCamera;
    public Transform xRotationTransform;
    public Transform yRotationTransform;
    // public Vector3 positionOffset;
    // public Vector2 rotationOffset;
    float mainSpeed = 100.0f;
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    private bool isPaused;

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
            Vector3 newPosition = transform.position;
            yRotationTransform.Translate(p);
            yRotationTransform.eulerAngles += GetYRotation();
            xRotationTransform.eulerAngles += GetXRotation();
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (hit.transform.tag == "Unit") {
                            Unit hitUnit = hit.transform.gameObject.GetComponent<Unit>();
                            if (GameManager.Instance.GetCurrentPlayer().playerUnits.Contains(hitUnit)) {
                                UIData.Instance.currentUnit = hitUnit;
                                Debug.Log("<b>Camera Controller:</b> Found Unit");
                            } 
                        } else {
                            UIData.Instance.currentUnit = null;
                        }
                        if (hit.transform.tag == "City") {
                            City hitCity = hit.transform.gameObject.GetComponent<City>();
                            if (GameManager.Instance.GetCurrentPlayer().playerCities.Contains(hitCity)) {
                                UIData.Instance.currentCity = hitCity;
                                Debug.Log("<b>Camera Controller:</b> Found City");
                            }
                        } else {
                            UIData.Instance.currentCity = null;
                        }
                    }
                }
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
}