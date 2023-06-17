using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public GameUi gameUi;

    [Header("Logic")]
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private LayerMask weaponPickupLayerMask;
    [SerializeField] private float pickupRadius;

    [Header("Weapons")]
    [SerializeField] private GameObject revolver;
    [SerializeField] private GameObject assaultRifle;

    private string currentWeapon;

    private void Start()
    {
        playerScript = GetComponent<PlayerScript>();
    }

    private void Update()
    {
        if (playerScript.dead)
        {
            DisableAllWeapons();
        }
        else
        {
            //WeaponPickUp();
        }
    }

    /*
    private void WeaponPickUp()
    {
        Collider2D weaponCollider = Physics2D.OverlapCircle(transform.position, pickupRadius, weaponPickupLayerMask);

        if (weaponCollider != null && Input.GetKeyDown(KeyCode.Q))
        {
            Weapon weapon = weaponCollider.GetComponent<Weapon>();

            if (weapon != null)
            {
                SwitchWeapon(weapon.WeaponName);
            }
        }
    }*/

    public void SwitchWeapon(string newWeapon)
    {
        currentWeapon = newWeapon;

        switch (currentWeapon)
        {
            case "assaultRifle":
                assaultRifle.SetActive(true);
                revolver.SetActive(false);
                break;
            case "revolver":
                revolver.SetActive(true);
                assaultRifle.SetActive(false);
                break;
        }

        gameUi.UpdateMaxAmmo();
    }

    private void DisableAllWeapons()
    {
        assaultRifle.SetActive(false);
        revolver.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
