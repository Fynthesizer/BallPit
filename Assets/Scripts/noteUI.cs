using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class noteUI : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler
{
    public Sprite unselectedSprite;
    public Sprite selectedSprite;

    Image img;
    public controller controller;
    public int noteIndex;
    
    float holdDuration = 1f;
    bool holding = false;
    bool hovering = false;


    // Start is called before the first frame update
    void Start()
    {
        img = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (holding) holdDuration -= 0.05f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        holding = false;
        holdDuration = 1f;
        hovering = false;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        //if(holdDuration > 0f && hovering == true) controller.SetPitch(noteIndex);
        //holding = false;
        //holdDuration = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.SetPitch(noteIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (Input.GetMouseButton(0))
        //{
        //    controller.SetPitch(noteIndex); 
        //}
        hovering = true;
        
    }

    public void SetState(bool state)
    {
        if (state) img.sprite = selectedSprite;
        else img.sprite = unselectedSprite;
    }
}
