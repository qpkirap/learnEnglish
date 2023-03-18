using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(NextCardSystem))]
    public partial class FirebaseLeaderPointClickUpdateSystem : UpdateSystem
    {
        private const int MaxScores = 20;
        private const float UpdateCycleTime = 60;

        private DatabaseReference refLeaderboards;

        private Entity timerUpdateEntity;

        protected override void OnCreate()
        {
            // Get the root reference location of the database.
            refLeaderboards = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboards");

            refLeaderboards.OrderByChild(SaveKeys.pointClickKey).ValueChanged += GetScoreToLeaders;

            timerUpdateEntity = EntityManager.CreateEntity();

            //При первом обращении (после запуска) обновляем сразу
            var timer = new Timer()
            {
                TotalTime = 10,
                TimeLeft = 0,
                Timescale = 1,
                IsScalable = false,
                IsPaused = false
            };

            EntityManager.AddComponentData(timerUpdateEntity, timer);
            EntityManager.AddComponentData(timerUpdateEntity, new TimerPointClickTag());
        }

        protected override void OnUpdate()
        {
            //обновляеть данные в бд
            if (!HasSingleton<GameState>()) return;
            if (!HasSingleton<TimerPointClickTag>()) return;
            if (!HasSingleton<FirebaseRegistrationCompleteTag>()) return;
            
            var stateEntity = GetSingletonEntity<GameState>();

            var data = EntityManager.GetComponentData<GameState>(stateEntity);

            Entities.WithAll<TimerPointClickTag, Timer>().ForEach((Entity e, ref Timer timer) =>
            {
                if (!timer.IsCompleted) return;

                TryAddOrUpdateScoreLeaders(data.UserState.FirebaseId, data.UserState.PointClick, refLeaderboards);

                //перезапуск таймера
                timer.TimeLeft = UpdateCycleTime;
                
            }).WithoutBurst().Run();
        }

        private void GetScoreToLeaders(object sender, ValueChangedEventArgs args)
        {
            var leaders = args.Snapshot.Value as List<object>;

            if (leaders == null) return;

            var convert = new List<LeaderData>();

            foreach (var obj in leaders)
            {
                if (!(obj is Dictionary<string, object>)) continue;

                var score = (long)((Dictionary<string, object>)obj)[SaveKeys.pointClickKey];
                var id = (string)((Dictionary<string, object>)obj)[SaveKeys.firebaseIdKey];
                
                convert.Add(new LeaderData(score, id));
            }
            
            UpdateUI(convert);
        }

        private void UpdateUI(List<LeaderData> data)
        {
            if (!data.Any()) return;
            
            var item = data.First();
            
            if(item == null) return;
            
            Entities.WithAll<LeaderBoardController>().ForEach((LeaderBoardController controller) =>
            {
                if (controller != null)
                {
                    controller.leaderText.text = $"{item.id.Substring(0, 10)}";
                    controller.leaderPoint.text = $"{item.pointClick.ToString()}";
                }
            }).WithoutBurst().Run();
        }

        private void TryAddOrUpdateScoreLeaders(
            string firebaseId,
            long score,
            DatabaseReference leaderBoardRef)
        {
            leaderBoardRef.RunTransaction(mutableData =>
            {
                var leaders = mutableData.Value as List<object>;
                var currentIndex = -1;

                if (leaders == null)
                {
                    leaders = new List<object>();
                }
                else 
                {
                    var minScore = long.MaxValue;
                    object minVal = null;

                    for (var i = 0; i < leaders.Count; i++)
                    {
                        var child = leaders[i];
                        if (!(child is Dictionary<string, object>)) continue;

                        var childScore = (long)
                            ((Dictionary<string, object>)child)[SaveKeys.pointClickKey];
                        var firebaseId = (string)
                            ((Dictionary<string, object>)child)[SaveKeys.firebaseIdKey];

                        if (firebaseId.Equals(firebaseId)) currentIndex = i;

                        if (childScore < minScore)
                        {
                            minScore = childScore;
                            minVal = child;
                        }
                    }

                    if (minScore >= score)
                    {
                        // The new score is lower or equals than the existing 5 scores, abort.
                        return TransactionResult.Abort();
                    }

                    // Remove the lowest score.
                    if (currentIndex < 0 && mutableData.ChildrenCount >= MaxScores) leaders.Remove(minVal);
                }

                // Add the new high score.
                Dictionary<string, object> newScoreMap =
                    new Dictionary<string, object>();

                newScoreMap[SaveKeys.pointClickKey] = score;
                newScoreMap[SaveKeys.firebaseIdKey] = firebaseId;
                
                if (currentIndex < 0)
                {
                    leaders.Add(newScoreMap);
                }
                else
                {
                    leaders[currentIndex] = newScoreMap;
                }
                
                mutableData.Value = leaders;

                return TransactionResult.Success(mutableData);
            });
        }
    }

    /// <summary>
    /// Метка таймера для обновления таблиц лидеров
    /// </summary>
    public struct TimerPointClickTag : IComponentData
    {
    }

    public class LeaderData
    {
        public readonly long pointClick;
        public readonly string id;
        
        public LeaderData(long pointClick, string id)
        {
            this.pointClick = pointClick;
            this.id = id;
        }
    }
}