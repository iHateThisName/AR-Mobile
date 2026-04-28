using Assets.Scripts.Singleton;
using System.ComponentModel;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [field: SerializeField] public int CurrentTaps { get; private set; } = 0;

    [SerializeField] private int maxTaps = 3;
    [SerializeField] private GameObject pillarPrefab;

    public int OnTap(Vector3 position, Quaternion rotation) {
        this.CurrentTaps++;

        if (this.CurrentTaps <= 3) {
            Instantiate(this.pillarPrefab, position, rotation);
        } else {
            // For testing purposes.
            this.CurrentTaps = 0;
        }

        return this.CurrentTaps;
    }
}