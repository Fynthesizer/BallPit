using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class timbreToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Toggle toggle;
    Image image;

    bool hovering = false;
    float currentAlpha = 0.2f;
    float targetAlpha = 0.2f;

    public float fadeSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        toggle = gameObject.GetComponent<Toggle>();
        image = transform.GetChild(0).GetComponent<Image>();
        image.color = new Color(255, 255, 255, currentAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        if (toggle.isOn) targetAlpha = 1;
        else if (hovering) targetAlpha = 0.5f;
        else targetAlpha = 0.1f;

        image.color = new Color(255, 255, 255, currentAlpha);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnDisable()
    {
        hovering = false;
        if (!toggle.isOn) currentAlpha = 0.2f;
    }
}
