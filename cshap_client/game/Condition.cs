using System;
using System.Linq;

namespace cshap_client.game
{
    public class Condition
    {

        public bool CompareOpValue(object obj, int compareValue, Gserver.ValueCompareCfg valueCompareCfg)
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
    }
}