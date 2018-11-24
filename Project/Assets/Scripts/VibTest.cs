using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VibTest : MonoBehaviour {

    public void OnChangeScene() {
        if(SceneManager.GetActiveScene().name != "NiceVibrationsDemo")
            SceneManager.LoadScene("NiceVibrationsDemo");
        else SceneManager.LoadScene("SampleScene");
    }
}
