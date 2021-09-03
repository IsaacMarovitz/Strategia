using UnityEngine;
using System.Collections.Generic;

public class UnitAppearanceManager : MonoBehaviour {

    public GameObject meshParent;
    public List<Renderer> playerColoredRenderers;

    const string PLACEHOLDER_MAT_NAME = "COLOR_PLACEHOLDER (Instance)";

    public void UpdateColor(Player player) {
        foreach (var renderer in playerColoredRenderers) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                if (renderer.materials[i].name == PLACEHOLDER_MAT_NAME) {
                    Material[] mats = renderer.materials;
                    mats[i] = player.playerMaterial;
                    renderer.materials = mats;
                }
            }
        }
    }

    public void Show() {
        if (meshParent != null) {
            meshParent.SetActive(true);
        }
    }

    public void Hide() {
        if (meshParent != null) {
            meshParent.SetActive(false);
        }
    }
}