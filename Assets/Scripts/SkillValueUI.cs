using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillValueUI : MonoBehaviour
{
    [HideInInspector]
    public SkillUIManager skillUIManager;
    // Update is called once per frame

    private void Start()
    {
        Invoke("RemoveText", skillUIManager.textLifeLength);
    }
    void Update()
    {
        float y = skillUIManager.panSpeedUI * Time.deltaTime;
        transform.position += new Vector3(0, y, 0);
    }

    void RemoveText()
    {
        Destroy(this.gameObject);
    }
}
