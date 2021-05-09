using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionArrow : MonoBehaviour {

    public float yOffset;
    public float rotationSpeed;
    public GameObject mesh;

    private Unit oldUnit;

    void Update() {
        Unit currentUnit = UIData.Instance.currentUnit;
        if (currentUnit != null) {
            mesh.SetActive(true);
            this.transform.position = new Vector3(currentUnit.transform.position.x, yOffset, currentUnit.transform.position.z);
        } else {
            mesh.SetActive(false);
        }

        if (currentUnit != oldUnit) {
            this.transform.rotation = Quaternion.identity;
        } else {
            this.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
        }

        oldUnit = currentUnit;
    }
}
