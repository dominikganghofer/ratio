using System;
using PCAD.Helper;
using PCAD.Model;
using PCAD.UI;
using PCAD.UserInput;
using UnityEngine;

namespace PCAD.Logic
{
    /// <summary>
    /// This static class holds all the logic for the drawing tool. 
    /// </summary>
    public static class Drawing
    {
        public static void Update(ref PCad.Model model, CoordinateSystemUI coordinateSystemUI,
            bool isMouseOnDrawArea, Action saveToHistory)
        {
            model.InteractionState.hoveredCoordinate =
                CoordinateManipulation.TryGetCoordinateAtPosition(coordinateSystemUI);

            NumpadInput.UpdateNumpadInput(ref model.InteractionState.keyboardInputModel,
                model.Sketch.coordinateSystem.GetAllParameters()
            );

            model.InteractionState.focusPosition = CoordinateCreation.UpdateCursorPosition(
                model.InteractionState.focusPosition,
                model.Sketch.coordinateSystem,
                model.InteractionState.keyboardInputModel
            );

            if (model.InteractionState.focusPosition == null)
            {
                Debug.LogError($"Focus Position should always be != null if state == DrawRectangles");
                return;
            }

            switch (model.InteractionState.CurrentMouseState)
            {
                case MouseInput.MouseState.None:
                case MouseInput.MouseState.PrimaryHold:
                case MouseInput.MouseState.PrimaryUp:
                    break;
                case MouseInput.MouseState.PrimaryDown:
                    if (isMouseOnDrawArea || Input.GetKeyDown(KeyCode.Return))
                    {
                        AddPointToDrawing(ref model, saveToHistory);
                    }

                    break;
                case MouseInput.MouseState.SetAnchorDown:
                    var mousePosition = MouseInput.RaycastPosition;
                    model.Sketch.coordinateSystem.SetAnchorPosition(mousePosition);
                    break;
                case MouseInput.MouseState.DeleteDown:
                    CoordinateCreation.DeletePositionAtMousePosition(model.Sketch.coordinateSystem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateGeometry(ref model.InteractionState.incompleteGeometry, model.InteractionState.focusPosition);
        }

        public static void AddPointToDrawing(ref PCad.Model model, Action saveToHistory)
        {
            CoordinateCreation.BakePosition(model.InteractionState.focusPosition);
            model.Sketch.coordinateSystem.SetAnchorPosition(MouseInput.RaycastPosition);

            switch (model.Tool.CurrentGeometryType)
            {
                case GeometryType.Point:
                    model.Sketch.geometries.Add(PointCreation.NewPoint(model.InteractionState.focusPosition));
                    saveToHistory();
                    break;
                case GeometryType.Line:
                    if (!(model.InteractionState.incompleteGeometry is LineModel))
                    {
                        model.InteractionState.incompleteGeometry =
                            LineCreation.StartNewLine(model.InteractionState.focusPosition);
                        model.Sketch.geometries.Add(model.InteractionState.incompleteGeometry);
                    }
                    else
                    {
                        LineCreation.CompleteLine(model.InteractionState.incompleteGeometry as LineModel,
                            model.InteractionState.focusPosition);
                        model.InteractionState.incompleteGeometry = null;
                        saveToHistory();
                    }

                    break;

                case GeometryType.Rectangle:
                    if (!(model.InteractionState.incompleteGeometry is RectangleModel))
                    {
                        model.InteractionState.incompleteGeometry =
                            RectangleCreation.StartNewRectangle(model.InteractionState.focusPosition,
                                model.Tool.CurrentGeometryColor);
                        model.Sketch.geometries.Add(model.InteractionState.incompleteGeometry);
                    }
                    else
                    {
                        RectangleCreation.CompleteRectangle(model.InteractionState.incompleteGeometry as RectangleModel,
                            model.InteractionState.focusPosition);
                        model.InteractionState.incompleteGeometry = null;
                        saveToHistory();
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // reset input
            model.InteractionState.keyboardInputModel = new NumpadInput.Model();
        }

        public static void CleanUpIncompleteGeometry(ref PCad.InteractionState interactionState,
            ref SketchModel sketchModel)
        {
            if (interactionState.incompleteGeometry == null)
                return;

            var incompleteGeometry = interactionState.incompleteGeometry;
            interactionState.incompleteGeometry.P0.ForEach(c =>
                c.UnregisterGeometryAndTryToDelete(incompleteGeometry));

            sketchModel.geometries.Remove(interactionState.incompleteGeometry);
            interactionState.incompleteGeometry = null;
        }

        public static void CleanUpFocusPosition(ref PCad.InteractionState interactionState)
        {
            if (interactionState.focusPosition == null)
                return;

            interactionState.focusPosition.ForEach(c =>
            {
                if (c.IsCurrentlyDrawn) c.Delete();
            });
            interactionState.focusPosition = null;
        }

        private static void UpdateGeometry(ref GeometryModel incompleteGeometry, Vec<Coordinate> focusPosition)
        {
            switch (incompleteGeometry)
            {
                case RectangleModel rectangleModel:
                    RectangleCreation.UpdateRectangle(rectangleModel, focusPosition);
                    break;
                case LineModel lineModel:
                    LineCreation.UpdateLine(lineModel, focusPosition);
                    break;
            }
        }
    }
}