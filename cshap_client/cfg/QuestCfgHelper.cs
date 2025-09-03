using System.Collections.Generic;
using cshap_client.game;

namespace cshap_client.cfg
{
    using QuestDict = Dictionary<int, Gserver.QuestCfg>;
    
    public static class QuestCfgHelper
    {
        public static Dictionary<int,QuestDict> QuestsByQuestType; // 任务类型索引
        public static Dictionary<int,QuestDict> QuestsByCategory; // 任务分类索引
        
        public static void AfterLoad()
        {
            QuestsByQuestType = Helper.CreateDictionaryIndex(DataMgr.Quests, (questCfg) => questCfg.QuestType);
            QuestsByCategory = Helper.CreateDictionaryIndex(DataMgr.Quests, (questCfg) => questCfg.Category);
        }
    }
}