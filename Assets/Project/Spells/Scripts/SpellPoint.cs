using UnityEngine;

namespace Project.Spells.Scripts
{
    public struct RawSpellPoint
    {
        public Vector3 Position;
        public int StrokeId;

        public RawSpellPoint(Vector3 position, int strokeId)
        {
            Position = position;
            StrokeId = strokeId;
        }
    }
    
    public struct SpellPoint
    {
        public float X, Y;
        public int StrokeId;

        public SpellPoint(float x, float y, int strokeId)
        {
            X = x;
            Y = y;
            StrokeId = strokeId;
        }
    }

    public struct NormalizedSpellPoint
    {
        public float X, Y;
        public int StrokeId;
        public int IntX, IntY;

        public NormalizedSpellPoint(float x, float y, int strokeId)
        {
            X = x;
            Y = y;
            StrokeId = strokeId;
            IntX = 0;
            IntY = 0;
        }
    }
}