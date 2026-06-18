using UnityEngine;

namespace FinalRogue
{
    public static class StageMarkerUtility
    {
        static Sprite squareSprite;

        public static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
                return squareSprite;

            var texture = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
            return squareSprite;
        }
    }
}