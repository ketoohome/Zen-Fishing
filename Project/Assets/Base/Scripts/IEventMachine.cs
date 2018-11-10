using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TOOL
{
    /// <summary>
    /// 
    /// </summary>
    public class IEventMachine
    {

        public delegate void OnEvent(params object[] args);
		internal protected Dictionary<System.Enum, EventNode> eventTable;

        /// <summary>
        /// 
        /// </summary>
        public class EventNode
        {

			private System.Enum callkey;
            private OnEvent callEvent = null;
            private List<OnEvent> callList;
            private object[] parameters;

			public EventNode(System.Enum key, OnEvent hander)
            {
                callkey = key;
                parameters = null;
                callEvent = hander;
                callList = new List<OnEvent>();
                callList.Add(hander);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            public void Add(OnEvent other)
            {
                if (!callList.Contains(other))
                {
                    callEvent += other;
                    callList.Add(other);
                }
#if DEBUG
                else
                {
                    Debug.Log("事件添加:事件已经存在 " + callkey);
                }
#endif
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            public void Remove(OnEvent other)
            {
                if (callList.Contains(other))
                {
                    callEvent -= other;
                    callList.Remove(other);
                }
#if DEBUG
                else
                {
                    Debug.Log("事件删除:没有这事件" + callkey);
                }
#endif
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public void Send(params object[] args)
            {
                if (callEvent != null)
                {
                    callEvent(args);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Send()
            {
                if (callEvent != null && parameters != null)
                {
                    callEvent(parameters);
                    parameters = null;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Set(params object[] args)
            {
                parameters = args;
            }

            /// <summary>
            /// 清除
            /// </summary>
            public void Clear()
            {
                callEvent = null;
                callList.Clear();
                callList = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class EventTime
        {
            private float delayTime;
            private float elapseTime;
            private EventNode events;
            private object[] args;

            public EventTime(EventNode callback, float delay, params object[] a)
            {
                events = callback;
                delayTime = delay;
                elapseTime = 0;
                args = a;
            }

            public bool Update()
            {
                if (elapseTime < delayTime)
                {
                    elapseTime += UnityEngine.Time.deltaTime;
                    return true;
                }
                else
                {
                    if (events != null)
                    {
                        events.Send(args);
                    }

                    return false;
                }
            }
        }

        public IEventMachine()
        {
			eventTable = new Dictionary<System.Enum, EventNode>();
        }

		public virtual void Add(System.Enum key, OnEvent function)
        {
            if (!eventTable.ContainsKey(key))
            {
                eventTable.Add(key, new EventNode(key, function));
            }
            else
            {
                eventTable[key].Add(function);
            }
        }

		public virtual void Remove(System.Enum key, OnEvent function)
        {
            if (eventTable.ContainsKey(key))
            {
                eventTable[key].Remove(function);
            }
#if DEBUG
            else
            {
                Debug.Log("事件注销:没有事件 " + key);
            }
#endif
        }

		public virtual void Send(System.Enum key, params object[] args)
        {
            EventNode node = null;

            if (eventTable.TryGetValue(key, out node))
            {
                node.Send(args);
            }
#if DEBUG
            else
            {
                Debug.Log("事件发送:没有事件 " + key);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 清除
        /// </summary>
        public virtual void Clear()
        {
            if (eventTable != null)
            {
                foreach (EventNode node in eventTable.Values)
                {
                    node.Clear();
                }
                eventTable.Clear();
            }
        }
    }

}