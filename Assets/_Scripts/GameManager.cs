using Assets.Scripts.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public static event System.Action TapDetected;

    [field: SerializeField] public int CurrentTaps { get; private set; } = 0;

    [field: SerializeField] public int MaxTaps { get; private set; } = 3;
    [SerializeField] private GameObject pillarPrefab;

    [SerializeField] private Material sphereVoidOverlay;
    private readonly string sphereVoidCutoffHeightProperty = "_CuttoffHeight";

    public static Dictionary<EnumDialogueType, string[]> DialogueLines { get; private set; } = new Dictionary<EnumDialogueType, string[]> {
        { EnumDialogueType.None, new string[] {} },
        { EnumDialogueType.Scan, DialogueTreeData.ScanEnvironmentText },
        { EnumDialogueType.FirstCrack, DialogueTreeData.FirstCrackText },
        { EnumDialogueType.RiftOpened, DialogueTreeData.RiftOpenedText },
        { EnumDialogueType.Obelisk, DialogueTreeData.ObeliskText },
        { EnumDialogueType.Completion, DialogueTreeData.CompletionText },
        { EnumDialogueType.Ending, DialogueTreeData.EndingText }
    };

    private IEnumerator Start() {
        this.sphereVoidOverlay.SetFloat(this.sphereVoidCutoffHeightProperty, -2f);
        yield return new WaitForSeconds(3f);

        CanvasController.Instance.StartNextConversation(EnumDialogueType.Scan);
    }

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

    public int GetDialogueLength(EnumDialogueType dialogueType) {
        return DialogueLines[dialogueType].Length;
    }

    public IEnumerator GameWin() {
        CanvasController.Instance.StartNextConversation(EnumDialogueType.Completion);
        CanvasController.Instance.DisableAllInteractionButtons();
        StartCoroutine(SimultateVoidIncrease());

        yield return new WaitUntil(() => CanvasController.Instance.IsInformationPanelActive == false);
        CanvasController.Instance.StartNextConversation(EnumDialogueType.Ending);
    }

    private IEnumerator SimultateVoidIncrease() {
        float currentVoidValue = 0f;
        
        while (currentVoidValue < 1f) {
            currentVoidValue += (Time.deltaTime * 0.1f);
            this.sphereVoidOverlay.SetFloat(this.sphereVoidCutoffHeightProperty, currentVoidValue);
            yield return null;
        }

        this.sphereVoidOverlay.SetFloat(this.sphereVoidCutoffHeightProperty, 3f);
    }
}