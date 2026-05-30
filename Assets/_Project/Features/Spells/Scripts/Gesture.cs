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
            // this.Points = NProcessor.Normalize(points, settings);
            this.Points = QProcessor.Normalize(points, settings);
            this.Lut = QProcessor.ConstructLut(Points, settings);
        }

        public Gesture(SpellData spellData, SpellSettings settings)
        {
            this.Name = spellData.spellType.ToString();
            this.SpellType = spellData.spellType;
            this.StrokeCount = spellData.gestureData.strokeCount;
            // this.Points = NProcessor.Normalize(spellData.gestureData.Points, settings);
            this.Points = QProcessor.Normalize(spellData.gestureData.Points, settings);
            this.Lut = QProcessor.ConstructLut(Points, settings);
        }
    }
}