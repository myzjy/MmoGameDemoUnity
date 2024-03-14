using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    internal static class SpriteAssetViewerExtension
    {
        /// <summary>
        /// 重新排列Sprite。
        /// </summary>
        public static Sprite[] GetSpriteArray(this SpriteAtlas self)
        {
            var sprites = new Sprite[self.spriteCount];
            self.GetSprites(sprites);
            return sprites;
        }

        public static Object[] GetAtlasPacked(this SpriteAtlas self)
        {
            return self.GetPackables();
        }

        public static Rect Move(this Rect rect, Vector2 move)
        {
            rect.position += move;
            rect.size -= move;
            return rect;
        }

        public static Rect MoveH(this Rect rect, float move)
        {
            rect.x += move;
            rect.width -= move;
            return rect;
        }
        public static Rect MoveV(this Rect rect, float move)
        {
            rect.y += move;
            rect.height -= move;
            return rect;
        }
    }
}