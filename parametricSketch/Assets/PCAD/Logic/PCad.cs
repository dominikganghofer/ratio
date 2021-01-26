using System;
using System.Collections.Generic;
using PCAD.Helper;
using PCAD.Model;
using PCAD.UI;
using PCAD.UserInput;
using UnityEngine;

namespace PCAD.Logic
{
    /// <summary>
    /// The Main controller. Holds the <see cref="SketchModel"/>, <see cref="ToolConfiguration"/> and
    /// <see cref="InteractionState"/>. Connects all components of the app.
    /// </summary>
    public class PCad : MonoBehaviour
    {
        [SerializeField] private UnityUI _ui;

        private Model _model;
        private History _history;
        private static bool IsMouseOnDrawArea() => Input.mousePosition.x > 30;

        public struct Model
        {
            public SketchModel Sketch;
            public ToolConfiguration Tool;
            public InteractionState InteractionState;

            public void Initialize(Vec<float> mousePositionAsOrigin)
            {
                Sketch.coordinateSystem = new CoordinateSystem(mousePositionAsOrigin);
                Sketch.geometries = new List<GeometryModel>();
                InteractionState.keyboardInputModel = new NumpadInput.Model();
                Tool = new ToolConfiguration()
                {
                    ActiveTool = ToolConfiguration.Tool.Drawing,
                    CurrentGeometryType = GeometryType.Rectangle,
                    CurrentGeometryColor = GeometryStyleAsset.GeometryColor.White,
                };
                InteractionState.focusPosition = CoordinateCreation.UpdateCursorPosition(
                    InteractionState.focusPosition,
                    Sketch.coordinateSystem,
                    InteractionState.keyboardInputModel
                );
            }
        }

        private void Start()
        {
            // initialize control panel on start. everything else is initialized when the first point is defined
            _ui.InitializeControlPanel(_model.Tool, HandleCommand);
        }

        private void InitializeSketch(Vec<float> mousePositionAsOrigin)
        {
            _model.Initialize(mousePositionAsOrigin);
            _ui.InitializeCoodinateSystem();

            _history = new History(HistoryPositionChangedHandler);
            Drawing.AddPointToDrawing(ref _model, SaveCurrentStateToHistory);
        }

        private void Update()
        {
            var hasBeenInitialized = _model.Sketch.coordinateSystem != null;

            // do not start drawing if first click is on command panel 
            if (!_ui.DrawingCanvas.IsPointerOnCanvas)
            {
                if (hasBeenInitialized)
                    _ui.UpdateUI(_model);
                return;
            }

            // wait until the origin is defined by first click on canvas
            if (!hasBeenInitialized)
            {
                if (_model.InteractionState.CurrentMouseState == MouseInput.MouseState.PrimaryDown &&
                    IsMouseOnDrawArea())
                    InitializeSketch(MouseInput.RaycastPosition);
                return;
            }

            var hotKeyCommand = HotKeyInput.Update();
            if (hotKeyCommand.HasValue)
                HandleCommand(hotKeyCommand.Value);

            switch (_model.Tool.ActiveTool)
            {
                case ToolConfiguration.Tool.Transform:
                    Transformation.Update(ref _model.InteractionState, ref _model.Sketch, _ui.CoordinateSystemUI,
                        IsMouseOnDrawArea(), SaveCurrentStateToHistory);
                    break;

                case ToolConfiguration.Tool.Drawing:
                    Drawing.Update(ref _model, _ui.CoordinateSystemUI, IsMouseOnDrawArea(), SaveCurrentStateToHistory);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _ui.UpdateUI(_model);
        }


        private void HistoryPositionChangedHandler(SketchModel.Serialization serializationToSet)
        {
            _model.Sketch.SetSerialization(serializationToSet);
            _model.InteractionState.Reset();
        }

        private void SaveCurrentStateToHistory()
        {
            _history.AddToHistory(_model.Sketch.Serialize());
        }

        private void SetState(ToolConfiguration.Tool newTool)
        {
            switch (newTool)
            {
                case ToolConfiguration.Tool.Transform:
                    Drawing.CleanUpFocusPosition(ref _model.InteractionState);
                    Drawing.CleanUpIncompleteGeometry(ref _model.InteractionState, ref _model.Sketch);
                    break;
                case ToolConfiguration.Tool.Drawing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newTool), newTool, null);
            }

            _model.Tool.ActiveTool = newTool;
        }

        private void HandleCommand(Command buttonType)
        {
            switch (buttonType)
            {
                case Command.Transform:
                    SetState(ToolConfiguration.Tool.Transform);
                    break;
                case Command.Undo:
                    _model.Sketch.SetSerialization(_history.Undo());
                    _model.InteractionState.Reset();
                    break;
                case Command.Redo:
                    _model.Sketch.SetSerialization(_history.Redo());
                    _model.InteractionState.Reset();
                    break;
                case Command.DrawPoint:
                    SetState(ToolConfiguration.Tool.Drawing);
                    _model.Tool.CurrentGeometryType = GeometryType.Point;
                    Drawing.CleanUpIncompleteGeometry(ref _model.InteractionState, ref _model.Sketch);
                    break;
                case Command.DrawLine:
                    SetState(ToolConfiguration.Tool.Drawing);
                    _model.Tool.CurrentGeometryType = GeometryType.Line;
                    Drawing.CleanUpIncompleteGeometry(ref _model.InteractionState, ref _model.Sketch);
                    break;
                case Command.DrawRect:
                    SetState(ToolConfiguration.Tool.Drawing);
                    _model.Tool.CurrentGeometryType = GeometryType.Rectangle;
                    Drawing.CleanUpIncompleteGeometry(ref _model.InteractionState, ref _model.Sketch);
                    break;
                case Command.ColorBlack:
                    _model.Tool.CurrentGeometryColor = GeometryStyleAsset.GeometryColor.Black;
                    break;
                case Command.ColorGrey:
                    _model.Tool.CurrentGeometryColor = GeometryStyleAsset.GeometryColor.Grey;
                    break;
                case Command.ColorWhite:
                    _model.Tool.CurrentGeometryColor = GeometryStyleAsset.GeometryColor.White;
                    break;
                case Command.Help:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null);
            }
        }

        [Serializable]
        public struct InteractionState
        {
            public Coordinate draggedCoordinate;
            public Vec<Coordinate> focusPosition;
            public NumpadInput.Model keyboardInputModel;
            public GeometryModel incompleteGeometry;
            public (Coordinate coordinate, Vec.AxisID axis)? hoveredCoordinate;
            public MouseInput.MouseState CurrentMouseState => MouseInput.CurrentMouseState();

            public void Reset()
            {
                draggedCoordinate = null;
                keyboardInputModel = new NumpadInput.Model();
                incompleteGeometry = null;
                focusPosition = null;
            }
        }
    }
}