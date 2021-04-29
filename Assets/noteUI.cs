using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class noteUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public Sprite unselectedSprite;
    public Sprite selectedSprite;

    Image img;
    public controller controller;
    public int noteIndex;

    // Start is called before the first frame update
    void Start()
    {
        img = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        controller.SetPitch(noteIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            controller.SetPitch(noteIndex); 
        }
        
    }

    public void SetState(bool state)
    {
        if (state) img.sprite = selectedSprite;
        else img.sprite = unselectedSprite;
    }
}
