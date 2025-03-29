using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayText : MonoBehaviour
{
    public float textTimer = 1.0f;
    public float textTiming = 0f;

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if(textTimer > textTiming)
            {
                textTiming += Time.deltaTime;
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    public void SetText(string sub)
    {
        this.GetComponent<Text>().text = sub;
        textTiming = 0;
        this.gameObject.SetActive(true);
    }
}
