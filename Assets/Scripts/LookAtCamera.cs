using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    public Camera mainCamera;

    void Update() {
        transform.LookAt(mainCamera.transform.position, -Vector3.up);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
