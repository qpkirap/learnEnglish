using System;
using InitGame.GameResources.Base;
using Unity.Entities;
using UnityEngine;


public class TestController : MonoBehaviour, IEntitySharedGameObjectConfig
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ISharedComponentData CreateSharedComponentData(ref Entity entity)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ComponentType ctx = ComponentType.ReadWrite<TestControllerSharedData>();
        var data = new TestControllerSharedData()
        {
            _testController = this
        };
        entityManager.AddSharedComponentData(entity, data);
        return data;
    }

    public MonoBehaviour monoBehaviour => this;
}

public struct TestControllerSharedData : ISharedComponentData, IEquatable<TestControllerSharedData>
{
    public TestController _testController;

    public bool Equals(TestControllerSharedData other)
    {
        return Equals(_testController, other._testController);
    }

    public override bool Equals(object obj)
    {
        return obj is TestControllerSharedData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (_testController != null ? _testController.GetHashCode() : 0);
    }
}
