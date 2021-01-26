using PCAD.Helper;
using PCAD.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace PCAD.UI
{
    /// <summary>
    /// A cursor with states for horizontal and vertical manipulation. 
    /// </summary>
    [CreateAssetMenu(fileName = "cursorUI", menuName = "parametricSketch/CursorUI", order = 1)]
    public class CursorUI : ScriptableObject
    {
        public Texture2D VerticalManipulationCursor;
        public Texture2D HorizontalManipulationCursor;

        [FormerlySerializedAs("cursorMode")] public CursorMode CursorMode = CursorMode.Auto;
        [FormerlySerializedAs("hotSpot")] public Vector2 HotSpot = Vector2.zero;

        public void UpdateCursor((Coordinate coordinate, Vec.AxisID axis)? hitResult)
        {
            if (hitResult.HasValue)
            {
                var cursor = hitResult.Value.axis == Vec.AxisID.X
                    ? HorizontalManipulationCursor
                    : VerticalManipulationCursor;
                Cursor.SetCursor(cursor, HotSpot, CursorMode);
            }
            else
            {
                Cursor.SetCursor(null, HotSpot, CursorMode);
            }
        }

        public void ResetCursor()
        {
            Cursor.SetCursor(null, HotSpot, CursorMode);
        }
    }
}