using _Project.Features.Spells.ScriptableObjects;

namespace _Project.Features.Spells.Scripts
{
    public class Gesture
    {
        public readonly string Name;
        public readonly SpellType SpellType;
        public readonly int StrokeCount;
        public readonly ProcessedPoint[] Points;
        public readonly int[] Lut;

        public Gesture(GesturePoint[] points, string name, SpellSettings settings)
        {
            this.Name = name;
            this.SpellType = SpellType.Unknown;
            this.StrokeCount = points[^1].StrokeId;
            this.Points = QProcessor.Normalize(points, settings);
            this.Lut = QProcessor.ConstructLut(Points, settings);
        }

        public Gesture(GestureData template, SpellSettings settings)
        {
            this.Name = template.name;
            this.SpellType = template.spellType;
            this.StrokeCount = template.strokeCount;
            this.Points = QProcessor.Normalize(template.points, settings);
            this.Lut = QProcessor.ConstructLut(Points, settings);
        }
    }
}