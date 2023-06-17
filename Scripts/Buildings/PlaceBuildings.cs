using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceBuildings : MonoBehaviour
{
    private PlayerScript player;
    private GridManager gridManager;
    private GameObject ghostBuilding;

    // Prefabs for each type of building
    [Header("Turret")]
    public GameObject buildingPrefab1;
    public int building1Cost;

    [Header("Mine")]
    public GameObject buildingPrefab2;
    public int building2Cost;

    [Header("CarrotFarm")]
    public GameObject buildingPrefab3;
    public int building3Cost;

    [Header("SnapToGrid")]
    public bool snapToGrid;
    public GameObject snapToGridButton;
    public Color normalColor = Color.white;
    public Color toggledColor = Color.red;
    private Color targetColor;
    public float colorTransitionSpeed = 2.0f; // Speed of color transition
    private Image image;

    // Currently selected building prefab
    private GameObject selectedBuildingPrefab;
    private int selectedBuildingCost;

    private void Start()
    {
        player = GetComponent<PlayerScript>();
        gridManager = FindObjectOfType<GridManager>();

        image = snapToGridButton.GetComponent<Image>();
        snapToGrid = true;
    }

    private void Update()
    {
        ChangeButtonColour();

        // Update the position of the ghost building to follow the mouse
        if (ghostBuilding != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10; // distance from the camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 gridPos = gridManager.SnapToGrid(worldPos);

            Vector3 finalPosition;

            if (snapToGrid)
            {
                // Snap the position to the grid
                finalPosition = gridManager.SnapToGrid(worldPos);
            }
            else
            {
                // Do not snap, use the world position directly
                finalPosition = worldPos;
            }

            ghostBuilding.transform.position = finalPosition;
        }
    }

    // Event handlers for the buttons
    public void SelectBuilding1()
    {
        StartDraggingBuilding(buildingPrefab1, building1Cost);
        Debug.Log(selectedBuildingPrefab);
    }

    public void SelectBuilding2()
    {
        StartDraggingBuilding(buildingPrefab2, building2Cost);
        Debug.Log(selectedBuildingPrefab);
    }

    public void SelectBuilding3()
    {
        StartDraggingBuilding(buildingPrefab3, building3Cost);
        Debug.Log(selectedBuildingPrefab);
    }

    public void StartDraggingBuilding(GameObject buildingPrefab, int buildingCost)
    {
        if (ghostBuilding == null)
        {
            selectedBuildingPrefab = buildingPrefab;
            selectedBuildingCost = buildingCost;
            ghostBuilding = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);

            // Maybe make the ghost building semi-transparent or different visually
            // ...
        }
    }

    public void EndDraggingBuilding()
    {
        if (ghostBuilding != null && player.currentCash >= selectedBuildingCost)
        {
            player.RemoveCash(selectedBuildingCost);
            // The ghost building becomes the actual building
        }
        else
        {
            // Destroy the ghost building if not enough cash or dragging canceled
            Destroy(ghostBuilding);
        }

        ghostBuilding = null;
        DeselectBuilding();
    }

    public void DeselectBuilding()
    {
        selectedBuildingCost = 0;
        selectedBuildingPrefab = null;
    }

    //Snap To Grid
    public void ToggleSnapToGrid()
    {
        snapToGrid = !snapToGrid;

        // Set the target color based on the snapToGrid state
        targetColor = snapToGrid ? normalColor : toggledColor;
    }

    private void ChangeButtonColour()
    {
        image.color = Color.Lerp(image.color, targetColor,Time.deltaTime * colorTransitionSpeed);
    }
}
