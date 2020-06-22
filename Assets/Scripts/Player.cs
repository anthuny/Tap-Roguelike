using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Gamemode gm;

    public Camera cam;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Plan();
        }
    }

    void Plan()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Ground")
            {
                Instantiate(gm.turrets[0].turretObj, hit.transform.position, Quaternion.identity);
                Debug.Log(hit.transform);
            }

        }

        else
            print("I'm looking at nothing!");
    }
}
