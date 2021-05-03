using UnityEngine;
using UnityEngine.Events;

namespace Polymorph.Unity
{
    public class DragInLayout : MonoBehaviour
    {
        [System.Serializable]
        public class OrderChangedEvent : UnityEvent<int, GameObject> { }

        [SerializeField] private Transform TmpArea;
        [SerializeField] private Transform TmpObj;
        [SerializeField] private OrderChangedEvent OnOrderChanged;

        private GameObject _dragged;
        private int _originalIndex = 0;

        private void Awake()
        {
            TmpObj.gameObject.SetActive(false);
        }

        public void BeginDrag(GameObject obj)
        {
            _dragged = obj;
            _originalIndex = _dragged.transform.GetSiblingIndex();
            TmpObj.SetParent(_dragged.transform.parent);
            TmpObj.SetSiblingIndex(_originalIndex);
            TmpObj.gameObject.SetActive(true);
            _dragged.transform.SetParent(TmpArea, true);
        }

        public void EndDrag(GameObject obj)
        {
            var newIndex = TmpObj.GetSiblingIndex();
            var layout = TmpObj.parent;
            _dragged.transform.SetParent(layout);
            _dragged.transform.SetSiblingIndex(newIndex);
            TmpObj.SetParent(TmpArea);
            TmpObj.gameObject.SetActive(false);
            _dragged = null;

            if(OnOrderChanged != null)
            {
                var start = newIndex > _originalIndex ? _originalIndex : newIndex;
                var end = newIndex > _originalIndex ? newIndex : _originalIndex;
                for(int i = start; i <= end; ++i)
                {
                    OnOrderChanged.Invoke(i, layout.GetChild(i).gameObject);
                }
            }
        }

        public void PointerEnteredOnDraggable(GameObject obj)
        {
            if((obj != TmpObj.gameObject) && (_dragged != null))
                TmpObj.SetSiblingIndex(obj.transform.GetSiblingIndex());
        }
    }
}
