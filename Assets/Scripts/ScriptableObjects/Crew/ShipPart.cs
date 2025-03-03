using UnityEngine;

[CreateAssetMenu(fileName = "NewShipPart", menuName = "Crew/Ship Part")]
public class ShipPart : ScriptableObject
{
    public string partName;
    public int durability;
    public int level;
    public GenericEffect genericEffect;

    private const int maxDurability = 100;
    private const int minDurability = 0;

    [SerializeField] private int defaultDurability = 100;
    [SerializeField] private int defaultLevel = 1;

    public void ResetValues()
    {
        durability = defaultDurability;
        level = defaultLevel;
    }

    public void TakeDamage(int amount)
    {
        durability -= amount;
        if (durability < minDurability)
        {
            durability = minDurability;
            Debug.Log($"{partName} is destroyed!");
        }
        else
        {
            Debug.Log($"{partName} durability: {durability}");
        }
    }

    public void Repair(int amount)
    {
        durability += amount;
        if (durability > maxDurability)
        {
            durability = maxDurability;
            Debug.Log($"{partName} is fully repaired!");
        }
        else
        {
            Debug.Log($"{partName} repaired. Current durability: {durability}");
        }
            
    }

    public void ApplyEffect()
    {
        if (genericEffect != null)
        {
            genericEffect.ApplyEffect();
            Debug.Log($"{partName} applied effect: {genericEffect.effectName}");
        }
        else
        {
            Debug.Log($"{partName} has no effect.");
        }
    }
}