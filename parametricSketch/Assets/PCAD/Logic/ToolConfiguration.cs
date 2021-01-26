using PCAD.Model;
using PCAD.UI;

namespace PCAD.Logic
{
    /// <summary>
    /// Holds the info for the active tool.
    /// </summary>
    public struct ToolConfiguration
    {
        public Tool ActiveTool;
        public GeometryType CurrentGeometryType;
        public GeometryStyleAsset.GeometryColor CurrentGeometryColor;

        public enum Tool
        {
            Transform,
            Drawing,
        }
    }
}