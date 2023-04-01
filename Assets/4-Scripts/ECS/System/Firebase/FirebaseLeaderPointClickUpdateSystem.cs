using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;
using Util;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(NextCardSystem))]
    public partial class FirebaseLeaderPointClickUpdateSystem : UpdateSystem
    {
        private const int MaxScores = 20;
        private const float UpdateCycleTime = 60;

        private static DatabaseReference refLeaderboards;
        private static DatabaseReference root;

        private Entity timerUpdateEntity;
        private UICanvasController canvas;

        protected override void OnCreate()
        {
            // Get the root reference location of the database.
            UpdateReference();

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

        private void UpdateReference()
        {
            if (root != null)
            {
                root.ValueChanged -= UpdateViewScoreLeaders;
            }
            if(refLeaderboards != null)
            {
                refLeaderboards.ValueChanged -= UpdateViewScoreLeaders;
            }
            
            root = FirebaseDatabase.DefaultInstance.RootReference;
            if (root != null) refLeaderboards = root.Child("leaderboards");

            if (refLeaderboards != null)
                refLeaderboards.OrderByChild(SaveKeys.pointClickKey).ValueChanged += UpdateViewScoreLeaders;
            else root.ValueChanged += UpdateViewScoreLeaders;
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
                if (refLeaderboards == null)
                {
                    UpdateReference();
                    if(refLeaderboards == null) return;
                }

                TryAddOrUpdateScoreLeaders(data.UserState.FirebaseId, data.UserState.Nick, data.UserState.PointClick, refLeaderboards);

                //перезапуск таймера
                timer.TimeLeft = UpdateCycleTime;
                
            }).WithoutBurst().Run();
        }

        private void UpdateViewScoreLeaders(object sender, ValueChangedEventArgs args)
        {
            if (args == null || args.Snapshot == null || args.Snapshot.Value == null) return;
            
            var leaders = args.Snapshot.Value as List<object>;

            if (leaders == null)
            {
                UpdateUI();
                
                return;
            }

            var convert = new List<LeaderData>();

            foreach (var obj in leaders)
            {
                if (!(obj is Dictionary<string, object>)) continue;

                var score = (long)((Dictionary<string, object>)obj)[SaveKeys.pointClickKey];
                var id = (string)((Dictionary<string, object>)obj)[SaveKeys.firebaseIdKey];
                var nick = (string)((Dictionary<string, object>)obj)[SaveKeys.nickKey];

                convert.Add(new LeaderData(score, id, nick));
            }
            
            UpdateUI(convert);
        }

        private void UpdateUI(List<LeaderData> data = null)
        {
            var nick = data != null && data.Any() ? data.First().nick : $"!not found";
            var point = data != null && data.Any() ? data.First().pointClick : 0;
            
            var canvas = GetCanvas();
            
            if (canvas == null) return;

            Entities.WithAll<GameState>().ForEach((GameState gameState) =>
            {
                canvas.LeaderBoard.leaderText.text = $"{nick}";
                canvas.LeaderBoard.leaderPoint.text = $"{point.ToPrettyString()}";
            }).WithoutBurst().Run();
        }

        private void TryAddOrUpdateScoreLeaders(
            string curFirebaseId,
            string curNick,
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

                        if (firebaseId.Equals(curFirebaseId)) currentIndex = i;

                        if (childScore < minScore)
                        {
                            minScore = childScore;
                            minVal = child;
                        }
                    }

                    if (minScore >= score && mutableData.ChildrenCount >= MaxScores)
                    {
                        // The new score is lower or equals than the existing 5 scores, abort.
                        return TransactionResult.Abort();
                    }

                    // Remove the lowest score.
                    if (currentIndex < 0
                        && mutableData.ChildrenCount >= MaxScores) leaders.Remove(minVal);
                }

                // Add the new high score.
                Dictionary<string, object> newScoreMap =
                    new Dictionary<string, object>();

                newScoreMap[SaveKeys.pointClickKey] = score;
                newScoreMap[SaveKeys.firebaseIdKey] = curFirebaseId;
                newScoreMap[SaveKeys.nickKey] = curNick; 
                
                if (currentIndex < 0)
                {
                    leaders.Add(newScoreMap);
                }
                else
                {
                    var oldScore = (long)
                        ((Dictionary<string, object>)leaders[currentIndex])[SaveKeys.pointClickKey];
                    
                    if (oldScore == score)
                    {
                        return TransactionResult.Abort();
                    }
                    
                    leaders[currentIndex] = newScoreMap;
                }
                
                mutableData.Value = leaders;

                return TransactionResult.Success(mutableData);
            });
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
        public readonly string nick;
        
        public LeaderData(long pointClick, string id, string nick)
        {
            this.pointClick = pointClick;
            this.id = id;
            this.nick = nick;
        }
    }
}