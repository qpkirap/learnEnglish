using System.Collections.Generic;
using Firebase.Database;

namespace Game.ECS.System
{
    public class FirebaseLeaderPointClickUpdateSystem : UpdateSystem
    {
        private const int MaxScores = 20;
        
        private DatabaseReference refLeaderboards;
        
        protected override void OnCreate()
        {
            // Get the root reference location of the database.
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboards");

            refLeaderboards.OrderByChild(SaveKeys.pointClickKey).ValueChanged += GetScoreToLeaders;

        }
        
        protected override void OnUpdate()
        {
            
        }

        private void GetScoreToLeaders(object sender, ValueChangedEventArgs args)
        {
            var leaders = args.Snapshot.Value as List<object>;
            
            if (leaders == null) return;

            foreach (var obj in leaders)
            {
                if (!(obj is Dictionary<string, object>)) continue;
                
                var score = (long)((Dictionary<string, object>)obj)[SaveKeys.pointClickKey];
            }
        }
        
        private void AddScoreToLeaders(
            string firebaseId, 
            long score,
            DatabaseReference leaderBoardRef) {

            leaderBoardRef.RunTransaction(mutableData =>
            {
                var leaders = mutableData.Value as List<object>;

                if (leaders == null) {
                    leaders = new List<object>();
                } 
                else if (mutableData.ChildrenCount >= MaxScores) 
                {
                    long minScore = long.MaxValue;
                    object minVal = null;
                    
                    foreach (var child in leaders) 
                    {
                        if (!(child is Dictionary<string, object>)) continue;
                        
                        long childScore = (long)
                            ((Dictionary<string, object>)child)[SaveKeys.pointClickKey];
                        
                        if (childScore < minScore) 
                        {
                            minScore = childScore;
                            minVal = child;
                        }
                    }
                    
                    if (minScore > score) 
                    {
                        // The new score is lower than the existing 5 scores, abort.
                        return TransactionResult.Abort();
                    }

                    // Remove the lowest score.
                    leaders.Remove(minVal);
                }

                // Add the new high score.
                Dictionary<string, object> newScoreMap =
                    new Dictionary<string, object>();
                
                newScoreMap[SaveKeys.pointClickKey] = score;
                newScoreMap[SaveKeys.firebaseIdKey] = firebaseId;
                
                leaders.Add(newScoreMap);
                
                mutableData.Value = leaders;
                
                return TransactionResult.Success(mutableData);
            });
        }
    }
}