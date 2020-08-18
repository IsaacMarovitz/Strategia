using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour {

    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


    float mainSpeed = 100.0f; //regular speed
    //float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)

    void Update() {
        /*lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;*/
        //Mouse camera angle done.  

        //Keyboard commands
        Vector3 p = GetBaseInput();
        p = p * mainSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position;
        transform.parent.transform.Translate(p);
        transform.parent.eulerAngles += GetYRotation();
        transform.eulerAngles += GetXRotation();
    }

    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
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