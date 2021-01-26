using PCAD.Model;
using UnityEngine;

namespace PCAD.UI
{
    /// <summary>
    /// The ui for an <see cref="Anchor"/>.
    /// </summary>
    public class AnchorUI : MonoBehaviour
    {
        [SerializeField] CircleGizmo _primaryAnchorUI;
        [SerializeField] CircleGizmo _secondaryAnchorUI;

        public void UpdateUI(Anchor anchor, CoordinateUIStyle.AnchorStyle anchorStyle)
        {
            _primaryAnchorUI.UpdateUI(
                anchor.PrimaryPosition,
                anchorStyle.CircleStyle.Radius,
                anchorStyle.CircleStyle.InnerRadius,
                anchorStyle.CircleStyle.Width,
                anchorStyle.CircleStyle.PrimaryColor.Value);

            _secondaryAnchorUI.UpdateUI(
                anchor.SecondaryPosition,
                anchorStyle.CircleStyle.Radius,
                anchorStyle.CircleStyle.InnerRadius,
                anchorStyle.CircleStyle.Width,
                anchorStyle.CircleStyle.SecondaryColor.Value);
        }
    }
}