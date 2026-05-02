using Assets.Scripts.Singleton;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : Singleton<CanvasController> {
    [SerializeField] private Button OneButton;
    [SerializeField] private Button TwoButton;
    [SerializeField] private Button ThreeButton;

    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;

    [SerializeField] private Button InformationPanelButton;
    [SerializeField] private TMP_Text DialogueText;

    private PillarController.EnumSegment currentSegment = PillarController.EnumSegment.None;

    // Information panel configurations
    private float informationTextSpeed = 0.02f;
    public bool IsInformationPanelActive { get; private set; } = false;


    private EnumDialogueType currentDialogueType = EnumDialogueType.None;
    private int currentDialogueIndex = 0;
    private Coroutine currentTypingCoroutine;

    private void OnEnable() {
        this.OneButton.onClick.AddListener(OnOneButton);
        this.TwoButton.onClick.AddListener(OnTwoButton);
        this.ThreeButton.onClick.AddListener(OnThreeButton);

        this.LeftButton.onClick.AddListener(OnLeftButton);
        this.RightButton.onClick.AddListener(OnRightButton);

        this.InformationPanelButton.onClick.AddListener(OnInformationPanelButton);

        GameManager.TapDetected += OnTap;
    }

    private void OnDisable() {
        this.OneButton.onClick.RemoveListener(OnOneButton);
        this.TwoButton.onClick.RemoveListener(OnTwoButton);
        this.ThreeButton.onClick.RemoveListener(OnThreeButton);

        this.LeftButton.onClick.RemoveListener(OnLeftButton);
        this.RightButton.onClick.RemoveListener(OnRightButton);

        this.InformationPanelButton.onClick.RemoveListener(OnInformationPanelButton);

        GameManager.TapDetected -= OnTap;
    }

    protected override void Awake() {
        base.Awake(); // Singelton Awake call
        DisableAllInteractionButtons();
        DisableInformationPanel();
    }

    private void OnOneButton() => this.currentSegment = PillarController.EnumSegment.Top;
    private void OnTwoButton() => this.currentSegment = PillarController.EnumSegment.Middle;
    private void OnThreeButton() => this.currentSegment = PillarController.EnumSegment.Bottom;

    private void OnLeftButton() {
        if (GameManager.Instance.CurrentTaps != 3) return;
        if (this.currentSegment == PillarController.EnumSegment.None) return;
        GameManager.Instance.MoveSegment(this.currentSegment, true);
    }

    private void OnRightButton() {
        if (GameManager.Instance.CurrentTaps != 3) return;
        if (this.currentSegment == PillarController.EnumSegment.None) return;
        GameManager.Instance.MoveSegment(this.currentSegment, false);
    }
    private void OnTap() {
        if (GameManager.Instance.CurrentTaps == GameManager.Instance.MaxTaps) {
            EnableAllInteracitonButtons();
        }
    }
    private void OnInformationPanelButton() {
        if (this.IsInformationPanelActive) ShowDialog();
    }
    private void EnableAllInteracitonButtons() {
        this.OneButton.gameObject.SetActive(true);
        this.TwoButton.gameObject.SetActive(true);
        this.ThreeButton.gameObject.SetActive(true);
        this.LeftButton.gameObject.SetActive(true);
        this.RightButton.gameObject.SetActive(true);
    }
    private void DisableAllInteractionButtons() {
        this.OneButton.gameObject.SetActive(false);
        this.TwoButton.gameObject.SetActive(false);
        this.ThreeButton.gameObject.SetActive(false);
        this.LeftButton.gameObject.SetActive(false);
        this.RightButton.gameObject.SetActive(false);
    }

    private void DisableInformationPanel() {
        this.InformationPanelButton.gameObject.SetActive(false);
        this.IsInformationPanelActive = false;
    }

    private void EnableInformationPanel() {
        this.InformationPanelButton.gameObject.SetActive(true);
        this.IsInformationPanelActive = true;
    }

    private void ShowDialog() {
        if (!this.IsInformationPanelActive) {
            Debug.LogWarning("Trying to show dialog while information panel is not active.");
            return;
        }

        string[] currentConversation = GameManager.DialogueLines[this.currentDialogueType];

        // If the typing coroutine is still running, stop it and finish the sentence instantly
        if (this.currentTypingCoroutine != null) {
            StopCoroutine(this.currentTypingCoroutine);
            this.currentTypingCoroutine = null;

            // Set the dialogue text to the full line immediately
            this.DialogueText.text = currentConversation[this.currentDialogueIndex];
        } else { // The previous line has finished typing, so move to the next

            // Check if there are still lines left in the conversation
            if (this.currentDialogueIndex + 1 < currentConversation.Length) {
                // Determine if the next string we're about to play is the final line in the conversation
                bool isLastLine = (this.currentDialogueIndex + 2 == currentConversation.Length);

                PlayNextLine(currentConversation[this.currentDialogueIndex + 1], isLastLine);
            } else {
                // Conversation is over, close panel
                DisableInformationPanel();
            }
        }
    }

    private void PlayNextLine(string line, bool isLastLine) {
        this.currentDialogueIndex++;
        this.currentTypingCoroutine = StartCoroutine(TypeText(line, isLastLine));
    }
    private IEnumerator TypeText(string fullText, bool isLastLine) {
        this.DialogueText.text = "";
        foreach (char c in fullText) {
            this.DialogueText.text += c;
            yield return new WaitForSeconds(this.informationTextSpeed);
        }
        this.currentTypingCoroutine = null;

    }

    /// <summary>
    /// Starts a new conversation sequence using the specified dialogue type, resetting the current dialogue index and
    /// displaying the corresponding dialog.
    /// </summary>
    /// <remarks>If the information panel is not currently active, this method enables it before displaying
    /// the dialog. This method replaces any ongoing conversation.</remarks>
    /// <param name="newDialog">The dialogue type to use for the new conversation sequence.</param>
    public void StartNextConversation(EnumDialogueType newDialog) {
        this.currentDialogueType = newDialog;
        this.currentDialogueIndex = -1; // -1 so the first call to ShowDialog increments it to 0

        // If the typing coroutine is still running from a previous conversation, stop it
        if (this.currentTypingCoroutine != null) {
            StopCoroutine(this.currentTypingCoroutine);
            this.currentTypingCoroutine = null;
        }

        if (!this.IsInformationPanelActive) EnableInformationPanel();
        ShowDialog();
    }
}
