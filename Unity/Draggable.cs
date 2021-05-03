using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Polymorph.Unity
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [SerializeField] private float AlphaOnDrag = 0.6f;

        private CanvasGroup _group;
        private float _origAlpha;
        private Canvas _canvas;
        private RectTransform _rTrans;
        private DragInLayout _zone;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _origAlpha = _group.alpha;
            _canvas = GetComponentInParent<Canvas>();
            _rTrans = transform as RectTransform;
            _zone = GetComponentInParent<DragInLayout>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _group.blocksRaycasts = false;
            _group.alpha = AlphaOnDrag;
            if(_zone != null)
                _zone.BeginDrag(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rTrans.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _group.blocksRaycasts = true;
            _group.alpha = _origAlpha;
            if(_zone != null)
                _zone.EndDrag(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_zone != null)
                _zone.PointerEnteredOnDraggable(gameObject);
        }
    }
}
