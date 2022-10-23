using System.Collections;
using System.Collections.Generic;
using InitGame.GameResources.Adressables;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private AdressablesSharedGOFactory testFactory;
    void Start()
    {
        var test = testFactory.GetFactoryConfig;
        var test2 = testFactory.GetTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
