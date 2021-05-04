using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public GameObject pcController;
    public GameObject mobileController;

    void Start()
    {
#if UNITY_ANDROID
        Instantiate(mobileController);
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        Instantiate(pcController);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
