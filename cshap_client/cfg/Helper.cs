using Google.Protobuf.Collections;
using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.cfg
{
    // 配置数据加载完成后,进行一些预处理,对原始配置数据进行一些加工,便于业务处理
    internal static class Helper
    {
        public static int MaxLevel; // 最高等级

        public static void AfterLoad()
        {
            levelAfterLoad();
            exchangeAfterLoad();
            questAfterLoad();
            QuestCfgHelper.AfterLoad();
            ActivityCfgHelper.AfterLoad();
        }

        // 等级表预处理,计算出最高等级
        private static void levelAfterLoad()
        {
            foreach (var v in DataMgr.LevelExps)
            {
                MaxLevel = Math.Max(MaxLevel, v.Level);
            }
            Console.WriteLine("MaxLevel:" + MaxLevel);
        }

        // 条件模板id+values,转换成ConditionCfg对象
        private static List<Gserver.ConditionCfg> convertConditionCfgs(RepeatedField<Gserver.CfgArgs> cfgArgs)
        {
            if (cfgArgs == null)
            {
                return null;
            }
            var conditions = new List<Gserver.ConditionCfg>();
            foreach(var arg in cfgArgs)
            {
                conditions.Add(convertConditionCfg(arg));
            }
            return conditions;
        }

        // 条件模板id+values,转换成ConditionCfg对象
        private static Gserver.ConditionCfg convertConditionCfg(Gserver.CfgArgs cfgArg)
        {
            if(!DataMgr.ConditionTemplateCfgs.TryGetValue(cfgArg.CfgId, out var conditionTemplate))
            {
                Console.WriteLine("convertConditionCfgErr cfgArg:" + cfgArg);
                return null;
            }
            var conditionCfg = new Gserver.ConditionCfg
            {
                Type = conditionTemplate.Type,
                Key = conditionTemplate.Key,
                Op = conditionTemplate.Op,
                ClientCheck = conditionTemplate.ClientCheck,
            };
            conditionCfg.Values.AddRange(cfgArg.Args.Clone());
            conditionCfg.Properties.Add(conditionTemplate.Properties.Clone());
            return conditionCfg;
        }

        // 进度模板配置id+进度值,转换成ProgressCfg对象
        private static Gserver.ProgressCfg convertProgressCfg(Gserver.CfgArg cfgArg)
        {
            if (!DataMgr.ProgressTemplateCfgs.TryGetValue(cfgArg.CfgId, out var progressTemplate))
            {
                Console.WriteLine("convertProgressCfgErr cfgArg:" + cfgArg);
                return null;
            }
            var progressCfg = new Gserver.ProgressCfg
            {
                Type = progressTemplate.Type,
                Total = cfgArg.Arg,
                NeedInit = progressTemplate.NeedInit,
                Event = progressTemplate.Event,
                ProgressField = progressTemplate.ProgressField,
            };
            progressCfg.IntEventFields.Add(progressTemplate.IntEventFields.Clone());
            progressCfg.StringEventFields.Add(progressTemplate.StringEventFields);
            progressCfg.Properties.Add(progressTemplate.Properties);
            return progressCfg;
        }

        // 兑换表预处理
        private static void exchangeAfterLoad()
        {
            foreach( var kvp in DataMgr.ExchangeCfgs)
            {
                kvp.Value.Conditions.AddRange(convertConditionCfgs(kvp.Value.ConditionTemplates));
            }
        }

        // 任务预处理
        private static void questAfterLoad()
        {
            foreach (var kvp in DataMgr.Quests)
            {
                kvp.Value.Conditions.AddRange(convertConditionCfgs(kvp.Value.ConditionTemplates));
                kvp.Value.Progress = convertProgressCfg(kvp.Value.ProgressTemplate);
            }
        }
        
        // 根据字段值创建索引字典
        public static Dictionary<int, Dictionary<int, T>> CreateDictionaryIndex<T>(Dictionary<int, T> data, Func<T, int> indexFn)
        {
            var indexDict = new Dictionary<int, Dictionary<int, T>>();
            foreach (var kvp in data)
            {
                var index = indexFn(kvp.Value);
                indexDict.TryGetValue(index, out var dict);
                if (dict == null)
                {
                    dict = new Dictionary<int, T>();
                    indexDict.Add(index, dict);
                }
                dict.Add(kvp.Key, kvp.Value);
            }
            return indexDict;
        }

    }
}
