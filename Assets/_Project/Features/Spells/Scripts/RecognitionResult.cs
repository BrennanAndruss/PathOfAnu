namespace _Project.Features.Spells.Scripts
{
    public struct RecognitionResult
    {
        public readonly SpellType SpellType;
        public readonly float Confidence;

        public RecognitionResult(SpellType type, float confidence)
        {
            SpellType = type;
            Confidence = confidence;
        }
    }
}