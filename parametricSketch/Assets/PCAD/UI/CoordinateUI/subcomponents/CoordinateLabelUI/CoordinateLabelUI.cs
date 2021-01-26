using PCAD.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PCAD.UI
{
    /// <summary>
    /// The label of a <see cref="CoordinateUI"/>.
    /// </summary>
    public class CoordinateLabelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private Image _labelBackground = null;

        public void UpdateUI(string labelString, Vector3 labelPositionWorld,
            CoordinateUIStyle.LabelStyle style, CoordinateUIStyle.ColorSet colors, SketchStyle.State state)
        {
            _label.text = labelString;
            _label.color = colors.GetForState(state).Value;
            _label.fontSize = style.FontSize;
            _labelBackground.color = style.LabelBackground.Value;
        
            var anchoredPosition =
                WorldScreenTransformationHelper.WorldToUISpace(GetComponentInParent<Canvas>(), labelPositionWorld);
            GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        }
    }
}