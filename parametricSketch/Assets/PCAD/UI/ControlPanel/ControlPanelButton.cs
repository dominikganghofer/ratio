using System;
using PCAD.UserInput;
using UnityEngine;
using UnityEngine.UI;

namespace PCAD.UI
{
    public class ControlPanelButton : MonoBehaviour
    {
        [SerializeField] private Image _selectedImage;
        [SerializeField] private Button _button;
        [SerializeField] public Command ButtonType;
        
        public void Initialize(Action<Command> ButtonClickCallback)
        {
            _button.onClick.AddListener(() => ButtonClickCallback(ButtonType));
        }

        public void UpdateUI(bool isSelected)
        {
            _selectedImage.enabled = isSelected;
        }
    }
}