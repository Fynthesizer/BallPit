using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class underlayKey : MonoBehaviour
{
    float amplitude = 0;
    float decayRate = 0.015f;
    Color color;
    Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = gameObject.GetComponent<Image>();
        color = new Color(0,0,0,1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (amplitude > 0) amplitude -= decayRate;
        else amplitude = 0;

        img.color = new Color(color.r, color.g, color.b, amplitude/2);
    }

    public void SetColour(Color c)
    {
        color = c;
        img.color = c;

    }

    public void Glow(float amp)
    {
        amplitude = amp;
    }
}
