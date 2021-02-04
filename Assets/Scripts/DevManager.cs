using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevManager : MonoBehaviour
{
    private Gamemode gm;
    public Font font;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();

        InitialLaunch();
    }

    private void InitialLaunch()
    {
        //gm.devText.gameObject.SetActive(false);
    }

    public IEnumerator FlashText(string contents)
    {
        if (!gm.devTextOn)
        {
            yield return null;
        }

        if (gm.devTextParent.transform.childCount == gm.devTextQueueCount)
        {
            GameObject go = Instantiate(new GameObject(), gm.devTextParent.transform.position, Quaternion.identity);
            go.transform.SetParent(gm.devTextParent.transform);
            go.transform.position = new Vector3(gm.devTextParent.transform.position.x, 1860 - (80 * gm.devTextQueueCount), gm.devTextParent.transform.position.z);

            Text text = go.AddComponent<Text>();
            text.alignment = TextAnchor.UpperLeft;
            //text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 46;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = Color.white;
            text.font = font;
            text.resizeTextForBestFit = true;
            text.text = contents;

            go.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 70);

            gm.devTextQueueCount++;
        }

        /*
        // If the text object is not active
        if (!gm.devText.gameObject.activeSelf)
        {
            // Set the text
            gm.devText.text = contents;
            // Show the text
            gm.devText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0f);

        // If the text object is active (after waiting a bit
        if (gm.devText.gameObject.activeSelf)
        {
            // Hide the text
            gm.devText.gameObject.SetActive(false);
        }
        */
    }
}
