using UnityEngine;
using System.Text;

public static class JsonSerializer
{
    public static string SerializeFrame(Color32[] framePixels)
    {
        StringBuilder jsonSB = new StringBuilder("[");
        for (int i = 0; i < framePixels.Length; i++)
        {
            Color32 pixelColor = framePixels[i];
            jsonSB.Append("[");
            jsonSB.Append(pixelColor.r);
            jsonSB.Append(", ");
            jsonSB.Append(pixelColor.g);
            jsonSB.Append(", ");
            jsonSB.Append(pixelColor.b);
            jsonSB.Append("]");
            if (i + 1 < framePixels.Length)
            {
                jsonSB.Append(", ");
            }
        }
        jsonSB.Append("]");
        return jsonSB.ToString();
    }
}
