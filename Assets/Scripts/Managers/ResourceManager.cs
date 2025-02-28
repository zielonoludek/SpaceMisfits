using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private int booty;
    private int notoriety;
    private int sightLevel;

    public event Action<int> OnBootyChanged;
    public event Action<int> OnNotorietyChanged;
    public event Action<int> OnSightChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
            }
        }
    }

    //ResourceManager.Instance.Booty += 100; (trigger booty event in other scripts)
    //ResourceManager.Instance.Notoriety -= 5;

    public int Sight
    {
        get => sightLevel;
        set
        {
            sightLevel = Mathf.Clamp(value, 0, 3);
            OnSightChanged?.Invoke(sightLevel);
        }
    }

    public void IncreaseSight(int amount = 1)
    {
        Sight += amount;
    }

    public void DecreaseSight(int amount = 1)
    {
        Sight -= amount;
    }

    public int GetCurrentSight()
    {
        return Sight;
    }

    //ResourceManager.Instance.IncreaseSight(); //This line adds sight level in other scripts
    //ResourceManager.Instance.DecreaseSight(); //This line removes sight level in other scripts
}