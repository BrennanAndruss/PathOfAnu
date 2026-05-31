using _Project.Features.Spells.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public static class NRecognizer
    {
        /// <summary>
        /// Classifies a candidate gesture against a set of templates.
        /// Returns the class of the closest neighbor in the template set.
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="templates"></param>
        /// <param name="spellSettings"></param>
        /// <returns></returns>
        public static RecognitionResult Classify(Gesture candidate, Gesture[] templates, SpellSettings spellSettings)
        {
            float bestScore = float.MinValue;
            Gesture bestMatch = null;
            foreach (Gesture template in templates)
            {
                float distance = SequentialDistance(candidate.Points, template.Points);
                
                // Convert distance to normalized accuracy score
                float score = 1f - distance / Mathf.Sqrt(2f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = template;
                }
            }

            if (bestMatch == null)
                return new RecognitionResult(SpellType.Unknown, float.MaxValue);

            return new RecognitionResult(bestMatch.SpellType, bestScore);
        }

        /// <summary>
        /// Sequential point-by-point distance calculation between two lists of points
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <returns></returns>
        private static float SequentialDistance(ProcessedPoint[] points1, ProcessedPoint[] points2)
        {
            float totalDistance = 0.0f;

            for (int i = 0; i < points1.Length; i++)
            {
                totalDistance += Vector2.Distance(points1[i].Pos, points2[i].Pos);
            }
            
            // Return average variation per point
            return totalDistance / points1.Length;
        }
    }
}