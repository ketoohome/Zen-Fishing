using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOOL;

public class Fish : MonoBehaviour {

    // Use this for initialization
    void Start() {
        _targetpos = transform.position;
        _currpos = _targetpos;
        InitlizeState();
    }

    // Update is called once per frame
    void Update() {
        mStateMachine.Update();
    }

    /* TODO LIST
    1. 吃鱼钩
        1>. 如果目标在指定范围内，则关注开始关注目标，并逐渐靠近目标
        2>. 如果目标的动量大于某个范围，则开始逃跑,逃跑目标为目标反方向，且目标远近根据动量本身
        3>. 当接触目标是鱼钩，则开始触碰目标，反复触碰n次，则制定其他目标
        4>. 当触碰目标是别的，则开始制定下一个目标
     */

    #region Action
    void swim(Vector3 target,float speed = 1){
        _targetpos = mTarget.transform.position;
        _currpos = Vector3.Lerp(_currpos,_targetpos,Time.deltaTime * speed);
        transform.position = _currpos;
    }
    Vector3 _targetpos, _currpos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mTarget.gameObject) {
            mTouchTargetAction(other.GetComponent<TargetPoint>());
        }
    }

    void SetTarget(TargetPoint target, Action<TargetPoint> action) {
        mTarget = target;
        mTouchTargetAction = action;
    }
    TargetPoint mTarget;
    Action<TargetPoint> mTouchTargetAction;

    #endregion

    #region State
    enum State {
        Standy, // 
        Escape, // 
        EatHook,
        End,
    }
    IStateMachine<Fish> mStateMachine;

    void InitlizeState() {
        mStateMachine = new IStateMachine<Fish>(this);
        mStateMachine.Add(State.Standy, new StateStandy());
        mStateMachine.Add(State.Escape, new StateEscape());
        mStateMachine.Add(State.EatHook, new StateEatHook());
        mStateMachine.Add(State.End, new StateEnd());
        mStateMachine.SetCurrentState(State.Standy);
    }

    class StateStandy : IState<Fish> {

        public override void Enter(Fish root)
        {
            Debug.LogError("进入普通状态");

            _root = root;
            //获取一个靠近视野范围的随机目标,并设定为鱼的目标
            //root.SetTarget(SchoolFish.GetViewportTarget(),(TargetPoint target)=> OnTouchTarget(target));
            root.StartCoroutine(ChangeTarget());
        }

        public override void Execute(Fish root)
        {
            root.swim(root.mTarget.transform.position,1);
            // TODO : 判断当前的目标的动量，如果动量大于某个值则进入Escape状态
            if (root.mTarget.mTargetAccelerated > 0.1f) root.mStateMachine.ChangeState(State.Escape);
        }

        public override void Exit(Fish root)
        {
            root.StopAllCoroutines();
        }
        Fish _root;

        void OnTouchTarget(TargetPoint target) {
            // TODO : 判断该目标是否为鱼钩，如果是鱼钩则进入EatHook状态
            if (target.name == "hook")  _root.mStateMachine.ChangeState(State.EatHook);
        }

        IEnumerator ChangeTarget() {
            while (true) {
                _root.SetTarget(SchoolFish.GetViewportTarget(), (TargetPoint tar) => OnTouchTarget(tar));
                yield return new WaitForSeconds(2.0f);
            }
        }
    }

    class StateEscape : IState<Fish> {
        public override void Enter(Fish root)
        {
            Debug.LogError("进入逃离状态");
            _root = root;
            _targetPos = (root.transform.position - root.mTarget.transform.position).normalized * 5;
        }
        public override void Execute(Fish root)
        {
            root.swim(_targetPos, 1);
        }

        Vector3 _targetPos;
        Fish _root; 
        void OnTouchTarget(TargetPoint target)
        {
            _root.mStateMachine.ChangeState(State.Standy);
        }
    }

    class StateEatHook : IState<Fish> {
        public override void Enter(Fish root)
        {
            Debug.LogError("进入吃鱼钩状态");
            _root = root;
            root.SetTarget(root.mTarget, (TargetPoint target) => OnTouchTarget(target));
        }
        public override void Execute(Fish root)
        {
            root.swim(root.mTarget.transform.position, 1);
            // 判断当前的目标的动量，如果动量大于某个值则进入Escape状态
            if (root.mTarget.mTargetAccelerated > 0.1f) root.mStateMachine.ChangeState(State.Escape);
        }
        Fish _root;

        void OnTouchTarget(TargetPoint target)
        {
            // TODO : 如果触碰三次则变回普通状态
            _root.mStateMachine.ChangeState(State.Standy);
        }
    }

    class StateEnd : IState<Fish> {
    }
    #endregion
}
