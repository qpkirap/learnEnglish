using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    private static EntityManager manager =  World.DefaultGameObjectInjectionWorld.EntityManager;
    
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator CheckCardSize()
    {
        yield return new WaitForSeconds(2);
        
       // var allEntities = manager.GetAllEntities();
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
