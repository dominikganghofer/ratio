using UnityEngine;
using UnityEngine.EventSystems;

namespace PCAD.UI
{
    /// <summary>
    /// Can be added on a ui image to find out if it is currently hovered.
    /// </summary>
    public class ClickableImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsPointerOnCanvas;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            IsPointerOnCanvas = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            IsPointerOnCanvas = false;
        }
    }
}
