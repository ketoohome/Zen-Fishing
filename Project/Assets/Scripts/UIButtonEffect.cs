using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour {

    public GameObject _Effect;
    public float _Interval = 1.0f, _Duration = 3.0f;
	// Use this for initialization
	void OnEnable () {
        StartCoroutine(CreateNode());
	}

    // TODO : Create a ImageNode by sometime
    IEnumerator CreateNode() {
        yield return new WaitForSeconds(1.0f);
        while (true) {
            GameObject node = GameObject.Instantiate<GameObject>(_Effect, _Effect.transform.parent);
            StartCoroutine(UpdateNode(node));
            yield return new WaitForSeconds(_Interval);
        }
    }

    // TODO : Update a Node
    IEnumerator UpdateNode(GameObject obj) {
        obj.SetActive(true);
        yield return new WaitForSeconds(_Duration);
        Destroy(obj);
    }

}
