using System;
using System.Collections.Generic;
using PCAD.Logic;
using PCAD.Model;
using PCAD.UserInput;
using UnityEngine;

namespace PCAD.UI
{
    /// <summary>
    /// Holds a list of <see cref="ControlPanelButton"/>s that are mapped onto <see cref="Command"/>s.
    /// </summary>
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] private List<ControlPanelButton> _buttons = new List<ControlPanelButton>();

        public void Initialize(Action<Command> buttonClickedCallback)
        {
            foreach (var b in _buttons)
            {
                b.Initialize(buttonClickedCallback);
            }
        }

        [ContextMenu("RenameGameObjects")]
        private void RenameGameObjects()
        {
            foreach (var button in _buttons)
            {
                button.gameObject.name = button.ButtonType.ToString() + " - Button";
            }
        }
        
        public void UpdateUI(ToolConfiguration model)
        {
            var buttonStates = new Dictionary<Command, bool>
            {
                {Command.Help, false},
                {Command.Redo, false},
                {Command.Undo, false},
                {Command.Transform, model.ActiveTool == ToolConfiguration.Tool.Transform},
                {
                    Command.DrawPoint,
                    model.ActiveTool == ToolConfiguration.Tool.Drawing && model.CurrentGeometryType == GeometryType.Point
                },
                {
                    Command.DrawLine,
                    model.ActiveTool == ToolConfiguration.Tool.Drawing && model.CurrentGeometryType == GeometryType.Line
                },
                {
                    Command.DrawRect,
                    model.ActiveTool == ToolConfiguration.Tool.Drawing && model.CurrentGeometryType == GeometryType.Rectangle
                },
                {Command.ColorBlack, model.CurrentGeometryColor == GeometryStyleAsset.GeometryColor.Black},
                {Command.ColorGrey, model.CurrentGeometryColor == GeometryStyleAsset.GeometryColor.Grey},
                {Command.ColorWhite, model.CurrentGeometryColor == GeometryStyleAsset.GeometryColor.White}
            };

            foreach (var b in _buttons)
            {
                b.UpdateUI(buttonStates[b.ButtonType]);
            }
        }
    }
}