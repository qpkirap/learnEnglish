using System.Collections.Generic;
using System.Linq;
using Game.ECS.System;

namespace Game.Leaders
{
    public class LeadersState
    {
        public List<LeaderData> LeaderDatas { get; private set; }

        public void UpdateLeaders(List<LeaderData> data = null)
        {
            if (LeaderDatas != null && data == null) return;
            
            this.LeaderDatas = data?.ToList();
        }
    }
}