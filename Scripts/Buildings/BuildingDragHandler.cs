using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject buildingPrefab;
    public int buildingCost;

    private PlaceBuildings placeBuildings;

    private void Start()
    {
        placeBuildings = FindObjectOfType<PlaceBuildings>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        placeBuildings.StartDraggingBuilding(buildingPrefab, buildingCost);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // This will be handled by the PlaceBuildings script
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        placeBuildings.EndDraggingBuilding();
    }
}
