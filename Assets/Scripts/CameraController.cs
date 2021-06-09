using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    public Camera mainCamera;
    public Transform virtualCamera;
    public Transform cameraRig;

    [Space(10)]
    public float normalSpeed = 0.5f;
    public float fastSpeed = 1.5f;
    public float movementTime = 10;
    public float dragDeltaThreshold;
    public Vector2 rotationAmount = new Vector2(1, 1);
    public Vector3 zoomAmount = new Vector3(0, -5, -5);

    [Space(10)]
    public Vector3 maxPosition = new Vector3(220, 100, 220);
    public Vector3 minPosition = new Vector3(-20, -0, -20);
    // x = min, y = max
    public Vector2 zoomLimits = new Vector2(-10, 40);
    public Vector2 xRotationLimits = new Vector2(0, 90);

    [Space(10)]
    public LayerMask ignoredLayers;
    public UnitUI unitUI;

    [HideInInspector]
    public bool didDrag = false;
    [HideInInspector]
    public bool didClickUI = false;

    private bool isPaused;
    private Unit oldUnit;
    private City oldCity;
    private float movementSpeed;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;
    private float newXRotation;

    private Vector3 worldDragStartPosition;
    private Vector3 worldDragCurrentPosition;
    private Vector3 screenDragStartPosition;
    private Vector3 screenDragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    private Vector3 startingZoom;

    void Start() {
        GameManager.Instance.pauseGame += Pause;
        GameManager.Instance.resumeGame += Resume;

        instance = this;
        newPosition = cameraRig.position;
        newRotation = cameraRig.rotation;
        startingZoom = virtualCamera.transform.localPosition;
        newZoom = startingZoom;
        newXRotation = virtualCamera.transform.localRotation.eulerAngles.x;
    }

    void Pause() {
        isPaused = true;
    }

    void Resume() {
        isPaused = false;
    }

    void Update() {
        if (!isPaused) {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, ignoredLayers)) {
                if (!IsMouseOverUI()) {
                    TileTag tileTag = hit.transform.gameObject.GetComponent<TileTag>();
                    if (tileTag != null) {
                        UIData.Instance.mouseOverTile = GameManager.Instance.grid.grid[tileTag.pos.x, tileTag.pos.y];
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && IsMouseOverUI()) {
                didClickUI = true;
            }

            if (Input.GetMouseButtonUp(0) && !unitUI.showLine && !didDrag) {
                if (!didClickUI) {
                    ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit)) {
                        if (!IsMouseOverUI()) {
                            if (hit.transform.tag == "Unit") {
                                Unit hitUnit = hit.transform.parent.gameObject.GetComponent<Unit>();
                                if (GameManager.Instance.GetCurrentPlayer().playerUnits.Contains(hitUnit)) {
                                    UIData.Instance.currentUnit = hitUnit;
                                    Focus(GridUtilities.TileToWorldPos(hitUnit.pos), true);
                                    Debug.Log("<b>Camera Controller:</b> Found Unit");
                                }
                            } else {
                                UIData.Instance.currentUnit = null;
                            }
                            if (hit.transform.tag == "City") {
                                City hitCity = hit.transform.gameObject.GetComponent<City>();
                                if (GameManager.Instance.GetCurrentPlayer().playerCities.Contains(hitCity)) {
                                    UIData.Instance.currentCity = hitCity;
                                    Focus(GridUtilities.TileToWorldPos(hitCity.pos), true);
                                    Debug.Log("<b>Camera Controller:</b> Found City");
                                }
                            } else {
                                UIData.Instance.currentCity = null;
                            }
                        }
                    }
                } else {
                    didClickUI = false;
                }
            }
        }
        if (UIData.Instance.currentUnit != null) {
            if (oldUnit != UIData.Instance.currentUnit) {
                oldUnit = UIData.Instance.currentUnit;
                oldCity = null;
                Focus(GridUtilities.TileToWorldPos(oldUnit.pos), true);
            }
        } else if (UIData.Instance.currentCity != null) {
            if (oldCity != UIData.Instance.currentCity) {
                oldCity = UIData.Instance.currentCity;
                oldUnit = null;
                Focus(GridUtilities.TileToWorldPos(oldCity.pos), true);
            }
        }
    }

    public void LateUpdate() {
        if (!isPaused) {
            GetMouseInput();
            GetMovementInput();
        }
    }

    private void GetMouseInput() {
        if (Input.mouseScrollDelta.y != 0) {
            if (Input.GetMouseButton(1)) {
                newXRotation += Input.mouseScrollDelta.y * rotationAmount.x * 10;
            } else {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry) && !IsMouseOverUI()) {
                worldDragStartPosition = ray.GetPoint(entry);
            }
            screenDragStartPosition = Input.mousePosition;
            didDrag = false;
        }
        if (Input.GetMouseButton(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry)) {
                worldDragCurrentPosition = ray.GetPoint(entry);
                screenDragCurrentPosition = Input.mousePosition;

                if (IsMouseOverUI()) {
                    worldDragStartPosition = worldDragCurrentPosition;
                    screenDragStartPosition = screenDragCurrentPosition;
                } else {
                    if ((screenDragStartPosition - screenDragCurrentPosition).magnitude > dragDeltaThreshold) {
                        didDrag = true;
                        newPosition = cameraRig.position + worldDragStartPosition - worldDragCurrentPosition;
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) {
            rotateCurrentPosition = Input.mousePosition;

            if (IsMouseOverUI()) {
                rotateStartPosition = rotateCurrentPosition;
            } else {
                Vector3 difference = rotateStartPosition - rotateCurrentPosition;
                rotateStartPosition = rotateCurrentPosition;
                newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
            }
        }
    }

    private void GetMovementInput() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            movementSpeed = fastSpeed;
        } else {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += (cameraRig.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPosition += (cameraRig.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPosition += (cameraRig.right * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPosition += (cameraRig.right * movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount.y);
        }
        if (Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount.y);
        }

        if (Input.GetKey(KeyCode.R)) {
            newXRotation -= rotationAmount.x;
        }
        if (Input.GetKey(KeyCode.F)) {
            newXRotation += rotationAmount.x;
        }

        if (Input.GetKey(KeyCode.Z)) {
            newZoom += zoomAmount / 10;
        }
        if (Input.GetKey(KeyCode.X)) {
            newZoom -= zoomAmount / 10;
        }

        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);
        newPosition.z = Mathf.Clamp(newPosition.z, minPosition.z, maxPosition.z);

        newZoom.y = Mathf.Clamp(newZoom.y, zoomLimits.x, zoomLimits.y);
        newZoom.z = startingZoom.z - newZoom.y + startingZoom.y;

        newXRotation = Mathf.Clamp(newXRotation, xRotationLimits.x, xRotationLimits.y);

        cameraRig.position = Vector3.Lerp(cameraRig.position, newPosition, Time.deltaTime * movementTime);
        cameraRig.rotation = Quaternion.Lerp(cameraRig.rotation, newRotation, Time.deltaTime * movementTime);
        virtualCamera.transform.localPosition = Vector3.Lerp(virtualCamera.transform.localPosition, newZoom, Time.deltaTime * movementTime);
        virtualCamera.localRotation = Quaternion.Euler(Vector3.Lerp(virtualCamera.localRotation.eulerAngles, new Vector3(newXRotation, 0, 0), Time.deltaTime * movementTime));
    }

    private Vector3 GetYRotation() {
        Vector3 p_Rotation = new Vector3();
        if (Input.GetKey(KeyCode.Q)) {
            p_Rotation += new Vector3(0, -1f, 0);
        }
        if (Input.GetKey(KeyCode.E)) {
            p_Rotation += new Vector3(0, 1f, 0);
        }
        return p_Rotation;
    }

    private Vector3 GetXRotation() {
        Vector3 p_Rotation = new Vector3();
        if (Input.GetKey(KeyCode.R)) {
            p_Rotation += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.F)) {
            p_Rotation += new Vector3(1, 0, 0);
        }
        return p_Rotation;
    }

    public void Focus(Vector3 pos, bool smoothMove) {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        float entry;
        if (plane.Raycast(ray, out entry)) {
            Vector3 cameraCenterWorldPos = ray.GetPoint(entry);
            Vector3 posDifference = cameraRig.position - cameraCenterWorldPos;
            newPosition = pos + posDifference;

            if (!smoothMove) {
                cameraRig.position = newPosition;
            }
        }
    }

    public bool IsMouseOverUI() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++) {
            if (raycastResultList[i].gameObject.GetComponent<IgnoreRaycast>() != null) {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultList.Count > 0;
    }
}