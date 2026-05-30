using _Project.Features.Spells.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.Spells.Scripts
{
    public static class NProcessor
    {
        /// <summary>
        /// Normalizes the gesture path.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ProcessedPoint[] Normalize(GesturePoint[] points, SpellSettings settings)
        {
            // Resample, scale, and translate points to origin
            points = Resample(points, settings.sampleResolution);
            points = Scale(points);
            points = RotateToZero(points);
            points = TranslateTo(points);

            return ToProcessedPoints(points);
        }

        private static GesturePoint[] Resample(GesturePoint[] points, int sampleResolution)
        {
            GesturePoint[] newPoints = new GesturePoint[sampleResolution];
            newPoints[0] = new GesturePoint(points[0].Pos, points[0].StrokeId);
            int numPoints = 1;

            // Determine the interval length for each resampled point
            float intervalLength = PathLength(points) / (sampleResolution - 1);
            float currentLength = 0;

            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].StrokeId == points[i - 1].StrokeId)
                {
                    float nextDist = Vector2.Distance(points[i - 1].Pos, points[i].Pos);
                    if (currentLength + nextDist >= intervalLength)
                    {
                        Vector2 firstPoint = points[i - 1].Pos;
                        while (currentLength + nextDist >= intervalLength)
                        {
                            // Add interpolated point
                            float t = math.min(math.max((intervalLength - currentLength) / nextDist, 0f), 1f);
                            if (float.IsNaN(t)) t = 0.5f;

                            newPoints[numPoints] = new GesturePoint(
                                (1f - t) * firstPoint + t * points[i].Pos,
                                points[i].StrokeId
                            );

                            // Update current length
                            nextDist = currentLength + nextDist - intervalLength;
                            currentLength = 0;
                            firstPoint = newPoints[numPoints++].Pos;
                        }

                        currentLength = nextDist;
                    }
                    else currentLength += nextDist;
                }
            }

            // Add an extra point if falling a rounding-error short of adding the last point
            if (numPoints == sampleResolution - 1)
            {
                int lastIndex = points.Length - 1;
                newPoints[numPoints] = new GesturePoint(points[lastIndex].Pos, points[lastIndex].StrokeId);
            }
            return newPoints;
        }

        /// <summary>
        /// Computes the path length for an array of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static float PathLength(GesturePoint[] points)
        {
            float length = 0;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].StrokeId == points[i - 1].StrokeId)
                {
                    length += Vector3.Distance(points[i - 1].Pos, points[i].Pos);
                }
            }

            return length;
        }

        /// <summary>
        /// Normalizes scale with shape preservation to domain [0,1] x [0,1].
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static GesturePoint[] Scale(GesturePoint[] points)
        {
            // Find the min and max of each dimension
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var point in points)
            {
                Vector2 pos = point.Pos;
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }

            // Normalize bounds to [0,1] x [0,1]
            float invScale = 1f / math.max(maxX - minX, maxY - minY);
            Vector2 minBounds = new Vector2(minX, minY);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].Pos = (points[i].Pos - minBounds) * invScale;
            }

            return points;
        }

        private static GesturePoint[] RotateToZero(GesturePoint[] points)
        {
            // Calculate indicative angle from Centroid to Point 0
            Vector2 centroid = Centroid(points);
            float angle = math.atan2(points[0].Pos.y - centroid.y, points[0].Pos.x - centroid.x);
            
            // Rotate the entire gesture path by the negative of that angle
            return RotateBy(points, -angle, centroid);
        }

        private static GesturePoint[] RotateBy(GesturePoint[] points, float radians, Vector2 centroid)
        {
            float cos = math.cos(radians);
            float sin = math.sin(radians);

            for (int i = 0; i < points.Length; i++)
            {
                float dx = points[i].Pos.x - centroid.x;
                float dy = points[i].Pos.y - centroid.y;

                points[i].Pos.x = dx * cos - dy * sin + centroid.x;
                points[i].Pos.y = dx * sin + dy * cos + centroid.y;
            }

            return points;
        }

        /// <summary>
        /// Translates an array of points by p.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private static GesturePoint[] TranslateTo(GesturePoint[] points)
        {
            Vector2 centroid = Centroid(points);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].Pos -= centroid;
            }

            return points;
        }

        /// <summary>
        /// Computes the centroid for an array of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static Vector2 Centroid(GesturePoint[] points)
        {
            Vector2 centroid = new Vector2();
            foreach (GesturePoint point in points)
            {
                centroid += point.Pos;
            }

            return centroid / points.Length;
        }

        /// <summary>
        /// Convert GesturePoints to ProcessedPoints.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static ProcessedPoint[] ToProcessedPoints(GesturePoint[] points)
        {
            ProcessedPoint[] newPoints = new ProcessedPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                newPoints[i] = new ProcessedPoint(points[i].Pos, points[i].StrokeId);
            }

            return newPoints;
        }
    }
}