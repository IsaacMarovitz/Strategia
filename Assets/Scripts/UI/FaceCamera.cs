using UnityEngine;

public class FaceCamera : MonoBehaviour {

    private Camera cachedCam;
    private Quaternion originalRotation;

    void Start() {
        cachedCam = Camera.main;
    }

    void Update() {
        transform.LookAt(transform.position + cachedCam.transform.rotation * Vector3.forward, cachedCam.transform.rotation * Vector3.up);
    }
}
