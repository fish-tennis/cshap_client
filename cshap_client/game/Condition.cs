using System;
using System.Collections.Generic;
using System.Linq;

using ConditionCheckFunc = System.Func<object, Gserver.ConditionCfg, bool>;

namespace cshap_client.game
{
    public static class Condition
    {
        // 条件类型和条件检查接口的映射表
        private static Dictionary<Gserver.ConditionType,ConditionCheckFunc> _checkers = new Dictionary<Gserver.ConditionType, ConditionCheckFunc>
        {
            [Gserver.ConditionType.PlayerPropertyCompare] = PlayerPropertyInt32Checker,
            [Gserver.ConditionType.ActivityPropertyCompare] = DefaultPropertyInt32Checker,
        };
        
        // 检查单个条件
        public static bool CheckCondition(object obj, Gserver.ConditionCfg conditionCfg)
        {
            if (!conditionCfg.ClientCheck)
            {
                // NOTE: 客户端无法直接判断的条件,默认返回true,由服务器来判断
                return true;
            }
            return _checkers.TryGetValue((Gserver.ConditionType)conditionCfg.Type, out var checker) && checker(obj, conditionCfg);
        }

        // 检查多个条件
        public static bool CheckConditions(object obj, IEnumerable<Gserver.ConditionCfg> conditionCfg)
        {
            return conditionCfg.All(condition => CheckCondition(obj, condition));
        }
        
        /// <summary>
        /// 值比较接口,比较符: = > >= < <= != [] ![]
        /// </summary>
        public static bool CompareOpValue(object obj, int compareValue, Gserver.ValueCompareCfg valueCompareCfg)
        {
            if (valueCompareCfg.Values.Count == 0)
            {
                Console.WriteLine("CompareOpValueErr:{0} {1}", compareValue, valueCompareCfg);
                return false;
            }

            switch (valueCompareCfg.Op)
            {
                case "=":
                    // 满足其中一个即可
                    return valueCompareCfg.Values.Any(value => compareValue == value);
                case ">":
                    return compareValue > valueCompareCfg.Values[0];
                case ">=":
                    return compareValue >= valueCompareCfg.Values[0];
                case "<":
                    return compareValue < valueCompareCfg.Values[0];
                case "<=":
                    return compareValue <= valueCompareCfg.Values[0];
                case "!=":
                    // 有一个相等就返回false
                    return valueCompareCfg.Values.All(value => compareValue != value);
                case "[]":
                    // 可以配置多个区间,如Args:[1,3,8,15]表示[1,3] [8,15]
                    for (int i = 0; i < valueCompareCfg.Values.Count; i+=2)
                    {
                        if (i + 1 < valueCompareCfg.Values.Count)
                        {
                            if (compareValue >= valueCompareCfg.Values[i] &&
                                compareValue <= valueCompareCfg.Values[i + 1])
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case "![]":
                    // 可以配置多个区间,如Args:[1,3,8,15]表示[1,3] [8,15]
                    for (int i = 0; i < valueCompareCfg.Values.Count; i+=2)
                    {
                        if (i + 1 < valueCompareCfg.Values.Count)
                        {
                            if (compareValue >= valueCompareCfg.Values[i] &&
                                compareValue <= valueCompareCfg.Values[i + 1])
                            {
                                return false;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return false;
        }
        
        // PropertyInt32的实现类的属性值比较接口
        public static bool DefaultPropertyInt32Checker(object obj, Gserver.ConditionCfg conditionCfg)
        {
            if (!(obj is IPropertyInt32 propertyGetter)) return false;
            var propertyName = conditionCfg.Key;
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            var propertyValue = propertyGetter.GetPropertyInt32(propertyName);
            var valueCompareCfg = new Gserver.ValueCompareCfg {Op = conditionCfg.Op};
            valueCompareCfg.Values.AddRange(conditionCfg.Values);
            return CompareOpValue(obj, propertyValue, valueCompareCfg);
        }

        // 玩家属性值比较条件检查器
        public static bool PlayerPropertyInt32Checker(object obj, Gserver.ConditionCfg conditionCfg)
        {
            // obj可能是Player,PlayerComponent,Activity等对象,解析出Player对象
            // 从而获取玩家的属性值
            var player = ParsePlayer(obj);
            return player != null && DefaultPropertyInt32Checker(player, conditionCfg);
        }

        // CheckConditions的obj参数可以传入Player,PlayerComponent,Activity等对象,
        // 会自动解析出Player对象,从而获取玩家的属性值
        public static Player ParsePlayer(object obj)
        {
            switch (obj)
            {
                case Player player:
                    return player;
                case IPlayerComponent component:
                    return component.GetPlayer();
                case Activity activity:
                    return activity.m_Activities.GetPlayer();
                default:
                    return null;
            }
        }
        
    }
}