using System;
using System.Collections;
using UnityEngine;
using TOOL;
using GameCommon;
using MoreMountains.NiceVibrations;

using Random = UnityEngine.Random;

public class Fish : MonoBehaviour {

    public int m_FishID = 1;
    public int m_Length = 20;
    public AnimationCurve m_EatAnimationCurve;

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
        _targetpos = target;
        _currpos = Vector3.Lerp(_currpos,_targetpos,Time.deltaTime * speed);
        transform.position = _currpos;
    }
    Vector3 _targetpos, _currpos;

    private void OnTriggerEnter(Collider other)
    {
        if (mTarget == null) return;
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

            Debug.LogError("进入待机状态");
            root.transform.Find("mesh").GetComponent<Renderer>().material.SetColor("_TintColor", Color.black);

            _root = root;
            //获取一个靠近视野范围的随机目标,并设定为鱼的目标
            //root.SetTarget(SchoolFish.GetViewportTarget(),(TargetPoint target)=> OnTouchTarget(target));
            _root.StartCoroutine(ChangeTarget());
        }

        public override void Execute(Fish root)
        {
            if (_root.mTarget != null) {
                _root.swim(root.mTarget.transform.position, 1);
                // 判断当前的目标的动量，如果动量大于某个值则进入Escape状态
                if (root.mTarget.mTargetAccelerated > 0.1f) _root.mStateMachine.ChangeState(State.Escape);
            }
        }

        public override void Exit(Fish root)
        {
            _root.StopAllCoroutines();
        }
        Fish _root;

        void OnTouchTarget(TargetPoint target) {
            // 判断该目标是否为鱼钩，如果是鱼钩则进入EatHook状态
            if (target.name == "hook")  _root.mStateMachine.ChangeState(State.EatHook);
        }

        /// <summary>
        /// 定时改变目标
        /// </summary>
        /// <returns></returns>
        IEnumerator ChangeTarget() {
            while (true) {
                TargetPoint target = SchoolFish.GetViewportTarget();
                if (target != null) _root.SetTarget(target, (TargetPoint tar) => OnTouchTarget(tar));
                yield return new WaitForSeconds(3.0f);
            }
        }
    }

    class StateEscape : IState<Fish> {
        public override void Enter(Fish root)
        {
            Debug.LogError("进入逃离状态");
            root.transform.Find("mesh").GetComponent<Renderer>().material.SetColor("_TintColor", Color.red);
            MMVibrationManager.Vibrate();

            _root = root;
            _targetPos = root.transform.position + new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized * 5;
            _root.StartCoroutine(Return2Standy());
        }
        public override void Execute(Fish root)
        {
            _root.swim(_targetPos, 1);
        }
        Vector3 _targetPos;
        Fish _root; 
        IEnumerator Return2Standy() {
            yield return new WaitForSeconds(5.0f);
            _root.mStateMachine.ChangeState(State.Standy);
        }
    }

    class StateEatHook : IState<Fish> {
        public override void Enter(Fish root)
        {
            Debug.LogError("进入吃鱼钩状态");
            root.transform.Find("mesh").GetComponent<Renderer>().material.SetColor("_TintColor", Color.green);

            EventMachine.Register(EventID.EventID_FishingRod, OnFishingRod);
            _root = root;
            _clock = Random.Range(2,7);

            _EatHook = EatHook();
            _root.StartCoroutine(_EatHook);
        }
        public override void Execute(Fish root)
        {
            // root.swim(root.mTarget.transform.position, 1);
            // 判断当前的目标的动量，如果动量大于某个值则进入Escape状态
            //if (root.mTarget.mTargetAccelerated > 0.1f) root.mStateMachine.ChangeState(State.Escape);

            // 倒计时3秒，超过三秒则逃脱
            _clock -= Time.deltaTime;
            if (_clock <= 0) _root.mStateMachine.ChangeState(State.Escape);
        }

        public override void Exit(Fish root)
        {
            EventMachine.Unregister(EventID.EventID_FishingRod, OnFishingRod);
            _root.StopCoroutine(_EatHook);
        }

        /// <summary>
        /// 鱼被钓到
        /// </summary>
        /// <param name="args"></param>
        void OnFishingRod(params object[] args) {
            if (!(bool)args[0]) {
                if(_isHock) _root.mStateMachine.ChangeState(State.End);
                else _root.mStateMachine.ChangeState(State.Escape);
            } 
        }
        Fish _root;
        float _clock;
        bool _isHock = false;
        /// <summary> 
        /// 播放鱼啄钩子的动画
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        IEnumerator EatHook() {
            Transform fish = _root.transform.GetChild(0);
            Vector3 original = fish.position + fish.right * 0.1f;
            Vector3 target = fish.position;
            Vector3 pos = fish.position;

            float clock = 1;
            while (clock >= 0) {
                float offset = _root.m_EatAnimationCurve.Evaluate(clock);
                fish.position = Vector3.Lerp(pos, original, offset);
                clock -= Time.deltaTime;
                yield return null;
            }

            while (true) {
                yield return new WaitForSeconds(Random.Range(0, 2));
                clock = 1; float speed = Random.Range(5.0f,10.0f);
                while (clock >= 0) {
                    float offset = _root.m_EatAnimationCurve.Evaluate(clock);
                    fish.position = Vector3.Lerp(original, target, offset);
                    clock -= Time.deltaTime * speed;
                    yield return null;
                }
                _isHock = true;

                switch (Random.Range(0, 3)) {
                    case 0: MMVibrationManager.Haptic(HapticTypes.LightImpact); break;
                    case 1: MMVibrationManager.Haptic(HapticTypes.MediumImpact); break;
                    case 2: MMVibrationManager.Haptic(HapticTypes.HeavyImpact); break;
                }
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                yield return new WaitForSeconds(Random.Range(1/speed,2/speed));
                _isHock = false;
                clock = 1;
                while (clock >= 0) {
                    float offset = _root.m_EatAnimationCurve.Evaluate(clock);
                    fish.position = Vector3.Lerp( target, original, offset);
                    clock -= Time.deltaTime * speed;
                    yield return null;
                }
            }
        } IEnumerator _EatHook;
    }

    class StateEnd : IState<Fish> {
        public override void Enter(Fish root)
        {
            Debug.LogError("鱼被钓起状态");
            _root = root;

            root.transform.parent = root.mTarget.transform;
            root.StartCoroutine(Return2Stanty());

            _SuccessHock = SuccessHock();
            _root.StartCoroutine(_SuccessHock);
        }

        IEnumerator Return2Stanty() {
            _root.StopCoroutine(_SuccessHock);
            yield return new WaitForSeconds(0.5f);
            EventMachine.SendEvent(GameCommon.EventID.EventID_FishingSuccess);
            yield return new WaitForSeconds(2.0f);
            Destroy(_root.gameObject);
        }

        IEnumerator SuccessHock() {
            while (true) {
                MMVibrationManager.Haptic(HapticTypes.Failure);
                yield return new WaitForSeconds(Random.Range(0.1f,0.5f));
            }
            
        }IEnumerator _SuccessHock;

        Fish _root;
    }
    #endregion
}
