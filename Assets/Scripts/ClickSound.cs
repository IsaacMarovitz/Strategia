using UnityEngine;

public class ClickSound : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip audioClip;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
