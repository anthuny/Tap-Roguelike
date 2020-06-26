using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Gamemode gm;

    public Camera cam;

    [HideInInspector]
    public bool shooting, placing;

    private TurretBehaviour activeTurret;
    private Transform activeTurretTransform;

    [HideInInspector]
    public Color activeColor;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Plan();
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeColor = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeColor = Color.blue;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeColor = Color.black;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeColor = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeColor = Color.green;
        }
    }

    void Plan()
    {
        if (!placing)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Ground")
                {
                    StartCoroutine(gm.StartPlaceCD(gm.placingTimeCD));

                    GameObject go = Instantiate(gm.turrets[0].turretObj, hit.point, Quaternion.identity);
                    go.transform.rotation = transform.rotation;
                    activeTurret = go.GetComponent<TurretBehaviour>();
                    activeTurretTransform = go.transform;
                }
            }
        }
    }

    void Shoot()
    {
        if (!shooting)
        {
            StartCoroutine(gm.StartShootCD(gm.reloadTime));

            // Shoot from turret
            if (activeTurret)
            {
                Instantiate(gm.bulletsObj, activeTurretTransform.position, activeTurretTransform.rotation);
            }

            // Shoot from player
            Instantiate(gm.bulletsObj, gm.playerBulletSpawner.position, transform.rotation);
        }
    }
}
