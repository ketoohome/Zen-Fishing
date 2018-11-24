using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOOL;
using GameCommon;

public class UIRoot : MonoBehaviour {

    // Use this for initialization
    void Start() {
        InitilizeState();
    }

    void OnDestroy()
    {

    }



    #region State
    enum state{
        Menu,
        Game,
        End
    }

    IStateMachine<UIRoot> mStateMechine;

    void InitilizeState() {
        mStateMechine = new IStateMachine<UIRoot>(this);
        mStateMechine.Add(state.Menu, new StateMenu());
        mStateMechine.Add(state.Game, new StateGame());
        mStateMechine.Add(state.End, new StateEnd());

        mStateMechine.SetCurrentState(state.Menu);
    }
    class StateMenu : IState<UIRoot>
    {
        public override void Enter(UIRoot root) {
            _Root = root;
            EventMachine.Register(EventID.EventID_FishingRod, OnFishingRod);
            root.transform.Find("Menu").gameObject.SetActive(true);
        }

        public override void Exit(UIRoot root)
        {
            EventMachine.Unregister(EventID.EventID_FishingRod, OnFishingRod);
            root.StartCoroutine(Change2Game());
        }

        void OnFishingRod(params object[] args)
        {
            if ((bool)args[0]) _Root.mStateMechine.ChangeState(state.Game);
        }
        UIRoot _Root;

        IEnumerator Change2Game() {
            _Root.transform.Find("Menu").GetComponent<Animator>().Play("End");
            yield return new WaitForSeconds(0.45f);
            _Root.transform.Find("Menu").gameObject.SetActive(false);
        }
    }

    class StateGame : IState<UIRoot>
    {
        public override void Enter(UIRoot root)
        {
            _Root = root;
            root.transform.Find("Game").gameObject.SetActive(true);
            EventMachine.Register(EventID.EventID_FishingSuccess, OnFishingSuccess);
        }

        public override void Exit(UIRoot root)
        {
            EventMachine.Unregister(EventID.EventID_FishingSuccess, OnFishingSuccess);
            root.StartCoroutine(Change2End());
        }

        void OnFishingSuccess(params object[] args)
        {
            _Root.mStateMechine.ChangeState(state.End);
        }

        IEnumerator Change2End()
        {
            _Root.transform.Find("Game").GetComponent<Animator>().Play("End");
            yield return new WaitForSeconds(0.45f);
            _Root.transform.Find("Game").gameObject.SetActive(false);
        }
        UIRoot _Root;
    }

    class StateEnd : IState<UIRoot>
    {
        public override void Enter(UIRoot root)
        {
            _Root = root;
            root.transform.Find("End").gameObject.SetActive(true);
            EventMachine.Register(EventID.EventID_FishingRod, OnFishingRod);
        }

        public override void Exit(UIRoot root)
        {
            EventMachine.Unregister(EventID.EventID_FishingRod, OnFishingRod);
            root.StartCoroutine(Change2Game());
        }

        void OnFishingRod(params object[] args)
        {
            if ((bool)args[0]) _Root.mStateMechine.ChangeState(state.Game);
        }

        IEnumerator Change2Game()
        {
            _Root.transform.Find("End").GetComponent<Animator>().Play("End");
            yield return new WaitForSeconds(0.45f);
            _Root.transform.Find("End").gameObject.SetActive(false);
        }
        UIRoot _Root;
    }
    #endregion
}
