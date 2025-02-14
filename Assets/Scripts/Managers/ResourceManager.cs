using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private int booty;

    public event Action<int> OnBootyChanged;

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

    //ResourceManager.Instance.Booty += 100; (trigger booty event in other scripts)

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
}