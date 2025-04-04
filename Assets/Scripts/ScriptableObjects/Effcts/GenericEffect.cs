using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/Generic Effect")]
public class GenericEffect : Effect
{
    public override void ApplyEffect()
    {
        if (GameManager.Instance.ResourceManager == null) return;

        switch (effectType)
        {
            case EffectType.Booty:
                GameManager.Instance.ResourceManager.Booty += amount;
                break;
            case EffectType.Notoriety:
                GameManager.Instance.ResourceManager.Notoriety += amount;
                break;
            case EffectType.Health:
                GameManager.Instance.ResourceManager.ShipHealth += amount;
                break;
            case EffectType.Sight:
                GameManager.Instance.ResourceManager.Sight += amount;
                break;
            case EffectType.Speed:
                GameManager.Instance.ResourceManager.Speed += amount;
                break;
            case EffectType.Food:
                GameManager.Instance.ResourceManager.Food += amount;
                break;
            case EffectType.CrewMood:
                GameManager.Instance.ResourceManager.CrewMood += amount;
                break;
            case EffectType.CrewMemberSpot:
                break;
            case EffectType.Durability:
                break;
        }
    }

    public override void RemoveEffect()
    {
        if (GameManager.Instance.ResourceManager == null) return;

        switch (effectType)
        {
            case EffectType.Booty:
                GameManager.Instance.ResourceManager.Booty -= amount;
                break;
            case EffectType.Notoriety:
                GameManager.Instance.ResourceManager.Notoriety -= amount;
                break;
            case EffectType.Health:
                GameManager.Instance.ResourceManager.ShipHealth -= amount;
                break;
            case EffectType.Sight:
                GameManager.Instance.ResourceManager.Sight -= amount;
                break;
            case EffectType.Speed:
                GameManager.Instance.ResourceManager.Speed -= amount;
                break;
            case EffectType.Food:
                GameManager.Instance.ResourceManager.Food -= amount;
                break;
            case EffectType.CrewMood:
                GameManager.Instance.ResourceManager.CrewMood -= amount;
                break;
        }
    }
}
