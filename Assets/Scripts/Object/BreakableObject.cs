using UnityEngine;

public class BreakableObject : HealthManager
{

    public override void Die()
    {
        print("object broken");
        Destroy(this.gameObject);
    }
}
