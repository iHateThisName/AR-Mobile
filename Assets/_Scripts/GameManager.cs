using Assets.Scripts.Singleton;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public static event System.Action TapDetected;

    [field: SerializeField] public int CurrentTaps { get; private set; } = 0;

    [field: SerializeField] public int MaxTaps { get; private set; } = 3;
    [SerializeField] private GameObject pillarPrefab;

    public int OnTap(Vector3 position, Quaternion rotation) {
        if (this.CurrentTaps == this.MaxTaps) return this.CurrentTaps;
        this.CurrentTaps++;

        if (this.CurrentTaps <= this.MaxTaps) {
            Instantiate(this.pillarPrefab, position, rotation);
        } else {
            // For testing purposes.
            //this.CurrentTaps = 0;
        }

        TapDetected?.Invoke();
        return this.CurrentTaps;
    }

    public void MoveSegment(PillarController.EnumSegment segment, bool isLeft) {
        if (this.CurrentTaps != this.MaxTaps) return;
        PillarController.Instance.RotateSegment(segment, isLeft);
    }

}