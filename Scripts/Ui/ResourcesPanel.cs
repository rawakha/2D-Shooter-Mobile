using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesPanel : MonoBehaviour
{
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private TextMeshProUGUI carrotText;
    [SerializeField] private PlayerResources playerResources;

    // Start is called before the first frame update
    void Start()
    {
        playerResources = FindObjectOfType<PlayerResources>();

        resourcePanel.SetActive(false);
    }

    private void Update()
    {
        UpdateCarrotText();
    }

    public void OpenResourcePanel()
    {
        resourcePanel.SetActive(true);
    }

    public void CloseResourcePanel()
    {
        resourcePanel.SetActive(false);
    }

    private void UpdateCarrotText()
    {
        carrotText.text = playerResources.GetCarrotAmount().ToString();
    }
}
