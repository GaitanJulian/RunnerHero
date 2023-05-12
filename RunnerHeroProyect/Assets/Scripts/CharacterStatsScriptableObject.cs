using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsScriptableObject", menuName = "Character Stats")]
public class CharacterStatsScriptableObject : ScriptableObject
{
    public string characterName;

    [Header("Movement system")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;
    public float airAcceleration;
    public float airDeceleration;

    [Header("Jump sistem")]
    public float jumpForce;
    public float fallMultiplier;
    public float jumpTime;
    public float jumpMultiplier;
    public float jumpDecayPercentage;
}