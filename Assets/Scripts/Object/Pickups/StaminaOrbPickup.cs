using UnityEngine;

public class StaminaOrbPickup : Pickup
{

    public GameObject pickupParticles;

    public float staminaToRestore = 50;

    protected override void OnPickUp()
    {
        Instantiate(pickupParticles, PlayerMovement.instance.transform.position, Quaternion.identity);

        StaminaManager.instance.RestoreStamina(staminaToRestore);

        Destroy(gameObject);
    }
}
