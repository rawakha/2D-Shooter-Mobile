using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUi : MonoBehaviour
{
    [Header("Ui Text")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI maxAmmoText;

    [Header("Ui Animations")]
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Animator scoreAnimator;
    [SerializeField] private Animator ammoAnimator;

    [Header("Starting Stats")]
    [SerializeField] private PlayerScript playerScript;
    private float health;
    private float cash;
    private float wave;

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject aim;
    private Shoot shootScript;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMaxAmmo();
        playerScript = FindAnyObjectByType<PlayerScript>();
        health = playerScript.playerHealthStart;
    }

    // Update is called once per frame
    void Update()
    {
        shootScript = aim.GetComponentInChildren<Shoot>();

        UpdateAmmo();
        UpdateHealth();
        UpdateCash();
    }

    private void UpdateHealth()
    {
        health = playerScript.playerHealth;
        healthText.text = "Health: " + health.ToString();
    }

    private void UpdateCash()
    {
        cash = playerScript.currentCash;
        scoreText.text = "$" + cash.ToString();
    }

    private void UpdateAmmo()
    {
        ammoText.text = shootScript.GetCurrentAmmo().ToString();
    }

    public void UpdateMaxAmmo()
    {
        maxAmmoText.text = shootScript.GetMaxAmmo().ToString();
    }
}
