using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/Generic Effect")]
public class GenericEffect : Effect
{
    public override void ApplyEffect()
    {
        if (ResourceManager.Instance == null) return;

        switch (effectType)
        {
            case EffectType.Booty:
                ResourceManager.Instance.Booty += amount;
                Debug.Log($"{effectName}: Booty changed by {amount}");
                break;
            case EffectType.Notoriety:
                ResourceManager.Instance.Notoriety += amount;
                Debug.Log($"{effectName}: Notoriety changed by {amount}");
                break;
        }
    }
}
