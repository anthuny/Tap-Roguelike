using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode : MonoBehaviour
{
    [Header("Tools")]
    public bool toggleCursorVisibility;

    [Header("Turrets")]
    public Turret[] turrets;

    // Start is called before the first frame update
    void Start()
    {
        ToggleMouseVisibility(toggleCursorVisibility);
        ChangeCursorState("locked");
    }

    void ToggleMouseVisibility(bool trigger)
    {
        Cursor.visible = false;
    }

    void ChangeCursorState(string state)
    {
        switch (state)
        {
            case "locked":
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
