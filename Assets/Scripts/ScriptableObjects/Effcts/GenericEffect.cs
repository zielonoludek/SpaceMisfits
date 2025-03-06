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
                Debug.Log($"{effectName}: Booty changed by {amount}");
                break;
            case EffectType.Notoriety:
                GameManager.Instance.ResourceManager.Notoriety += amount;
                Debug.Log($"{effectName}: Notoriety changed by {amount}");
                break;
        }
    }
}
