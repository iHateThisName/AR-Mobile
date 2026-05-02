using Assets.Scripts.Singleton;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public static event System.Action TapDetected;

    [field: SerializeField] public int CurrentTaps { get; private set; } = 0;

    [SerializeField] private int maxTaps = 3;
    [SerializeField] private GameObject pillarPrefab;

    public int OnTap(Vector3 position, Quaternion rotation) {
        if (this.CurrentTaps == this.maxTaps) return this.CurrentTaps;
        this.CurrentTaps++;

        if (this.CurrentTaps <= this.maxTaps) {
            Instantiate(this.pillarPrefab, position, rotation);
        } else {
            // For testing purposes.
            //this.CurrentTaps = 0;
        }

        TapDetected?.Invoke();
        return this.CurrentTaps;
    }
}