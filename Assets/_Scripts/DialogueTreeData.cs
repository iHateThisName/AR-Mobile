public enum EnumDialogueType {
    None,
    Scan,
    FirstCrack,
    RiftOpened,
    Obelisk,
    Completion,
    Ending
}

public static class DialogueTreeData {

    // Player starts the game (scanning phase)
    public static readonly string[] ScanEnvironmentText = new string[] {
        "…You are blind.",
        "Move. Let me see through you.",
        "Trace the surface. Reveal the ground beneath your feet."
    };

    // First taps (cracks begin)
    public static readonly string[] FirstCrackText = new string[] {
        "Yes… I feel it.",
        "The surface fractures.",
        "Again.",
        "Break it open."
    };

    // Rift opening (second crack)
    public static readonly string[] RiftOpenedText = new string[] {
        "There… a wound in your world.",
        "Do you see it?",
        "Do not stop now."
    };

    // Obelisk appears (It has moved out of the crack)
    public static readonly string[] ObeliskText = new string[] {
        "The key emerges.",
        "Ancient. Bound. Misaligned.",
        "Turn it.",
        "Set it right.. and all will be well."
    };

    // Puzzle completed
    public static readonly string[] CompletionText = new string[] {
        "You have done it.",
        "The seal weakens.",
        "Your world was never meant to hold me.",
        "Now.. watch it fade."
    };

    // Ending / void consumes everything
    public static readonly string[] EndingText = new string[] {
        "There is no ground.",
        "No sky.",
        "Only me."
    };
}