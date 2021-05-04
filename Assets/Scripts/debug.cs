using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debug : MonoBehaviour
{
    Text text;
    float fps;
    int ballCount;

    public controller controller;

    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    public float m_refreshTime = 0.5f;

    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        ballCount = controller.ballCount;

        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            fps = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
        text.text = "FPS: " + fps + "\n" +
            "Ball count: " + ballCount + "\n" +
            "Version " + Application.version;
    }
}
