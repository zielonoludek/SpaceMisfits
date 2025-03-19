using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int shipHealth = 1000;
    [SerializeField] private int booty = 1000;
    [SerializeField] private int food = 1000;
    [SerializeField] private int notoriety = 1000;
    [SerializeField] private int crewMood = 100;
    [SerializeField] private int sightLevel = 0;
    [SerializeField] private int speedValue = 1;

    private Dictionary<EffectType, Func<int>> resourceGetters;

    public event Action<int> OnShipHealthChanged;
    public event Action<int> OnBootyChanged;
    public event Action<int> OnNotorietyChanged;
    public event Action<int> OnCrewMoodChanged;
    public event Action<int> OnSightChanged;
    public event Action<int> OnSpeedChanged;
    public event Action<int> OnFoodChanged;

    public event Action<EffectType> OnResourceEmpty;

    /*
    // Example usage:

    GameManager.Instance.ResourceManager.Booty += 100; (trigger events in other scripts)
    GameManager.Instance.ResourceManager.Notoriety -= 5;


    GameManager.Instance.ResourceManager.OnResourceEmpty += HandleResourceEmpty;
    
    void HandleResourceEmpty(EffectType resource)
    {
        if (resource == EffectType.Food) Debug.Log("Food has been depleted! Crew is starving!");
    }
    */

    private void Awake()
    {
        resourceGetters = new Dictionary<EffectType, Func<int>>
        {
            { EffectType.Health, () => ShipHealth },
            { EffectType.Booty, () => Booty },
            { EffectType.Notoriety, () => Notoriety },
            { EffectType.CrewMood, () => CrewMood },
            { EffectType.Food, () => Food },
            { EffectType.Sight, () => Sight },
            { EffectType.Speed, () => Speed }
        };
    }

    public int ShipHealth
    {
        get => shipHealth;
        set
        {
            if (shipHealth != value)
            {
                shipHealth = Mathf.Max(0, value);
                OnShipHealthChanged?.Invoke(shipHealth);
                if (shipHealth == 0) OnResourceEmpty?.Invoke(EffectType.Health);
            }
        }
    }
    public int Booty
    {
        get => booty;
        set
        {
            if (booty != value)
            {
                booty = Mathf.Max(0, value);
                OnBootyChanged?.Invoke(booty);
                if (booty == 0) OnResourceEmpty?.Invoke(EffectType.Booty);
            }
        }
    }
    public int Notoriety
    {
        get => notoriety;
        set
        {
            if (notoriety != value)
            {
                notoriety = Mathf.Max(0, value);
                OnNotorietyChanged?.Invoke(notoriety);
                if (notoriety == 0) OnResourceEmpty?.Invoke(EffectType.Notoriety);
            }
        }
    }
    public int CrewMood
    {
        get => crewMood;
        set
        {
            if (crewMood != value)
            {
                crewMood = Mathf.Max(0, value);
                OnCrewMoodChanged?.Invoke(crewMood);
                if (crewMood == 0) OnResourceEmpty?.Invoke(EffectType.CrewMood);
            }
        }
    }
    public int Food
    {
        get => food;
        set
        {
            if (food != value)
            {
                food = Mathf.Max(0, value);
                OnFoodChanged?.Invoke(food);
                if (food == 0) OnResourceEmpty?.Invoke(EffectType.Food);
            }
        }
    }
    public int Sight
    {
        get => sightLevel;
        set
        {
            sightLevel = Mathf.Clamp(value, 0, 3);
            OnSightChanged?.Invoke(sightLevel);
            if (sightLevel == 0) OnResourceEmpty?.Invoke(EffectType.Sight);
        }
    }
    public int Speed
    {
        get => speedValue;
        set
        {
            speedValue = Mathf.Clamp(value, 0, 3);
            OnSpeedChanged?.Invoke(speedValue);
            if (speedValue == 0) OnResourceEmpty?.Invoke(EffectType.Speed);
        }
    }
   
    public int GetCurrentSpeed()
    {
        return Speed;
    }
    public int GetCurrentSight()
    {
        return Sight;
    }
    public int GetResourceValue(EffectType type)
    {
        return resourceGetters.TryGetValue(type, out var getter) ? getter() : 0;
    }
    public bool TryGetResourceTypeFromRequestOrigin(RequestOriginType requestType, out EffectType effectType)
    {
        Dictionary<RequestOriginType, EffectType> mapping = new Dictionary<RequestOriginType, EffectType>
        {
            { RequestOriginType.Booty, EffectType.Booty },
            { RequestOriginType.Notoriety, EffectType.Notoriety },
            { RequestOriginType.Health, EffectType.Health },
            { RequestOriginType.Sight, EffectType.Sight },
            { RequestOriginType.Speed, EffectType.Speed },
            { RequestOriginType.Food, EffectType.Food },
            { RequestOriginType.CrewMood, EffectType.CrewMood }
        };

        return mapping.TryGetValue(requestType, out effectType);
    }
    public void ModifyResource(EffectType type, int amount)
    {
        switch (type)
        {
            case EffectType.Health:
                ShipHealth += amount;
                break;
            case EffectType.Booty:
                Booty += amount;
                break;
            case EffectType.Notoriety:
                Notoriety += amount;
                break;
            case EffectType.CrewMood:
                CrewMood += amount;
                break;
            case EffectType.Food:
                Food += amount;
                break;
            case EffectType.Sight:
                Sight += amount;
                break;
            case EffectType.Speed:
                Speed += amount;
                break;
            default:
                Debug.LogWarning($"EffectType {type} is not recognized in ModifyResource.");
                break;
        }
    }

}