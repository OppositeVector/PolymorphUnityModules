using UnityEngine;

namespace Polymorph.Unity {

    public static class RectTransformExtensions {
        /// <summary>
        /// Get top edge of this transfrom relative to the parent's top edge
        /// </summary>
        /// <returns>Relative positoin of top edge, positive inside and negative outside</returns>
        public static float Top(this RectTransform rTrans) {
            var parent = rTrans.parent as RectTransform;
            var anchorTopOnParent = parent.rect.height * rTrans.anchorMax.y;
            return anchorTopOnParent - rTrans.offsetMax.y;
        }
        /// <summary>
        /// Position top edge of transform relative to parent's top edge
        /// </summary>
        /// <param name="value">Relative positoin of top edge, positive inside and negative outside</param>
        public static void Top(this RectTransform rTrans, float value) {
            var top = rTrans.Top();
            var pos = rTrans.anchoredPosition;
            rTrans.anchoredPosition = new Vector2(pos.x, pos.y - (value - top));
        }
        /// <summary>
        /// Get bottom edge of this transfrom relative to the parent's bottom edge
        /// </summary>
        /// <returns>Relative positoin of bottom edge, positive inside and negative outside</returns>
        public static float Bottom(this RectTransform rTrans) {
            var parent = rTrans.parent as RectTransform;
            var anchorBottomOnParent = parent.rect.height * rTrans.anchorMin.y;
            return anchorBottomOnParent + rTrans.offsetMin.y;
        }
        /// <summary>
        /// Position bottom edge of transform relative to parent's bottom edge
        /// </summary>
        /// <param name="value">Relative positoin of bottom edge, positive inside and negative outside</param>
        public static void Bottom(this RectTransform rTrans, float value) {
            var bottom = rTrans.Bottom();
            var pos = rTrans.anchoredPosition;
            rTrans.anchoredPosition = new Vector2(pos.x, pos.y + (value - bottom));
        }
        /// <summary>
        /// Get right edge of this transfrom relative to the parent's right edge
        /// </summary>
        /// <returns>Relative positoin of right edge, positive inside and negative outside</returns>
        public static float Right(this RectTransform rTrans) {
            var parent = rTrans.parent as RectTransform;
            var anchorRightOnParent = parent.rect.width * rTrans.anchorMax.x;
            return anchorRightOnParent - rTrans.offsetMax.x;
        }
        /// <summary>
        /// Position right edge of transform relative to parent's right edge
        /// </summary>
        /// <param name="value">Relative positoin of right edge, positive inside and negative outside</param>
        public static void Right(this RectTransform rTrans, float value) {
            var right = rTrans.Right();
            var pos = rTrans.anchoredPosition;
            rTrans.anchoredPosition = new Vector2(pos.x - (value - right), pos.y);
        }
        /// <summary>
        /// Get left edge of this transfrom relative to the parent's left edge
        /// </summary>
        /// <returns>Relative positoin of left edge, positive inside and negative outside</returns>
        public static float Left(this RectTransform rTrans) {
            var parent = rTrans.parent as RectTransform;
            var anchorLeftOnParent = parent.rect.width * rTrans.anchorMin.x;
            return anchorLeftOnParent + rTrans.offsetMin.x;
        }
        /// <summary>
        /// Position left edge of transform relative to parent's left edge
        /// </summary>
        /// <param name="value">Relative positoin of left edge, positive inside and negative outside</param>
        public static void Left(this RectTransform rTrans, float value) {
            var left = rTrans.Left();
            var pos = rTrans.anchoredPosition;
            rTrans.anchoredPosition = new Vector2(pos.x + (value - left), pos.y);
        }
    }
}
