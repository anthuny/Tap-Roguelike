using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevManager : MonoBehaviour
{
    private Gamemode gm;
    public GameObject devText;
    public Font font;
    public int maxDevTextCount;
    private List<GameObject> devTexts = new List<GameObject>();

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    public IEnumerator FlashText(string contents)
    {
        // If dev mode is not on, do not continue
        if (!gm.devTextOn)
        {
            yield return null;
        }

        #region Spawn and assign core variables for Dev Text
        GameObject go = Instantiate(devText, gm.devTextParent.transform.position, Quaternion.identity);
        go.transform.SetParent(gm.devTextParent.transform);
        go.transform.position = new Vector3(gm.devTextParent.transform.position.x, 1880, gm.devTextParent.transform.position.z);
        go.name = "DevText";
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 70);

        Text text = go.GetComponent<Text>();
        text.text = contents;
        #endregion

        // Insert the new line of text at the start of the list
        devTexts.Insert(0, go);

        // Move past lines of text down for each new line
        for (int i = 0; i < devTexts.Count; i++)
        {
            // Skip the first element in list. It is always the newest line of text
            if (i != 0)
            {
                devTexts[i].transform.position += new Vector3(0, -80, 0);
            }

            // Cap the amount of lines by removing the oldest lines of text
            if (i >= maxDevTextCount)
            {
                Destroy(devTexts[devTexts.Count-1]);
                devTexts.RemoveAt(devTexts.Count-1);
            }
        }
    }
}
