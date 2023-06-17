using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BuildingButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject panel;

    private void Start()
    {
        panel = transform.Find("Panel").gameObject;

        panel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // The mouse is over the button, show the panel
        panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // The mouse has left the button, hide the panel
        panel.SetActive(false);
    }
}
