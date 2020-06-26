using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Gamemode gm;

    private Rigidbody rb;

    

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.forward * gm.bulletForceStr);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            Debug.Log("hit with " + gm.player.activeColor);
            other.GetComponent<Renderer>().material.SetColor("_Color", gm.player.activeColor);
        }
    }
}
