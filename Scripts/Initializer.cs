using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Initializer : MonoBehaviour
{
    public List<GameObject> preInitObjects;
    public List<GameObject> normalInitObjects;
    public List<GameObject> postInitObjects;

    public void StartInitialization()
    {
        preInitObjects.ForEach(o => o.GetComponent<IInitializable>().PreInit());
        normalInitObjects.ForEach(o => o.GetComponent<IInitializable>().Init());
        postInitObjects.ForEach(o => o.GetComponent<IInitializable>().PostInit());
    }

    private void Start()
    {
        StartInitialization();
    }
}
