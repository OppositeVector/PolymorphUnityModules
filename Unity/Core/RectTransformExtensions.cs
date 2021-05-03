using UnityEngine;

namespace Polymorph.Unity.Core {

    public static class RectTransformExtensions {

        public static float Top(this RectTransform rTrans) {
            return -rTrans.offsetMax.y;
        }
        public static void Top(this RectTransform rTrans, float value) {
            rTrans.offsetMax = new Vector2(rTrans.offsetMax.x, -value);
        }
        public static float Bottom(this RectTransform rTrans) {
            return -rTrans.offsetMin.y;
        }
        public static void Bottom(this RectTransform rTrans, float value) {
            rTrans.offsetMin = new Vector2(rTrans.offsetMin.x, -value);
        }
        public static float Right(this RectTransform rTrans) {
            return -rTrans.offsetMax.x;
        }
        public static void Right(this RectTransform rTrans, float value) {
            rTrans.offsetMax = new Vector2(-value, rTrans.offsetMax.y);
        }
        public static float Left(this RectTransform rTrans) {
            return -rTrans.offsetMin.x;
        }
        public static void Left(this RectTransform rTrans, float value) {
            rTrans.offsetMin = new Vector2(-value, rTrans.offsetMin.y);
        }
        public static void Margins(this RectTransform rTrans, float? top = null, float? right = null, float? bottom = null, float? left = null) {
            if(top.HasValue || right.HasValue) {
                var lTop = top.HasValue ? -top.Value : rTrans.offsetMax.y;
                var lRight = right.HasValue ? -right.Value : rTrans.offsetMax.y;
                rTrans.offsetMax = new Vector2(lRight, lTop);
            }
            if(bottom.HasValue || left.HasValue) {
                var lBottom = bottom.HasValue ? bottom.Value : rTrans.offsetMin.y;
                var lLeft = left.HasValue ? left.Value : rTrans.offsetMin.x;
                rTrans.offsetMin = new Vector2(lLeft, lBottom);
            }
        }
        public static Vector2 CalculateOffparentPoint(this RectTransform rTrans, Vector2 refPoint, Direction direction) {

            Rect myRect = rTrans.rect;
            RectTransform parent = rTrans.parent as RectTransform;
            var anchorMin = parent.rect.width * rTrans.anchorMin.x;
            var anchorMax = parent.rect.width * rTrans.anchorMax.x;
            var x = anchorMin + ((anchorMax - anchorMin) / 2);
            anchorMin = parent.rect.height * rTrans.anchorMin.y;
            anchorMax = parent.rect.height * rTrans.anchorMax.y;
            var y = anchorMin + ((anchorMax - anchorMin) / 2);
            var anchorOnParent = new Vector2(x, y);

            var retVal = refPoint;

            if((direction & Direction.Top) > 0) {
                retVal.y = parent.rect.height + (rTrans.rect.height * rTrans.pivot.y) - anchorOnParent.y;
            } else if((direction & Direction.Bottom) > 0) {
                retVal.y = -(rTrans.rect.height * (1 - rTrans.pivot.y)) - anchorOnParent.y;
            }

            if((direction & Direction.Right) > 0) {
                retVal.x = parent.rect.width + (rTrans.rect.width * rTrans.pivot.x) - anchorOnParent.x;
            } else if((direction & Direction.Left) > 0) {
                retVal.x = -(rTrans.rect.width * (1 - rTrans.pivot.x)) - anchorOnParent.x;
            }

            return retVal;
        }

        public static bool ContainsScreenPoint(this RectTransform rTrans, Canvas myCanvas, Vector2 screenPoint) {
            var inCanvasRect = rTrans.rect;
            inCanvasRect.y += rTrans.localPosition.y;
            inCanvasRect.x += rTrans.localPosition.x;
            var checkedPoint = myCanvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, myCanvas.planeDistance));
            checkedPoint = myCanvas.transform.InverseTransformPoint(checkedPoint);
            return inCanvasRect.Contains(checkedPoint);
        }

        public static Vector2 GetSizeDelta(this RectTransform rTrans, Vector2 absoluteSize) {
            var anchoredWidth = rTrans.rect.width - rTrans.sizeDelta.x;
            var anchoredHeight = rTrans.rect.height - rTrans.sizeDelta.y;
            return new Vector2(absoluteSize.x - anchoredWidth, absoluteSize.y - anchoredHeight);
        }
    }
}
