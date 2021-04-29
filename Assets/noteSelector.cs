using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteSelector : MonoBehaviour
{
    public GameObject controller;
    public int currentNote = 24;

    GameObject selectedKey;

    bool showHighlights = true;

    public GameObject[] keys;
    public GameObject[] underlayKeys;

    void Start()
    {
        selectedKey = keys[currentNote];
        UpdateNote(currentNote);

        for (int i = 0; i < underlayKeys.Length; i++)
        {
            float hue = 0;
            if (i < 12) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 12, i));
            else if (i < 24) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(12, 24, i));
            else if (i < 36) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(24, 36, i));
            else if (i < 48) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(36, 48, i));
            Color color = Color.HSVToRGB(hue, 1, 1);
            underlayKeys[i].GetComponent<underlayKey>().SetColour(color);
        }
    }

    void Update()
    {
        
    }

    public void UpdateNote(int note)
    {
        selectedKey.GetComponent<noteUI>().SetState(false);
        currentNote = note;
        selectedKey = keys[currentNote];
        selectedKey.GetComponent<noteUI>().SetState(true);
    }

    public void HighlightNote(int note, float amp)
    {
        if(showHighlights) underlayKeys[note].GetComponent<underlayKey>().Glow(amp);
    }

    public void ToggleHighlights(bool state)
    {
        showHighlights = state;
    }
}
