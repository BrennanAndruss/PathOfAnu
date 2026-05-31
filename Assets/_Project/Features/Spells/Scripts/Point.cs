using UnityEngine;

namespace _Project.Features.Spells.Scripts
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
    /// Used as the "Raw Input" for sequential recognizer algorithms.
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
    /// Normalized, resampled point used by sequential gesture recognizers.
    /// </summary>
    public struct ProcessedPoint
    {
        public Vector2 Pos;
        public readonly int IntX, IntY; // Integer coordinates for $Q's LUT grid
        public readonly int StrokeId; // ID for multi-stroke sequence checking
        
        public ProcessedPoint(Vector2 pos, int strokeId, int intX = 0, int intY = 0)
        {
            Pos = pos;
            IntX = intX;
            IntY = intY;
            StrokeId = strokeId;
        }

        public ProcessedPoint(float x, float y, int strokeId, int intX = 0, int intY = 0)
        {
            Pos = new Vector2(x, y);
            IntX = intX;
            IntY = intY;
            StrokeId = strokeId;
        }
    }
}