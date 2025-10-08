using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[CreateAssetMenu(fileName = "MovementValues", menuName = "Scriptable Objects/MovementValues")]
public class PlayerMovementValues : ScriptableObject
{
    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float Gravity = 5f;
    public float WallJumpForceX = 5f;
    public float WallJumpForceY = 5f;
}
