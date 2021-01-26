using System;
using PCAD.Helper;
using PCAD.Model;
using PCAD.UI;
using PCAD.UserInput;

namespace PCAD.Logic
{
    /// <summary>
    /// This static class holds all the logic for the transformation tool. 
    /// </summary>
    public static class Transformation
    {
        public static void Update(ref PCad.InteractionState interactionState, ref SketchModel sketchModel,
            CoordinateSystemUI coordinateSystemUI, bool isMouseOnDrawArea, Action saveToHistory)

        {
            UpdateHoveredCoordinate(ref interactionState, coordinateSystemUI);

            switch (interactionState.CurrentMouseState)
            {
                case MouseInput.MouseState.None:
                case MouseInput.MouseState.SetAnchorDown:
                case MouseInput.MouseState.DeleteDown:
                    break;
                case MouseInput.MouseState.PrimaryDown:
                    if (interactionState.hoveredCoordinate.HasValue && isMouseOnDrawArea)
                        StartDrag(ref interactionState, coordinateSystemUI);
                    break;
                case MouseInput.MouseState.PrimaryHold:
                    if(interactionState.draggedCoordinate != null)
                        UpdateDrag(ref interactionState, ref sketchModel);
                    break;
                case MouseInput.MouseState.PrimaryUp:
                    if( interactionState.draggedCoordinate != null)
                    {
                        saveToHistory();
                        interactionState.draggedCoordinate = null;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void UpdateHoveredCoordinate(ref PCad.InteractionState interactionState,
            CoordinateSystemUI coordinateSystemUI)
        {
            interactionState.hoveredCoordinate =
                CoordinateManipulation.TryGetCoordinateAtPosition(coordinateSystemUI);

            if (interactionState.hoveredCoordinate.HasValue
                && !(interactionState.hoveredCoordinate.Value.coordinate is Mue))
            {
                // quick fix: only mue coordinates can be transformed
                interactionState.hoveredCoordinate = null;
            }
        }

        private static void StartDrag(ref PCad.InteractionState interactionState,
            CoordinateSystemUI coordinateSystemUI)
        {
            interactionState.draggedCoordinate = interactionState.hoveredCoordinate.Value.coordinate;
            CoordinateManipulation.TryGetCoordinateAtPosition(coordinateSystemUI);
        }

        private static void UpdateDrag(ref PCad.InteractionState interactionState, ref SketchModel sketchModel)
        {
            var (value, pointsInNegativeDirection) = CoordinateManipulation.UpdateDrag(
                interactionState.draggedCoordinate,
                sketchModel.coordinateSystem.AxisThatContainsCoordinate(interactionState.draggedCoordinate));

            interactionState.draggedCoordinate.Parameter.Value = value;
            //quick fix: for now, only mue coordinates can be dragged
            ((Mue) interactionState.draggedCoordinate).PointsInNegativeDirection =
                pointsInNegativeDirection;
        }
    }
}