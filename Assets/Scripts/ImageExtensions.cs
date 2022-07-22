using UnityEngine;
using UnityEngine.UI;

static class ImageExtensions
{
    public static void SetAlpha(this Image self, float alpha)
    {
        Color tempColor = self.color;
        tempColor.a = alpha;
        self.color = tempColor;
    }
}
