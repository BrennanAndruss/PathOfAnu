using System.Collections.Generic;
using UnityEngine;
using _Project.Features.Spells.Scripts;

namespace _Project.Features.Spells.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GestureData", menuName = "Spells/GestureData")]
    public class GestureData : ScriptableObject
    {
        public int strokeCount;
        // public GesturePoint[] points;

        public List<Stroke> strokes = new List<Stroke>();
        public GesturePoint[] Points
        {
            get
            {
                List<GesturePoint> flattened = new List<GesturePoint>();

                foreach (var stroke in strokes)
                {
                    if (stroke != null && stroke.points != null)
                    {
                        flattened.AddRange(stroke.points);
                    }
                }

                return flattened.ToArray();
            }
        }
        // public string name;
        
        // [ContextMenu("Migrate Flag Points to Strokes")]
        // public void MigrateData()
        // {
        //     if (strokeCount == 1)
        //     {
        //         strokes.Add(new Stroke
        //         {
        //             points = points
        //         });
        //     }
        //     
        //     else if (name == "Aquarius")
        //     {
        //         GesturePoint[] strokePoints = new GesturePoint[84];
        //         for (int i = 0; i < 84; i++)
        //         {
        //             strokePoints[i] = points[i];
        //         }
        //         strokes.Add(new Stroke
        //         {
        //             points = strokePoints
        //         });
        //
        //         strokePoints = new GesturePoint[176 - 84];
        //         for (int i = 0; i < 176 - 84; i++)
        //         {
        //             strokePoints[i] = points[i + 84];
        //         }
        //         strokes.Add(new Stroke
        //         {
        //             points = strokePoints
        //         });
        //     }
        //     
        //     else if (name == "Pisces")
        //     {
        //         GesturePoint[] strokePoints = new GesturePoint[21];
        //         for (int i = 0; i < 21; i++)
        //         {
        //             strokePoints[i] = points[i];
        //         }
        //         strokes.Add(new Stroke
        //         {
        //             points = strokePoints
        //         });
        //         
        //         strokePoints = new GesturePoint[47 - 21];
        //         for (int i = 0; i < 47 - 21; i++)
        //         {
        //             strokePoints[i] = points[i + 21];
        //         }
        //         strokes.Add(new Stroke
        //         {
        //             points = strokePoints
        //         });
        //         
        //         strokePoints = new GesturePoint[73 - 47];
        //         for (int i = 0; i < 73 - 47; i++)
        //         {
        //             strokePoints[i] = points[i + 47];
        //         }
        //         strokes.Add(new Stroke
        //         {
        //             points = strokePoints
        //         });
        //     }
        // }
    }
}
