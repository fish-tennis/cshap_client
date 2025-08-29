using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 玩家的任务组件
    public class Quest : BasePlayerComponent
    {
        public const string ComponentName = "Quest";
        public Dictionary<int, Gserver.FinishedQuestData> Finished; // 已完成的任务
        public Dictionary<int, Gserver.QuestData> Quests; // 当前任务列表

        public Quest(Player player) : base(ComponentName, player)
        {
            Finished = new Dictionary<int, Gserver.FinishedQuestData>();
            Quests = new Dictionary<int, Gserver.QuestData>();
        }

        // 同步数据
        public void OnQuestSync(Gserver.QuestSync res)
        {
            Finished.Clear();
            Quests.Clear();
            foreach (var kvp in res.Finished)
            {
                Finished[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in res.Quests)
            {
                Quests[kvp.Key] = kvp.Value;
            }
            Console.WriteLine("OnQuestSync: Finished:" + Finished.Count + " Quests:" + Quests.Count);
        }

        // 任务更新
        public void OnQuestUpdate(Gserver.QuestUpdate res)
        {
            Quests[res.QuestCfgId] = res.Data;
            Console.WriteLine("OnQuestUpdate:" + res.Data);
        }

        // 删除一个任务
        public void OnQuestRemoveRes(Gserver.QuestRemoveRes res)
        {
            Quests.Remove(res.QuestCfgId);
            Console.WriteLine("OnQuestRemoveRes:" + res);
        }

        // 任务完成
        public void OnFinishQuestRes(Gserver.FinishQuestRes res)
        {
            Quests.Remove(res.QuestCfgId);
            Finished.Add(res.QuestCfgId, new Gserver.FinishedQuestData
            {
                Timestamp = res.FinishedQuestData.Timestamp,
            });
            Console.WriteLine("OnFinishQuestRes:" + res);
        }

        // 向服务器发送完成任务的请求(领取任务奖励) (TODO: 改成支持批量完成)
        public void FinishQuestReq(int questCfgId)
        {
            Client.Send(new Gserver.FinishQuestReq
            {
                QuestCfgId = questCfgId,
            });
        }

        // 批量完成任务
        public void FinishQuestReqBatch(List<int> questCfgIds)
        {
            foreach (var questCfgId in questCfgIds)
            {
                Client.Send(new Gserver.FinishQuestReq
                {
                    QuestCfgId = questCfgId,
                });
            }
        }
    }
}
