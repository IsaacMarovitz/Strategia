using UnityEngine;

public class SelectionArrow : TurnBehaviour {

    public float yOffset;
    public float rotationSpeed;
    public GameObject mesh;

    void Update() {
        this.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
    }

    public override void OnUnitSelected(Unit unit) {
        mesh.SetActive(true);
        this.transform.position = GridUtilities.TileToWorldPos(unit.pos, yOffset);
        this.transform.rotation = Quaternion.identity;
    }

    public override void OnUnitDeselected() {
        mesh.SetActive(false);
    }

    public override void OnUnitMove(Unit unit) {
        this.transform.position = GridUtilities.TileToWorldPos(unit.pos, yOffset);
    }
}
