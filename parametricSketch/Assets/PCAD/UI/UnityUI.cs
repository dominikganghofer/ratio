using System;
using PCAD.Logic;
using PCAD.UserInput;
using UnityEngine;
using UnityEngine.Serialization;

namespace PCAD.UI
{
    public class UnityUI : MonoBehaviour
    {
        [SerializeField] private SketchStyle _sketchStyle;

        [FormerlySerializedAs("coordinateSystemUI")] public CoordinateSystemUI CoordinateSystemUI;
        public GeometryUIPool GeometryUI;
        public ParameterUI ParameterUI;
        public CursorUI CursorUI;
        public ControlPanel ControlPanel;
        public ClickableImage DrawingCanvas;

        public void InitializeControlPanel(ToolConfiguration toolConfiguration, Action<Command> handleCommand)
        {
            ControlPanel.Initialize(handleCommand);
            ControlPanel.UpdateUI(toolConfiguration);        }

        public void InitializeCoodinateSystem()
        {
            CoordinateSystemUI.Initialize();
        }

        public void UpdateUI(PCad.Model model)
        {
            CoordinateSystemUI.UpdateUI(
                model.Sketch.coordinateSystem,
                _sketchStyle.CoordinateUIStyle,
                model.InteractionState.keyboardInputModel,
                model.InteractionState.draggedCoordinate,
                model.InteractionState.hoveredCoordinate);

            GeometryUI.UpdateUI(model.Sketch.geometries, _sketchStyle._geometryStyleAsset.Set);
            ParameterUI.UpdateUI(model.Sketch.coordinateSystem.GetAllParameters());
            ControlPanel.UpdateUI(model.Tool);

            if (model.Tool.ActiveTool == ToolConfiguration.Tool.Transform)
                CursorUI.UpdateCursor(model.InteractionState.hoveredCoordinate);
            else
                CursorUI.ResetCursor();
        }
    }
}