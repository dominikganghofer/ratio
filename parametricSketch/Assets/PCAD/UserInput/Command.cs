namespace PCAD.UserInput{

    /// <summary>
    /// Used by the <see cref="PCAD.UI.ControlPanel"/> and the <see cref="HotKeyInput"/>.
    /// </summary>
    public enum Command
    {
        Transform, 
        Undo,
        Redo,
        DrawPoint,
        DrawLine,
        DrawRect,
        ColorBlack,
        ColorGrey,
        ColorWhite,
        Help,
    }
}