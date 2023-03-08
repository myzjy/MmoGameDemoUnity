using System.Collections.Generic;
using UnityEngine;

namespace ZJYFrameWork.UI.UIModel
{
  /**
 * 在Slice的Image上加上这个，就像Fill一样。
 * middle - center 的设定是理想状态
 */
public class SlicedImageFillHorizon : SlicedImageFillBase
{
    /// <summary>
    /// Left - Right
    /// </summary>
    protected override void FillNormal(List<UIVertex> vertexList)
    {
        var transRect = cacheRectTransform.rect;
        var spriteRect = cacheImage.sprite.rect;
        var spriteBorder = cacheImage.sprite.border;
        var zw = spriteBorder.z / spriteRect.width;

        var filledWidth = transRect.width * fillAmount;
        var maxX = transRect.xMin + filledWidth;
        var pivot = cacheRectTransform.pivot;
        var current = maxX + pivot.x * transRect.width;
        var hiddenWidth = transRect.width * (1f - fillAmount);
        var leftPer = current / spriteBorder.x;

        // border边缘部分的UV值
        var leftUV = filledWidth / cacheImage.sprite.texture.width + cacheImage.sprite.uv[0].x;
        var rightUV = (cacheImage.sprite.textureRect.xMax - (transRect.width * (1f - fillAmount))) /
                      (float)cacheImage.sprite.texture.width;
        // 右端的顶点基准地(0.01f用于允许误差)
        var rightEnd = pivot.x * transRect.width - 0.01f;

        var leftPerLess = leftPer < 1f;
        var thanLeft = filledWidth < spriteBorder.x;
        var thanRight = hiddenWidth < spriteBorder.z;
        var lessThanRight = transRect.width - current < spriteBorder.z;

        var vertexListCount = vertexList.Count;
        for (var i = 0; i < vertexListCount; ++i)
        {
            var element = vertexList[i];

            var vertexPos = vertexList[i].position;
            vertexPos.x = Mathf.Min(element.position.x, maxX);

            // UV操作.
            if (spriteBorder.x > 0f || spriteBorder.z > 0f)
            {
                // 隐藏区域被插入左边的边界，
                Vector2 uv0;
                if (0f < element.uv0.x - cacheImage.sprite.uv[0].x &&
                    element.position.x - transRect.xMin > filledWidth && leftPerLess && thanLeft)
                {
                    if (cacheImage.sprite.packed)
                    {
                        uv0.x = leftUV;
                    }
                    else
                    {
                        uv0.x = element.uv0.x * leftPer;
                    }

                    uv0.y = element.uv0.y;
                    element.uv0 = uv0;
                }
                else if (thanRight && rightEnd <= element.position.x)
                {
                    // 隐藏宽度小于右边界值
                    if (lessThanRight)
                    {
                        if (cacheImage.sprite.packed)
                        {
                            uv0.x = rightUV;
                        }
                        else
                        {
                            uv0.x = (zw - (transRect.width - current) / spriteBorder.z * zw) +
                                    (spriteRect.width - spriteBorder.z) / spriteRect.width;
                        }
                    }
                    else
                    {
                        uv0.x = element.uv0.x * (current / transRect.width);
                    }

                    uv0.y = element.uv0.y;
                    element.uv0 = uv0;
                }
            }

            element.position = vertexPos;
            vertexList[i] = element;
        }
    }

    /// <summary>
    /// X方向，从右边开始减少的Fill(仅Slice内侧变动)
    /// </summary>
    protected override void FillInner(List<UIVertex> vertexList)
    {
        var pivotX = cacheRectTransform.pivot.x;

        var width = cacheRectTransform.sizeDelta.x;
        var halfWidth = width * pivotX;
        var left = cacheImage.sprite.border.x;
        var right = cacheImage.sprite.border.z;

        var rect = cacheRectTransform.rect;
        var maxX = rect.xMin + (rect.width - left - right) * fillAmount;

        var vertexListCount = vertexList.Count;
        for (var i = 0; i < vertexListCount; ++i)
        {
            var element = vertexList[i];
            if (!(element.position.x > halfWidth * -1f + left)) continue;
            var tmp = element.position;
            tmp.x = Mathf.Min(element.position.x, maxX + left);
            if (element.position.x > width - halfWidth - right)
            {
                tmp.x += right;
            }

            element.position = tmp;
            vertexList[i] = element;
        }
    }
}
}