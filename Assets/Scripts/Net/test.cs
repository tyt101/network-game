using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogError("```````");
    }
    public void RegistBtnReturn()
    {
        SceneManager.LoadScene("Login");
    }
    public void RegistBtnTo()
    {
        SceneManager.LoadScene("Regist");
    }
}
