using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Transactions;

public class Mine : MonoBehaviour
{
    [SerializeField] private float cashPerTick = 10.0f;  // How much cash to generate each tick.
    [SerializeField] private float tickRate = 1.0f;      // How often to generate cash (in seconds).
    public int mineSellCash;

    private PlayerScript playerScript;   // Reference to the player script.

    //popUp
    [Header("Pop Up")]
    [SerializeField] private GameObject cashPopUp;
    [SerializeField] private GameObject popUpPoint;

    [Header("Upgrade1")]
    [SerializeField] private GameObject upgrade1;
    [SerializeField] private int u1Cost;
    [SerializeField] private int u1CashGen;
    [SerializeField] private int u1CashGenRate;
    private bool u1Active;

    [Header("Upgrade2")]
    [SerializeField] private GameObject upgrade2;
    [SerializeField] private int u2Cost;
    [SerializeField] private int u2CashGen;
    [SerializeField] private int u2CashGenRate;
    private bool u2Active;

    [Header("Ui")]
    public GameObject sellUpgradePanel; // Assign in Inspector
    public Button upgradeButton; // Assign in Inspector

    //animations
    private Animator animator;

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        animator = GetComponentInChildren<Animator>();

        //upgrades
        upgrade1.SetActive(false);
        upgrade2.SetActive(false);

        //panels
        sellUpgradePanel.SetActive(false);

        //booleans
        u1Active = false;
        u2Active = false;

        // Check if PlayerScript was successfully found.
        if (playerScript != null)
        {
            // Begin generating cash.
            StartCoroutine(GenerateCash());
        } else
        {
            return;
        }
    }

    IEnumerator GenerateCash()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickRate);
            playerScript.AddCash(cashPerTick);
            DisplayCashPopup(cashPerTick);
            animator.SetTrigger("Tick");
        }
    }

    private void DisplayCashPopup(float cash)
    {
        GameObject cashText = Instantiate(cashPopUp, popUpPoint.transform.position, Quaternion.identity) as GameObject;
        cashText.GetComponentInChildren<TextMeshPro>().text = "$" + cash.ToString();
    }

    public void Upgrade()
    {
        if (!u1Active) //upgrade 1
        {
            if (playerScript.currentCash > u1Cost)
            {
                playerScript.RemoveCash(u1Cost);
                upgrade1.SetActive(true);
                u1Active = true;

                //implement upgrade
                cashPerTick = u1CashGen;
                tickRate = u1CashGenRate;
            }
            else
            {
                Debug.Log("Insufficient Funds");
            }
        } else if(!u2Active) //upgrade 2
        {
            if(playerScript.currentCash > u2Cost)
            {
                playerScript.RemoveCash(u2Cost);

                //activate second upgrade
                upgrade2.SetActive(true);
                u2Active = true;

                //implement upgrade
                cashPerTick = u2CashGen;
                tickRate = u2CashGenRate;

                //deactivate first
                upgrade1.SetActive(false);
            } else
            {
                Debug.Log("Insufficient Funds");
            }
        }

        sellUpgradePanel.SetActive(false);
    }

    public void Sell()
    {
        Destroy(gameObject);
        playerScript.AddCash(mineSellCash);
    }

    //ui
    private void OnMouseDown()
    {
        sellUpgradePanel.SetActive(true);
    }

    public void ClosePanel()
    {
        sellUpgradePanel.SetActive(false);
    }
}
