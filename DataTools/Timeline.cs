using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools {

    /// <summary>
    /// Holds a list of events ordered by their time, with simple access methods
    /// to retrieve data based on time
    /// </summary>
    public class Timeline : IEnumerable<Timeline.Event> {

        public enum InclusionState { None = 0, Start = 1, End = 2, Both = 3 }

        class TimeNode {
            public List<Event> events;
            public TimeNode() {
                events = new List<Event>();
            }
            public TimeNode(Event e) {
                events = new List<Event>();
                events.Add(e);
            }
            public TimeNode(TimeNode other) {
                events = new List<Event>();
                for(int i = 0; i < other.events.Count; ++i) {
                    events.Add(new Event(other.events[i]));
                }
            }
            public override string ToString() {
                var builder = new StringBuilder();
                for(int i = 0; i < events.Count; ++i) {
                    builder.Append(events[i].ToString());
                    builder.Append("\n");
                }
                return builder.ToString();
            }
            public bool Has<T>() {
                foreach(var e in events) {
                    if(e.Is<T>()) {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Base class for events that will be added to the timeline
        /// </summary>
        public sealed class Event {
            public float time;
            public object meta;
            public Event(float t, object m) {
                time = t;
                meta = m;
            }
            public Event(Event other) {
                time = other.time;
                meta = other.meta;
            }
            public override string ToString() {
                var builder = new StringBuilder();
                builder.Append("(");
                builder.Append(time);
                builder.Append(", ");
                builder.Append(meta == null ? "null" : meta.ToString());
                builder.Append(")");
                return builder.ToString();
            }
            public bool Is<T>() {
                return meta is T;
            }
            public bool Is(Type t) {
                return t.IsAssignableFrom(meta.GetType());
            }
        }

        /// <summary>
        /// Return an Event at a specific timestamp, timestamp has to be 100% exact
        /// </summary>
        public List<Event> this[float time] {
            get { return nodes[time].events; }
        }
        /// <summary>
        /// Return an event at a spesific index
        /// </summary>
        public List<Event> this[int index] {
            get { return nodes[nodes.Keys[index]].events; }
        }
        /// <summary>
        /// Amount of timestamps in the timeline
        /// </summary>
        public int count { get { return nodes.Count; } }
        /// <summary>
        /// First (chronologically) timestamp in the timeline
        /// </summary>
        public float start { get { return count > 0 ? nodes.Keys[0] : 0; } }
        /// <summary>
        /// Last (chronologically) timestamp in the timeline
        /// </summary>
        public float end { get { return count > 0 ? nodes.Keys[count - 1] : 0; } }
        /// <summary>
        /// Returns all the events between two timestamps (inclusivly), timestamps dont have to be exact
        /// </summary>
        /// <param name="s">Start timestamp</param>
        /// <param name="e">End timestamp</param>
        /// <returns>Event array</returns>
        public List<Event> this[float s, float e] {
            get { return GetRange(s, e); }
        }

        SortedList<float, TimeNode> nodes;

        public Timeline() {
            nodes = new SortedList<float, TimeNode>();
        }

        Timeline(Timeline other) {
            nodes = new SortedList<float, TimeNode>(other.nodes.Count);
            foreach(var pair in other.nodes) {
                nodes.Add(pair.Key, new TimeNode(pair.Value));
            }
        }

        Timeline(Timeline other, int start, int end) {
            var range = (end - start) + 1;
            range = range < 0 ? 0 : range;
            nodes = new SortedList<float, TimeNode>(range);
            for(int i = 0; i < range; ++i) {
                var time = other.nodes.Keys[start + i];
                var e = other.nodes[time];
                nodes.Add(time, new TimeNode(e));
            }
        }

        Int32 BinarySearchIndexOf<InnerT>(IList<InnerT> list, InnerT value, IComparer<InnerT> comparer = null) {
            if(list == null)
                throw new ArgumentNullException("list");

            comparer = comparer ?? Comparer<InnerT>.Default;

            Int32 lower = 0;
            Int32 upper = list.Count - 1;

            while(lower <= upper) {
                Int32 middle = lower + (upper - lower) / 2;
                Int32 comparisonResult = comparer.Compare(value, list[middle]);
                if(comparisonResult == 0)
                    return middle;
                else if(comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return ~lower;
        }

        public List<Event> GetRange(float start, float end, InclusionState inclusion = InclusionState.Both) {
            int startIndex = BinarySearchIndexOf<float>(nodes.Keys, start);
            int endIndex = BinarySearchIndexOf<float>(nodes.Keys, end);

            if((inclusion & InclusionState.Start) > 0) {
                startIndex = startIndex < 0 ? (~startIndex) + 1 : startIndex;
            } else {
                startIndex = (startIndex < 0 ? ~startIndex : startIndex) + 1;
            }
            if((inclusion & InclusionState.End) > 0) {
                endIndex = endIndex < 0 ? ~endIndex : ++endIndex;
            } else {
                endIndex = endIndex < 0 ? ~endIndex : endIndex;
            }

            int range = endIndex - startIndex;
            range = range < 0 ? 0 : range;
            List<Event> retVal = new List<Event>();
            for(int i = 0; i < range; ++i) {
                retVal.AddRange(nodes[nodes.Keys[startIndex + i]].events);
            }
            return retVal;
        }

        /// <summary>
        /// Add event to the timeline with a generic metadata object at a specific time
        /// </summary>
        /// <param name="time">Time to place the event at</param>
        /// <param name="meta">Metadata object to place at that time</param>
        public Event Add(float time, object meta) {
            var retVal = new Event(time, meta);
            if(nodes.ContainsKey(time)) {
                nodes[time].events.Add(retVal);
            } else {
                nodes.Add(time, new TimeNode(retVal));
            }
            return retVal;
        }
        /// <summary>
        /// Remove specific event from the timeline, if it exists
        /// </summary>
        /// <param name="e">Event to be removed</param>
        public void Remove(Event e) {
            if(nodes.ContainsKey(e.time)) {
                var currnet = nodes[e.time];
                if(currnet.events.Contains(e)) {
                    currnet.events.Remove(e);
                    if(currnet.events.Count == 0) {
                        nodes.Remove(e.time);
                    }
                }
            }
        }
        /// <summary>
        /// Remove an event at a spesific timestamp
        /// </summary>
        /// <param name="time">Timestamp at which there should be an event</param>
        public void RemoveTime(float time) {
            nodes.Remove(time);
        }
        /// <summary>
        /// Remove an event at a spesific index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) {
            nodes.RemoveAt(index);
        }

        /// <summary>
        /// Split the timeline into sub timelines based on a spesific event type, excluding events of the segmentation type
        /// </summary>
        /// <typeparam name="T">Event type by which to segment the timeline</typeparam>
        /// <returns>New timeline with all the events in between the segmnets</returns>
        public IEnumerable<Timeline> Segmented<T>() {
            int current = 0;
            int next = IndexOf<T>(current);
            if(next > -1) {
                yield return new Timeline(this, current, next - 1);
                while(next > -1) {
                    current = next;
                    next = IndexOf<T>(current + 1);
                    if(next > -1) {
                        yield return new Timeline(this, current + 1, next - 1);
                    }
                }
                yield return new Timeline(this, current + 1, count - 1);
            } else {
                yield return new Timeline(this);
            }
        }

        public List<Event> GetEvents<T>() {
            List<Event> retVal = new List<Event>();
            foreach(var key in nodes.Keys) {
                var node = nodes[key];
                foreach(var e in node.events) {
                    if(e.meta is T) {
                        retVal.Add(e);
                    }
                }
            }
            return retVal;
        }

        public bool ContainsTime(float t) {
            return nodes.ContainsKey(t);
        }

        /// <summary>
        /// Find the next event in the timeline of a spesific type
        /// </summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <param name="time">The time of the found event</param>
        /// <param name="startTime">Alternate start position, will exclude the events at the exact alternate timestamp</param>
        /// <returns>If an event of the spesific type was found</returns>
        public bool TimeOf<T>(out float time, float startTime = float.MinValue) {
            time = 0;
            if(nodes.Count == 0) {
                return false;
            }
            int start;
            if(nodes.Keys[0] > startTime) {
                start = 0;
            } else {
                start = BinarySearchIndexOf<float>(nodes.Keys, startTime);
                if(start < 0) {
                    start = ~start;
                }
                if(Approximetly(nodes.Keys[start], startTime, 0.001f)) {
                    ++start;
                }
            }
            var index = IndexOf<T>(start);
            if(index >= 0) {
                time = nodes.Keys[index];
                return true;
            } else {
                return false;
            }
        }

        int IndexOf<T>(int startIndex = 0) {
            for(int i = startIndex; i < nodes.Keys.Count; ++i) {
                if(nodes[nodes.Keys[i]].Has<T>()) {
                    return i;
                }
            }
            return -1;
        }

        bool Approximetly(float v1, float v2, float delta) {
            return System.Math.Abs(v2 - v1) < delta;
        }

        public IEnumerator<Timeline.Event> GetEnumerator() {
            foreach(var key in nodes.Keys) {
                var node = nodes[key];
                for(int i = 0 ; i < node.events.Count; ++i) {
                    yield return node.events[i];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            for(int i = 0; i < nodes.Keys.Count; ++i) {
                var time = nodes.Keys[i];
                builder.Append("\t");
                builder.Append(nodes[time]);
                builder.Append(",\n");
            }
            builder.Append("}");
            return builder.ToString();
        } 
    }
}
