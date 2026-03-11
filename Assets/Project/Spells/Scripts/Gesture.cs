namespace Project.Spells.Scripts
{
    public class Gesture
    {
        public string Name;
        public ProcessedPoint[] Points;
        public int[] Lut;

        public Gesture(GesturePoint[] points, string name, SpellSettings settings)
        {
            this.Name = name;
            this.Points = QProcessor.Normalize(points, settings);
            this.Lut = QProcessor.ConstructLut(Points, settings);
        }
    }
}