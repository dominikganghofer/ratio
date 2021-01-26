using PCAD.Helper;
using UnityEngine;

namespace PCAD.UserInput
{
    public static class MouseInput
    {
        public static Vec<float> RaycastPosition
        {
            get
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                return Physics.Raycast(ray, out var hit)
                    ? new Vec<float>(hit.point.x, hit.point.y, hit.point.z)
                    : new Vec<float>(0f);
            }
        }

        public static MouseState CurrentMouseState()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                return MouseState.PrimaryDown;
            if (Input.GetKey(KeyCode.Mouse0))
                return MouseState.PrimaryHold;
            if (Input.GetKeyUp(KeyCode.Mouse0))
                return MouseState.PrimaryUp;
            if (Input.GetKeyUp(KeyCode.Mouse1))
                return MouseState.SetAnchorDown;
            if (Input.GetKeyUp(KeyCode.Mouse2))
                return MouseState.DeleteDown;
            return MouseState.None;
        }

        public enum MouseState
        {
            None,
            PrimaryDown,
            PrimaryHold,
            PrimaryUp,
            SetAnchorDown,
            DeleteDown
        }
    }
}