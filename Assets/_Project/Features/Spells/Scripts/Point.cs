using UnityEngine;

namespace Project.Spells.Scripts
{
    /// <summary>
    /// Captured directly from the VR Controller in 3D space.
    /// </summary>
    public struct WorldPoint
    {
        public Vector3 Position;
        public readonly int StrokeId;

        public WorldPoint(Vector3 position, int strokeId)
        {
            Position = position;
            StrokeId = strokeId;
        }
    }
    
    /// <summary>
    /// Flattened 2D representation of the player's drawing.
    /// Used as the "Raw Input" for the $Q algorithm.
    /// </summary>
    [System.Serializable]
    public struct GesturePoint
    {
        public Vector2 Pos;
        public readonly int StrokeId;

        public GesturePoint(Vector2 pos, int strokeId)
        {
            Pos = pos;
            StrokeId = strokeId;
        }

        public GesturePoint(float x, float y, int strokeId)
        {
            Pos = new Vector2(x, y);
            StrokeId = strokeId;
        }
    }

    /// <summary>
    /// Normalized, resampled, and LUT-ready point used by the $Q recognizer.
    /// </summary>
    public struct ProcessedPoint
    {
        public Vector2 Pos;
        public readonly int IntX, IntY;

        public ProcessedPoint(Vector2 pos, int intX, int intY)
        {
            Pos = pos;
            IntX = intX;
            IntY = intY;
        }

        public ProcessedPoint(float x, float y, int intX, int intY)
        {
            Pos = new Vector2(x, y);
            IntX = intX;
            IntY = intY;
        }
    }
}