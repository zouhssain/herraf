using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Duration : MonoBehaviour
{
    public GameObject panel;
    public float duration=6;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }

    public IEnumerator Test()
    {
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}
