using System;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    [Header("Cached References")]
    internal Player player;

    [Header("Score")]
    [ReadOnly, SerializeField] int score;

    void Start()
    {
        player  = FindObjectOfType<Player>();

    }

    void Update()
    {
        Debug.Log(GameManager.Instance.player.CurrentHealth);
    }
}