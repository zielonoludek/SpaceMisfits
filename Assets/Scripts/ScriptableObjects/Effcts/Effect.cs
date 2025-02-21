using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public string effectName;
    public string description;
    public EffectType effectType; 
    public int amount; 

    public abstract void ApplyEffect();
}
