using System.Collections;
using System.Collections.Generic;

namespace TOOL
{
	// 计时器方法委托
	//public delegate void AlarmDelegate();

	/// <summary>
	/// 定时器管理器
	/// </summary>
	public class ClockMachine : Singleton<ClockMachine> {

		// 定时器列表
		List<Clock> m_Clocks = new List<Clock>();
		List<Clock> m_TempClocks = new List<Clock>();

		//
        static int m_TaskCounter = 0;
		// 还剩多少时间
		public float LastTime(string key){
			foreach(Clock clock in m_Clocks){
				if(clock.Key == key){
					return (clock.Duration - clock.CurrentTime);
				}
			}
			return 0;
		}

		// 创建定时器
        public bool CreateClock(float duration, System.Action alarm)
        {
            return CreateClock("Default" + m_TaskCounter.ToString(), duration, alarm);
		}

		// 创建定时器
        public bool CreateClock(string key, float duration, System.Action alarm)
        {
			if(GetClock(key) == null){
                m_TaskCounter++;
				Clock clock = new Clock(key,duration,alarm);
				m_Clocks.Add(clock);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 创建循环计时器
		/// </summary>
		/// <returns><c>true</c>, if loop clock was created, <c>false</c> otherwise.</returns>
		/// <param name="duration">Duration.</param>
		/// <param name="alarm">Alarm.</param>
		public bool CreateLoopClock(string key, float duration, System.Action alarm){
			if(GetClock(key) == null){
				m_TaskCounter++;
				Clock clock = new Clock(key,duration,alarm,false,true);
				m_Clocks.Add(clock);
				return true;
			}
			return false;
		}

        /// <summary>
        /// 创建定时器（真实时间） 
        /// </summary>
        /// <param name="key">索引</param>
        /// <param name="duration">持续时间</param>
        /// <param name="alarm">委托函数</param>
        /// <returns></returns>
        public bool CreateRealClock(string key, float duration, System.Action alarm)
        {
            if (GetClock(key) == null)
            {
                m_TaskCounter++;
                Clock clock = new Clock(key, duration, alarm, true);
                m_Clocks.Add(clock);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建定时器（真实时间）
        /// </summary>
        /// <param name="duration">持续时间</param>
        /// <param name="alarm">委托函数</param>
        /// <returns></returns>
        public bool CreateRealClock(float duration, System.Action alarm)
        {
            return CreateRealClock("Default" + m_TaskCounter.ToString(), duration, alarm);
        }

		/// <summary>
		/// 创建循环计时器
		/// </summary>
		/// <returns><c>true</c>, if loop clock was created, <c>false</c> otherwise.</returns>
		/// <param name="duration">Duration.</param>
		/// <param name="alarm">Alarm.</param>
		public bool CreateRealLoopClock(string key, float duration, System.Action alarm){
			if(GetClock(key) == null){
				m_TaskCounter++;
				Clock clock = new Clock(key,duration,alarm,true,true);
				m_Clocks.Add(clock);
				return true;
			}
			return false;
		}

		// 删除定时器
		public void DestroyClock(string key){
			m_Clocks.Remove(GetClock(key));
		}

		// 查找一个计时器
		Clock GetClock(string key){
			foreach(Clock clock in m_Clocks){
				if(clock.Key == key)return clock;
			}
			return null;
		}

        //static int currtime = System.DateTime.Now.Millisecond;
        //static int oldtime = System.DateTime.Now.Millisecond;
        //deltaTime = (float)(currtime - oldtime)*0.01f;
        //oldtime = currtime;
        //currtime = System.DateTime.Now.Millisecond;

		// 更新所有时间
		public void CustomUpdate(float deltaTime){
            
			// 更新所有计时器
			foreach(Clock clock in m_Clocks){
                if (clock.IsRealTime) continue;

                clock.CustomUpdate(deltaTime);
				if(clock.IsEnd)	m_TempClocks.Add(clock);
			}

			//
			if(m_TempClocks.Count == 0)	return;
			// 如果计时器已经到时，则自动删除该计时器
			foreach(Clock clock in m_TempClocks){
				if(clock.IsLoop) clock.Reset();
				else m_Clocks.Remove (clock);
				clock.Alarming();
			}
			m_TempClocks.Clear();
		}

        /// <summary>
        /// 更新所有真实时间
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RealTimeUpdate(float deltaTime)
        {
            // 更新所有计时器
            foreach (Clock clock in m_Clocks)
            {
                if (!clock.IsRealTime) continue;

                clock.CustomUpdate(deltaTime);
                if (clock.IsEnd) m_TempClocks.Add(clock);
            }

            //
            if (m_TempClocks.Count == 0) return;
            // 如果计时器已经到时，则自动删除该计时器
            foreach (Clock clock in m_TempClocks)
            {
				if(clock.IsLoop) clock.Reset();
				else m_Clocks.Remove (clock);
                clock.Alarming();
            }
            m_TempClocks.Clear();
        }

		/// 定时器
		class Clock{
            System.Action m_alarm = null;
			// 闹铃
			//AlarmDelegate m_alarm = null;
            public System.Action Alarm { get { return m_alarm; } }
			
			string m_key;				// 
			public string Key{ get { return m_key; }}
			
			float m_duration;			// 持续时间
			public float Duration{ get { return m_duration; }}
			
			float m_currenttime;		// 当前时间
			public float CurrentTime{ get { return m_currenttime; }}
			
			bool m_IsEnd;	// 当前是否结束
			public bool IsEnd{	get{ return m_IsEnd; }}

            bool m_IsRealTime = false; // 是否是真实时间
            public bool IsRealTime { get { return m_IsRealTime; } }

			bool m_IsLoop = false; // 是否循环
			public bool IsLoop{ get{ return m_IsLoop;}}
			/// <summary>
            /// 构造函数 
			/// </summary>
			/// <param name="key">索引</param>
			/// <param name="duration">时间</param>
			/// <param name="alarm">委托函数</param>
			/// <param name="isRealTime">是否真实时间计时</param>
            public Clock(string key, float duration, System.Action alarm, bool isRealTime = false, bool isLoop = false)
            {
				m_key = key;
				m_duration = duration;
				m_alarm = alarm;
                m_IsRealTime = isRealTime;
				m_IsLoop = isLoop;
				m_currenttime = 0;
			}
			// 更新
			public void CustomUpdate(float deltaTime){
				m_currenttime += deltaTime;
				m_IsEnd = (m_currenttime >= m_duration);
			}
			// 重置
			public void Reset() { m_currenttime = 0;}
			// 委托方法
			public void Alarming(){
				
				// 针对静态变量委托
				if(m_alarm != null && m_alarm.Target == null )	{
					m_alarm();
					return;
				}
				
				// 针对非静态变量委托
				if( m_alarm.Target.ToString() == "null")	return;
				else m_alarm();
			}
		}
	}

	
	/// <summary>
	/// 计数器管理器
	/// </summary>
	public class CounterMachine : Singleton<CounterMachine> {
        //
        static int m_TaskCounter = 0; // 已经创建的计数器任务个数
		// 定时器列表
		List<Counter> m_Counters = new List<Counter>();
		List<Counter> m_TempCounters = new List<Counter>();
		
		// 还剩多少时间
		public int LastCount(string key){
			foreach(Counter counter in m_Counters){
				if(counter.Key == key){
					return (counter.DurationCount - counter.CurrentCount);
				}
			}
			return 0;
		}
		
		// 创建计数器
        public bool CreateCounter(string key, int duration, System.Action alarm)
        {
			
			if(GetCounter(key) == null){
                m_TaskCounter++;
				Counter counter = new Counter(key,duration,alarm);
				m_Counters.Add(counter);
				return true;
			}
			return false;
		}

        // 创建定时器
        public bool CreateCounter(int duration, System.Action alarm)
        {
            return CreateCounter("Default" + m_TaskCounter.ToString(), duration, alarm);
        }


		// 删除计数器
		public void DestroyCounter(string key){
			m_Counters.Remove(GetCounter(key));
		}
		
		// 查找一个计数器
		Counter GetCounter(string key){
			foreach(Counter counter in m_Counters){
				if(counter.Key == key)return counter;
			}
			return null;
		}
		
		// 更新所有时间
		public void CustomUpdate(){
			// 更新所有计数器
			foreach(Counter counter in m_Counters){
				counter.CustomUpdate();
				if(counter.IsEnd)	m_TempCounters.Add(counter);
			}
			
			//
			if(m_TempCounters.Count == 0)	return;
			// 如果计时器已经到时，则自动删除该计数器
			foreach(Counter counter in m_TempCounters){
				m_Counters.Remove (counter);
				counter.Alarming();
			}
			m_TempCounters.Clear();
		}
		
		/// 定时器
		class Counter{
			// 闹铃
            System.Action m_alarm = null;
            public System.Action Alarm { get { return m_alarm; } }
			
			string m_key;				// 
			public string Key{ get { return m_key; }}
			
			int m_durationCount;			// 持续时间
			public int DurationCount{ get { return m_durationCount; }}
			
			int m_currentCount;		// 当前时间
			public int CurrentCount{ get { return m_currentCount; }}
			
			bool m_IsEnd;	// 当前是否结束
			public bool IsEnd{	get{ return m_IsEnd; }}
			
			// 构造函数 
            public Counter(string key, int duration, System.Action alarm)
            {
				m_key = key;
				m_durationCount = duration;
				m_alarm = alarm;
				m_currentCount = 0;
			}
			// 更新
			public void CustomUpdate(){
				m_currentCount += 1;
				m_IsEnd = (m_currentCount >= m_durationCount);
			}
			//
			public void Alarming(){

				// 针对静态变量委托
				if( m_alarm.Target == null && m_alarm != null)	{
					m_alarm();
					return;
				}

				// 针对非静态变量委托
				if( m_alarm.Target.ToString() == "null")	return;
				else m_alarm();
			}
		}
	}

	/// <summary>
	/// 方法流管理器 : 按照时间间隔固定的步骤执行委托方法
	/// </summary>
	public class StreamFunMachine : Singleton<StreamFunMachine>{
		
		List<OrderStream> m_OrderStreams = new List<OrderStream>();
		
		// 创建流事件
        public bool CreateStreamFun(string key, List<float> clocks, params System.Action[] args)
        {
			// 如果该流重复，则不进行创建
			if(GetStream(key)!=null) return false;
			OrderStream stream = new OrderStream();
			stream.key = key;
			for(int i = 0;i<args.Length && i<clocks.Count;i++){
				stream.clocks.Add(clocks[i]);
                stream.frames.Add((System.Action)args[i]);
			}
			m_OrderStreams.Add(stream);
			return true;
		}
		
		// 删除计数器
		public void DestroyCounter(string key){
			m_OrderStreams.Remove(GetStream(key));
		}
		
		// 查找一个计数器
		OrderStream GetStream(string key){
			foreach(OrderStream stream in m_OrderStreams){
				if(stream.key == key)return stream;
			}
			return null;
		}
		
		// 更新所有时间
		public void CustomUpdate(float deltaTime){
			for(int i = m_OrderStreams.Count - 1 ; i>=0;i--){
				if(!m_OrderStreams[i].CustomUpdate(deltaTime)){
					m_OrderStreams.Remove(m_OrderStreams[i]);
				}
			}
		}
		
		// 顺序流
		class OrderStream{
			public string key;
			public List<float> clocks = new List<float>();
            public List<System.Action> frames = new List<System.Action>();
			float currTime = 0;
			int count = 0; // 计数器

			public bool CustomUpdate(float deltaTime){
				currTime += deltaTime;
				if(currTime >= clocks[count]){
					if(frames[count].Target.ToString() != "null" ) frames[count]();
					else return false;
					count++;
					if(count >= frames.Count)	return false;
					currTime = 0;
				}
				return true;
			}
		}
	}
}