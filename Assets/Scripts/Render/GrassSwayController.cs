using UnityEngine;

public class GrassSwayController : MonoBehaviour
{
    [Range(0f, 1f)] public float ExternalInfluenceStrength = 0.25f;
    public float EaseInTime = 0.15f;
    public float EaseOutTime = 0.15f;
    public float VelocityThreshold = 5f;
    private int extenralInfluence = Shader.PropertyToID("externalInfluence");

    public void InfluenceGrass(Material mat, float XVelocity)
    {
        mat.SetFloat(extenralInfluence, XVelocity);
    }
}
