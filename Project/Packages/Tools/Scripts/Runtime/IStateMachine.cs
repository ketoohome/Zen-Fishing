using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TOOL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IState<T>
    {
        protected T Root;
        public IState()
        {

        }

        //进入状态  
        public virtual void Enter(T root)
        {

        }

        //状态正常执行
        public virtual void Execute(T root)
        {

        }

        //退出状态
        public virtual void Exit(T root)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IMonoState<T> : MonoBehaviour
    {
        //进入状态  
        public virtual void Enter(T root)
        {

        }

        //状态正常执行
        public virtual void Execute(T root)
        {

        }

        //退出状态
        public virtual void Exit(T root)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IStateMachine<T>
    {

        private IState<T> currentState;
		private List<IState<T>> globalStates;
		private Dictionary<System.Enum, IState<T>> states;
        private T root;

        public IStateMachine(T _root)
        {
            root = _root;
            currentState = null;
			globalStates = new List<IState<T>>();
			states = new Dictionary<System.Enum, IState<T>>();
        }

		public void Add(System.Enum key, IState<T> node)
        {
            if (!states.ContainsKey(key))
            {
                states.Add(key, node);
            }
        }

		public IState<T> Get(System.Enum key)
        {
            if (states.ContainsKey(key))
            {
                return states[key];
            }
            else
            {
                return null;
            }
        }

        public void SetGlobalState(System.Enum key)
        {
			IState<T> state = Get(key);
            if (!globalStates.Contains(state))
            {
                state.Enter(root);
                globalStates.Add(state);
            }
        }

        public void SetCurrentState(System.Enum key)
        {
			IState<T> state = Get(key);
            currentState = state;
            currentState.Enter(root);
        }

        public void Update()
        {
            
            //全局状态的运行
			foreach (IState<T> state in globalStates)
            {
                state.Execute(root);
            }

            //一般当前状态的运行
            if (currentState != null)
                currentState.Execute(root);
        }

        public void ChangeState(System.Enum key)
        {
			IState<T> state = Get(key);
            if (state == null)
            {
                Debug.LogError("该状态不存在: " + key);
                return;
            }

            if (currentState == state)
            {
                Debug.LogError("该状态已存在: " + key);
                return;
            }

            //退出之前状态
            if (currentState != null)
                currentState.Exit(root);

            //设置当前状态
            currentState = state;

            //进入当前状态
            if (currentState != null)
                currentState.Enter(root);
        }

		public IState<T> CurrentState()
        {
            //返回目前状态 
            return currentState;
        }
		public List<IState<T>> GlobalStates()
        {
            //返回全局状态 
            return globalStates;
        }

        //
		public void RemoveGlobalState(IState<T> state)
        {
            if (globalStates.Contains(state))
            {
                state.Exit(root);
                globalStates.Remove(state);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoStateMachine<T>
    {

        private IMonoState<T> currentState;
		private IMonoState<T> previousState;
		private List<IMonoState<T>> globalStates;
        private T root;

        public MonoStateMachine(T _root)
        {
            root = _root;
            currentState = null;
            previousState = null;
			globalStates = new List<IMonoState<T>>();
        }

		public void SetGlobalState(IMonoState<T> state)
        {
            if (!globalStates.Contains(state))
            {
                state.Enter(root);
                globalStates.Add(state);
            }
        }

		public void RemoveGlobalState(IMonoState<T> state)
        {
            if (globalStates.Contains(state))
            {
                state.Exit(root);
                globalStates.Remove(state);
            }
        }

		public void SetCurrentState(IMonoState<T> state)
        {
            currentState = state;
            currentState.Enter(root);
        }

        public void Update()
        {
            //全局状态的运行
			foreach (IMonoState<T> state in globalStates)
            {
                state.Execute(root);
            }

            //一般当前状态的运行
            if (currentState != null)
                currentState.Execute(root);
        }

		public void ChangeState(IMonoState<T> pNewState)
        {
            if (pNewState == null)
            {
                Debug.LogError("该状态不存在");
            }

            if (currentState != pNewState)
            {
                //退出之前状态
                if (currentState != null)
                    currentState.Exit(root);

                //保存之前状态
                previousState = currentState;

                //设置当前状态
                currentState = pNewState;

                //进入当前状态
                if (currentState != null)
                    currentState.Enter(root);
            }
        }

        public void RevertToPreviousState()
        {
            //qie huan dao qian yi ge zhuang tai 
            ChangeState(previousState);
        }

		public IMonoState<T> CurrentState()
        {
            //fan hui dang qian zhuang tai 
            return currentState;
        }
		public List<IMonoState<T>> GlobalStates()
        {
            //fan hui quan ju zhuang tai 
            return globalStates;
        }
		public IMonoState<T> PreviousState()
        {
            //fan hui qian yi ge zhuang tai 
            return previousState;
        }
    }

}

