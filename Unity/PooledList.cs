using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Polymorph.Unity {

    internal enum Criticality { OutsideNeg, CriticalNeg, Inside, CriticalPos, OutsidePos, Start, End }

    /// <summary>
    /// A scroling list that holds only the items presented to the screen
    /// </summary>
    public class PooledList<T1, T2> : UIBehaviour, IList<T1> where T2 : PooledListItem<T1>  {

        //[System.Serializable]
        //public class ItemPool : Pool<T2> { }

        /// <summary>
        /// Basic directions for scroll
        /// </summary>
        public enum Direction { TopToBottom, BottomToTop, LeftToRight, RightToLeft }
        /// <summary>
        /// Pool which contains the elements in the list currently, and base data that is required to create elements in the pool
        /// </summary>
        // public ItemPool pool;
        /// <summary>
        /// Which axis should the list be scrolled on
        /// </summary>
        public Direction direction = Direction.TopToBottom;

        [Header("Debug")]
        public IList<T1> items = new List<T1>();
        public List<T2> visible = new List<T2>();
        public bool saturated = false;
        public float posEdge = 0;
        public float negEdge = 0;

        public void Scroll(Vector2 delta) {
            var d = delta;
            var myRTrans = transform as RectTransform;
            var rect = myRTrans.rect;
            switch(direction) {
                case Direction.TopToBottom: {
                    var topEdge = rect.height / 2;
                    var bottomEdge = -rect.height / 2;
                    for(int i = 0; i < visible.Count; ++i) {
                        var ele = visible[i];
                        var rTrans = ele.transform as RectTransform;
                        rTrans.anchoredPosition += d;
                        var eleTopEdge = rTrans.localPosition.y + (rTrans.rect.height / 2);
                        var eleBottomEdge = rTrans.localPosition.y - (rTrans.rect.height / 2);
                        var criticality = GetCriticalityState(eleBottomEdge, eleTopEdge);
                        if(criticality != ele.criticality) {
                            if(d.y > 0) {
                                if(criticality == Criticality.OutsidePos) {
                                    ReturnElement(ele);
                                    --i;
                                    continue;
                                } else if((criticality == Criticality.Inside) && (ele.criticality == Criticality.CriticalNeg)) {
                                    CreateElement(ele.index + 1, eleBottomEdge, 1, myRTrans);
                                }
                            } else if(d.y < 0) {
                                if(criticality == Criticality.OutsideNeg) {
                                    ReturnElement(ele);
                                    --i;
                                    continue;
                                } else if((criticality == Criticality.Inside) && (ele.criticality == Criticality.CriticalPos)) {
                                    CreateElement(ele.index + 1, eleBottomEdge, 1, myRTrans);
                                }
                            }
                            ele.criticality = criticality;
                        }
                    }
                }
                break;
            }
        }

        protected override void OnRectTransformDimensionsChange() {
            base.OnRectTransformDimensionsChange();
            var rt = transform as RectTransform;
            switch(direction) {
                case Direction.BottomToTop:
                case Direction.TopToBottom:
                    posEdge = (rt.rect.height / 2);
                    negEdge = -posEdge;
                    break;
                case Direction.LeftToRight:
                case Direction.RightToLeft:
                    posEdge = (rt.rect.width / 2);
                    negEdge = -posEdge;
                    break;
            }
        }

        protected virtual T2 GetElement() {
            return null;
        }

        protected virtual void ReturnElement(T2 ele) {

        }

        protected virtual void ClearElements() {

        }

        Criticality GetCriticalityState(float eleNegEdge, float elePosEdge) {
            if(eleNegEdge < negEdge) {
                if(elePosEdge < negEdge) {
                    return Criticality.OutsideNeg;
                } else {
                    return Criticality.CriticalNeg;
                }
            } else if(elePosEdge > posEdge) {
                if(eleNegEdge > posEdge) {
                    return Criticality.OutsideNeg;
                } else {
                    return Criticality.CriticalPos;
                }
            } else {
                return Criticality.Inside;
            }
        }

       T2 CreateElement(int index, float edge, float dir, RectTransform myRTrans) {
            var retVal = GetElement();
            var rTrans = retVal.transform as RectTransform;
            switch(direction) {
                case Direction.TopToBottom:
                    rTrans.anchorMin = Vector2.one;
                    rTrans.anchorMax = Vector2.one;
                    rTrans.sizeDelta = new Vector2(myRTrans.rect.width, rTrans.sizeDelta.y);
                    if(dir > 0) {
                        rTrans.localPosition = new Vector2(0, edge - (rTrans.sizeDelta.y / 2));
                        retVal.criticality = Criticality.CriticalNeg;
                    } else {
                        rTrans.localPosition = new Vector2(0, edge + (rTrans.sizeDelta.y / 2));
                        retVal.criticality = Criticality.CriticalPos;
                    }
                    break;
            }
            retVal.SetData(index, items[index]);
            return retVal;
        }

        T2 CreateElement() {

            var retVal = GetElement();
            var rt = retVal.transform as RectTransform;
            var mrt = transform as RectTransform;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.one;
            
            switch(direction) {
                case Direction.BottomToTop:
                case Direction.TopToBottom:
                    rt.sizeDelta = new Vector2(mrt.rect.width, rt.sizeDelta.y);
                    break;
                case Direction.RightToLeft:
                case Direction.LeftToRight:
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x,  mrt.rect.height);
                    break;
            }
            return retVal;
        }

        void CreateFirst() {
            switch(direction) {
                case Direction.TopToBottom:
                    var newEle = CreateElement();
                    var mrt = transform as RectTransform;
                    var rt = newEle.transform as RectTransform;
                    rt.localPosition = new Vector2(-(rt.rect.width / 2), mrt.rect.height - rt.rect.height / 2);
                    newEle.SetData(0, items[0]);
                    visible.Add(newEle);
                    break;
            }
        }

        void CreateElementAfter(T2 ele) {
            var rt = ele.transform as RectTransform;
            var newEle = CreateElement();
            switch(direction) {
                case Direction.TopToBottom:
                    var bottomEdge = rt.localPosition.y - (rt.rect.height / 2);
                    var nrt = newEle.transform as RectTransform;
                    nrt.localPosition = new Vector2(-(nrt.rect.width / 2), bottomEdge - nrt.rect.height / 2);
                    break;
            }
            var nIndex = ele.index + 1;
            if(nIndex >= items.Count) {
                newEle.index = nIndex;
                newEle.criticality = Criticality.End;
            } else {
                newEle.SetData(nIndex, items[nIndex]);
            }
            visible.Insert(newEle.index, newEle);
        }

        void CreateElementBefore(T2 ele) {
            var rt = ele.transform as RectTransform;
            var newEle = CreateElement();
            switch(direction) {
                case Direction.TopToBottom:
                    var topEdge = rt.localPosition.y + (rt.rect.height / 2);
                    var nrt = newEle.transform as RectTransform;
                    nrt.localPosition = new Vector2(-(nrt.rect.width / 2), topEdge + (nrt.rect.height / 2));
                    break;
            }
            var nIndex = ele.index - 1;
            if(nIndex < 0) {
                newEle.index = nIndex;
                newEle.criticality = Criticality.Start;
            } else {
                newEle.SetData(nIndex, items[nIndex]);
            }
            visible.Insert(newEle.index, newEle);
        }

        void Align(int vIndex = -1) {
            vIndex = vIndex == -1 ? 0 : vIndex;
            var rt = visible[vIndex].transform as RectTransform;
            switch(direction) {
                case Direction.TopToBottom: {
                    var bottom = rt.localPosition.y - (rt.rect.height / 2);
                    for(int i = vIndex + 1; i < visible.Count; ++i) {
                        var ele = visible[i];
                        rt = ele.transform as RectTransform;
                        var hh = rt.rect.height / 2;
                        rt.localPosition = new Vector2(rt.localPosition.x, bottom - hh);
                        bottom = rt.localPosition.y - hh;
                        var criticality = GetCriticalityState(rt.localPosition.y + hh, bottom);
                        if(criticality == Criticality.OutsideNeg) {
                            ReturnElement(ele);
                            --i;
                            continue;
                        }
                        ele.criticality = criticality;
                    }
                } break;
            }
            
        }

        void ElementsChanged() {
            if(visible.Count == 0) {

            } else {
                var first = visible[0];
                switch(direction) {
                    case Direction.TopToBottom:
                        if(first.criticality != Criticality.CriticalPos) {
                            
                        }
                        break;
                }
            }
            bool negCrit = false;
            bool posCrit = false;
            T2 prevEle = null;
            for(int i = 0; i < visible.Count; ++i) {
                var ele = visible[i];
                if(ele.criticality == Criticality.CriticalNeg) {
                    negCrit = true;
                } else if(ele.criticality == Criticality.CriticalPos) {
                    posCrit = true;
                }
                if((prevEle.index + 1) != ele.index) {
                    // Missing index
                }
                prevEle = ele;
            }
            if(!posCrit || !negCrit) {
                // Not saturated

            }
            if(visible.Count == 0) {
                if(items.Count > 0) {
                    switch(direction) {
                        case Direction.TopToBottom:

                            break;
                    }
                }
            }
        }

        #region IList<T1> implementation
        public int Count => items.Count;
        public bool IsReadOnly => items.IsReadOnly;
        public T1 this[int index] { get => items[index]; set => items[index] = value; }

        public void Add(T1 item) {
            items.Add(item);
            if(visible.Count == 0) {
                CreateFirst();
            } else {
                var last = visible[visible.Count - 1];
                if((last.index + 1) == (items.Count - 1)) {
                    CreateElementAfter(last);
                }
            }
        }
        public void RemoveAt(int index) {
            items.RemoveAt(index);
        }
        public int IndexOf(T1 item) { return items.IndexOf(item); }
        public void Insert(int index, T1 item) {
            items.Insert(index, item);
            if(visible.Count == 0) {
                CreateFirst();
            } else {
                var first = visible[0];
                var last = visible[visible.Count - 1];
                if((index >= first.index) && (index <= last.index)) {
                    var vIndex = 0;
                    for(int i = 0; i < visible.Count; ++i) {
                        var ele = visible[i];
                        if(ele.index == index) {
                            vIndex = i;
                            break;
                        }
                        if(ele.index >= index) {
                            ++ele.index;
                        }
                    }
                    CreateElementBefore(visible[vIndex]);
                }
                if(last.index == (items.Count - 1)) {
                    CreateElementAfter(last);
                }
            }
        }
        public void Clear() {
            items.Clear();
            items.Clear();
            ClearElements();
        }
        public bool Contains(T1 item) { return items.Contains(item); }
        public void CopyTo(T1[] array, int arrayIndex) { items.CopyTo(array, arrayIndex); }
        public bool Remove(T1 item) {
            for(int i = 0; i < items.Count; ++i) {
                if(items[i].GetHashCode() == item.GetHashCode()) {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public IEnumerator<T1> GetEnumerator() { return items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return items.GetEnumerator(); }
        #endregion
    }

    public class PooledListItem<T> : UIBehaviour {

        internal Criticality criticality;
        public int index;

        public virtual void SetData(int index, T data) {
            this.index = index;
        }

        protected override void OnRectTransformDimensionsChange() {
            base.OnRectTransformDimensionsChange();
            // Inform list my dims changed
        }
    }
}
