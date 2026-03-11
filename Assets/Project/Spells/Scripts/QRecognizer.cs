using Unity.Mathematics;
using UnityEngine;

namespace Project.Spells.Scripts
{
    public struct RecognitionResult
    {
        public readonly string Name;
        public readonly float Confidence;

        public RecognitionResult(string name, float confidence)
        {
            Name = name;
            Confidence = confidence;
        }
    }
    
    public static class QRecognizer
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
            float minDist = float.MaxValue;
            string gestureClass = string.Empty;
            foreach (Gesture template in templates)
            {
                float dist = GreedyCloudMatch(candidate, template, minDist, spellSettings);
                if (dist < minDist)
                {
                    minDist = dist;
                    gestureClass = template.Name;
                }
            }

            return new RecognitionResult(gestureClass, minDist);
        }

        /// <summary>
        /// Implements greedy search for a minimum-distance matching between two point clouds.
        /// Implements Early Abandoning and Lower Bounding (LUT) optimizations.
        /// </summary>
        /// <param name="gesture1"></param>
        /// <param name="gesture2"></param>
        /// <param name="currentMin"></param>
        /// <param name="spellSettings"></param>
        /// <returns></returns>
        private static float GreedyCloudMatch(Gesture gesture1, Gesture gesture2,
            float currentMin, SpellSettings spellSettings)
        {
            int n = gesture1.Points.Length;

            // Control the number of greedy search trials
            float eps = 0.5f; // range [0,1]
            int step = (int)math.floor(math.pow(n, 1.0f - eps));

            if (spellSettings.useLowerBounding)
            {
                // Compute lower bounds and direction of matching in both directions
                float[] lowerBound1 =
                    ComputeLowerBound(gesture1.Points, gesture2.Points, gesture2.Lut, step, spellSettings);
                float[] lowerBound2 =
                    ComputeLowerBound(gesture2.Points, gesture1.Points, gesture1.Lut, step, spellSettings);
                for (int i = 0, indexLB = 0; i < n; i += step, indexLB++)
                {
                    if (lowerBound1[indexLB] < currentMin)
                    {
                        // Check direction of matching from gesture1 to gesture2
                        currentMin = math.min(currentMin, 
                            CloudDistance(gesture1.Points, gesture2.Points, i, currentMin, spellSettings));
                    }

                    if (lowerBound2[indexLB] < currentMin)
                    {
                        // Check direction of matching from gesture2 to gesture1
                        currentMin = math.min(currentMin, 
                            CloudDistance(gesture2.Points, gesture1.Points, i, currentMin, spellSettings));
                    }
                }
            }
            else
            {
                for (int i = 0; i < n; i += step)
                {
                    // Check direction of matching from gesture1 to gesture2
                    currentMin = math.min(currentMin, 
                        CloudDistance(gesture1.Points, gesture2.Points, i, currentMin, spellSettings));

                    // Check direction of matching from gesture2 to gesture1
                    currentMin = math.min(currentMin, 
                        CloudDistance(gesture2.Points, gesture1.Points, i, currentMin, spellSettings));
                }
            }

            return currentMin;
        }

        /// <summary>
        /// Computes lower bounds for each starting point and the direction of matching from points1 to points2.
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <param name="lut"></param>
        /// <param name="step"></param>
        /// <param name="spellSettings"></param>
        /// <returns></returns>
        private static float[] ComputeLowerBound(ProcessedPoint[] points1, ProcessedPoint[] points2,
            int[] lut, int step, SpellSettings spellSettings)
        {
            int size = spellSettings.lutSize;
            float invScale = spellSettings.InvLutScaleFactor;
            
            int n = points1.Length;
            float[] lowerBound = new float[n / step + 1];
            float[] summedAreaTable = new float[n];

            lowerBound[0] = 0;
            for (int i = 0; i < n; i++)
            {
                int y = math.clamp((int)(points1[i].IntY * invScale), 0, size - 1);
                int x = math.clamp((int)(points1[i].IntX * invScale), 0, size - 1);
                int index = lut[y * size + x];
                float dist = Vector2.Distance(points1[i].Pos, points2[index].Pos);
                summedAreaTable[i] = (i == 0) ? dist : summedAreaTable[i - 1] + dist;
                lowerBound[0] += (n - i) * dist;
            }

            for (int i = step, indexLB = 1; i < n; i += step, indexLB++)
            {
                lowerBound[indexLB] = lowerBound[0] + i * summedAreaTable[n - 1] - n * summedAreaTable[i - 1];
            }

            return lowerBound;
        }

        /// <summary>
        /// Computes the distance between two point clouds by performing a minimum-distance
        /// greedy matching starting with point startIndex.
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <param name="startIndex"></param>
        /// <param name="currentMin"></param>
        /// <param name="spellSettings"></param>
        /// <returns></returns>
        private static float CloudDistance(ProcessedPoint[] points1, ProcessedPoint[] points2, 
            int startIndex, float currentMin, SpellSettings spellSettings)
        {
            int n = points1.Length;

            // Store indices for points from the 2nd cloud that have yet to be matched
            int[] indicesNotMatched = new int[n];
            for (int j = 0; j < n; j++)
            {
                indicesNotMatched[j] = j;
            }

            // Compute the sum of distances between matched points
            float cloudDist = 0;
            int i = startIndex;
            int weight = n;
            int indexNotMatched = 0;

            do
            {
                int index = -1;
                float minDist = float.MaxValue;
                for (int j = indexNotMatched; j < n; j++)
                {
                    float dist = Vector2.Distance(points1[i].Pos, points2[indicesNotMatched[j]].Pos);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = j;
                    }
                }

                indicesNotMatched[index] = indicesNotMatched[indexNotMatched];

                // Weight each distance with a confidence coefficient that decreases from n to 1
                cloudDist += (weight--) * minDist;

                if (spellSettings.useEarlyAbandoning && cloudDist >= currentMin)
                {
                    return cloudDist;
                }

                // Advance to the next points in the 1st cloud
                i = (i + 1) % n;

                // Update the number of points from the 2nd cloud that have yet to be matched
                indexNotMatched++;
            } while (i != startIndex);

            return cloudDist;
        }
    }
}