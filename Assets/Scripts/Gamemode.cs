using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode : MonoBehaviour
{
    [Header("Tools")]
    public bool toggleCursorVisibility;

    [Header("Turrets")]
    public Turret[] turrets;

    [Header("Bullets?")]
    public GameObject bulletsObj;
    public float bulletForceStr;

    [Header("Player Statistics")]
    [HideInInspector]
    public Player player;
    public Transform playerBulletSpawner;
    public float reloadTime;
    public float placingTimeCD;

    private void Awake()
    {
        FindPlayer();
    }
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

    void FindPlayer()
    {
        player = FindObjectOfType<Player>();
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

    public IEnumerator StartShootCD(float reloadTime)
    {
        player.shooting = true;

        yield return new WaitForSeconds(reloadTime);

        player.shooting = false;
    }

    public IEnumerator StartPlaceCD(float placingTime)
    {
        player.placing = true;

        yield return new WaitForSeconds(placingTime);

        player.placing = false;
    }
}
