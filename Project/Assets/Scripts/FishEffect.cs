using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOOL;
using GameCommon;

public class FishEffect : MonoBehaviour
{
    Vector3 _ForwardTarget;
    Transform _Fish;
    Animator _Anim;

    enum state
    {
        standy,
        swim,
        turnleft,
        turnright,
        eathook,
        end,
    } state _state;

    state State
    {
        set
        {
            if (_state == value) return;
            _state = value;
            switch (_state)
            {
                case state.standy: _Anim.Play("standy"); break;
                case state.swim: _Anim.Play("swim"); break;
                case state.turnleft: _Anim.Play("turnleft"); _state = state.swim; break;
                case state.turnright: _Anim.Play("turnright"); _state = state.swim; break;
                case state.eathook: _Anim.Play("touch"); break;
                case state.end: _Anim.Play("jump"); break;
            }
        }
    }
    IStateMachine<FishEffect> mStateMachine;

    private void Awake()
    {
        _Fish = transform.GetChild(0);
        _Anim = transform.Find("mesh/mesh").GetComponent<Animator>();

        EventMachine.Register(EventID.EventID_EatHook, OnEatHook);
        EventMachine.Register(EventID.EventID_PullFish, OnPullFish);
        EventMachine.Register(EventID.EventID_UnHook, OnUnHook);
    }

    private void OnDestroy()
    {
        EventMachine.Unregister(EventID.EventID_EatHook, OnEatHook);
        EventMachine.Unregister(EventID.EventID_PullFish, OnPullFish);
        EventMachine.Unregister(EventID.EventID_UnHook, OnUnHook);
    }

    void OnEatHook(params object[] args)
    {
        if (gameObject != (GameObject)args[0]) return;
        mStateMachine.ChangeState(state.eathook);
    }

    void OnPullFish(params object[] args)
    {
        if (gameObject != (GameObject)args[0]) return;
        mStateMachine.ChangeState(state.end);
    }

    void OnUnHook(params object[] args)
    {
        if (gameObject != (GameObject)args[0]) return;
        mStateMachine.ChangeState(state.standy);
    }

    private void Start()
    {
        mStateMachine = new IStateMachine<FishEffect>(this);
        mStateMachine.Add(state.standy,new StateStandy());
        mStateMachine.Add(state.eathook, new StateEatHook());
        mStateMachine.Add(state.end, new StateEnd());
        mStateMachine.SetCurrentState(state.standy);
    }

    // Update is called once per frame
    void Update() {
        mStateMachine.Update();
    }

    class StateStandy : IState<FishEffect>
    {
        public override void Execute(FishEffect root)
        {
            if (Camera.main == null) return;
            _CurrPoint = Camera.main.WorldToScreenPoint(root.transform.position);

            _Interval = _CurrPoint.x - _OldPoint.x;
            if (_Interval > 1) root._ForwardTarget = -Camera.main.transform.forward;
            else if (_Interval < -1) root._ForwardTarget = Camera.main.transform.forward;

            // Swim
            if (Mathf.Abs(_Interval) > 0.1f)
            {
                if (_OldInterval >= 0 && _Interval <= 0) root.State = state.turnleft;
                else if (_OldInterval <= 0 && _Interval > 0) root.State = state.turnright;
                else if (Mathf.Abs(_Interval) > 0.1f) root.State = state.swim;
            }
            // speed
            _CurrPos = root.transform.position;
            float dis = Vector3.Distance(_CurrPos, _OldPos);
            root._Anim.speed = Mathf.Clamp(dis * 50, 0.05f, 3);

            _OldInterval = _Interval;
            _OldPoint = _CurrPoint;
            _OldPos = _CurrPos;

            Quaternion targetquat = Quaternion.LookRotation(root._ForwardTarget, Camera.main.transform.up);
            root._Fish.rotation = Quaternion.Lerp(root._Fish.rotation, targetquat, Time.deltaTime * 5);
        }

        Vector3 _OldPoint, _CurrPoint, _OldPos, _CurrPos;
        float _OldInterval, _Interval;
    }

    class StateEatHook : IState<FishEffect>
    {
        public override void Enter(FishEffect root)
        {
            root.State = state.eathook;
        }
    }

    class StateEnd : IState<FishEffect>
    {
        public override void Enter(FishEffect root)
        {
            root.State = state.end;
            root._Anim.speed = 1;
        }
    }

}
