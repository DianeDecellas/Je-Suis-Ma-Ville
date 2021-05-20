using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testDistanceDamLev : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(StringDistance.GetDamerauLevenshteinDistance("climax", "volmax"));
        Debug.Log(StringDistance.GetDamerauLevenshteinDistance("Ram", "Rom"));
        Debug.Log(StringDistance.GetDamerauLevenshteinDistance("Mam", "Mom"));
    }

    // Update is called once per frame
  
    public static class StringDistance
    {
        public static int GetDamerauLevenshteinDistance(string s, string t)
        {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

            int[,] matrix = new int[bounds.Height, bounds.Width];

            for (int height = 0; height < bounds.Height; height++) { matrix[height, 0] = height; };
            for (int width = 0; width < bounds.Width; width++) { matrix[0, width] = width; };

            for (int height = 1; height < bounds.Height; height++)
            {
                for (int width = 1; width < bounds.Width; width++)
                {
                    int cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    int insertion = matrix[height, width - 1] + 1;
                    int deletion = matrix[height - 1, width] + 1;
                    int substitution = matrix[height - 1, width - 1] + cost;

                    int distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }
    }
}
