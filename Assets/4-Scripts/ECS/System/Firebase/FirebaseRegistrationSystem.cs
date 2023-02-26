/*using Cysharp.Threading.Tasks;
using Firebase;
using Game.Config;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class FirebaseRegistrationSystem : InitSystem
    {
        private UICanvasController canvas;
        private FirebaseApp app;
        private bool isInit;
        
        protected override void OnCreate()
        {
            CheckDependenciesAsync();
        }

        private async UniTask CheckDependenciesAsync()
        {
            var handle = await FirebaseApp.CheckDependenciesAsync();
            
            Debug.Log($"CheckDependenciesAsync status{handle}");

            if (handle == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                var entity = EntityManager.CreateEntity();

                EntityManager.AddComponentData(entity, new FirebaseReadyDependenciesTag());
            }
        } 

        protected override void OnUpdate()
        {
            if (!HasSingleton<InitAllFabricsTag>()) return;
            if (!HasSingleton<FirebaseReadyDependenciesTag>()) return;

            if (isInit) return;

            if (app == null) return;

            var appEntity = GetSingletonEntity<FirebaseReadyDependenciesTag>();

            var stateEntity = GetSingletonEntity<GameState>();
            
            if (stateEntity == Entity.Null) return;

            var dataState = EntityManager.GetComponentData<GameState>(stateEntity);
            
            if (dataState == null) return;

            if (!string.IsNullOrEmpty(dataState.UserState.Email))
            {
                isInit = true;

                var data = new FirebaseRegistrationData()
                {
                    email = dataState.UserState.Email,
                    pass = dataState.UserState.Pass
                };

                EntityManager.AddComponentData(appEntity, data);
                EntityManager.AddComponentData(appEntity, new FirebaseAppReadyTag());
            }

            if (!HasSingleton<UIRegistrationPanel>())
            {
                var canvas = GetCanvas();

                if (canvas == null) return;
                
                Entities.WithAll<FactoriesCardData>().ForEach((FactoriesCardData factories) =>
                {
                    var entityPanel = EntityManager.CreateEntity();
                    
                    var panel = factories.CreateRegInstance<UIRegistrationPanelFabric>(entityPanel, GetCanvas().root);
                }).WithStructuralChanges().WithoutBurst().Run();

            }

            Entities.WithAll<FirebaseRegistrationData>().ForEach((Entity e, FirebaseRegistrationData registration) =>
            {
                isInit = true;
                
                dataState.UserState.SetData(registration.email, registration.pass);
                
                dataState.UserState.SaveData();

                EntityManager.AddComponentData(e, new FirebaseAppReadyTag());
            }).WithStructuralChanges().WithoutBurst().Run();;
        }
        
        private UICanvasController GetCanvas()
        {
            if (canvas != null) return this.canvas;

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }
    }

    public struct FirebaseReadyDependenciesTag : IComponentData
    {
    }
    
    public struct FirebaseAppReadyTag : IComponentData
    {
    }
}*/