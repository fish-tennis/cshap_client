using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 登录阶段的逻辑(玩家选择角色进入游戏服之前)
    internal class Login
    {
        // 登录过程中的一些临时变量
        private static Gserver.LoginRes s_LoginRes;

        // 账号登录返回
        public static void OnLoginRes(Gserver.LoginRes res, int err)
        {
            Console.WriteLine("OnLoginRes:" + res + " err:" + err);
            if(err == (int)Gserver.ErrorCode.NotReg)
            {
                Console.WriteLine("register a new account");
            }
            else if(err == 0)
            {
                s_LoginRes = res;
                // 账号登录成功,自动进游戏服,假设没有选区服的流程
                Client.Send(new Gserver.PlayerEntryGameReq{
                    AccountId = res.AccountId,
                    LoginSession = res.LoginSession,
                    RegionId = 1,
                });
            }
        }

        // 账号注册返回
        private static void OnAccountRes(Gserver.AccountRes res, int err)
        {
            Console.WriteLine("OnAccountRes:" + res + " err:" + err);
            if (err == 0)
            {
                Console.WriteLine("create a new account,try login again");
            }
        }

        private static void OnPlayerEntryGameRes(Gserver.PlayerEntryGameRes res, int err)
        {
            Console.WriteLine("OnPlayerEntryGameRes:" + res + " err:" + err);
            // 没角色,需要先创建一个
            if (err == (int)Gserver.ErrorCode.NoPlayer)
            {
                Console.WriteLine("create a new player");
                // 自动创建一个角色名=账号名的角色,如果创建失败,就需要手动创建了
                Client.Send(new Gserver.CreatePlayerReq
                {
                    AccountId = s_LoginRes.AccountId,
                    LoginSession = s_LoginRes.LoginSession,
                    RegionId = 1,
                    Name = s_LoginRes.AccountName,
                    Gender = 1,
                });
            }
            // 登录遇到问题,服务器提示客户端稍后重试
            else if (err == (int)Gserver.ErrorCode.TryLater)
            {
                // TODO: 延迟几秒后,再尝试登录
            }
            else if (err == 0)
            {
                // 玩家进入游戏服成功,创建本地Player对象
                Player player = new Player((int)res.PlayerId);
                player.Name = res.PlayerName;
                player.AccountId = (int)res.AccountId;
                player.RegionId = res.RegionId;
                player.InitComponents();
                Client.Instance.Player = player;
                Console.WriteLine("entry game id:" + player.GetId() + " name:" + player.Name);
            }
        }

        // 创建角色
        public static void OnCreatePlayerRes(Gserver.CreatePlayerRes res, int err)
        {
            Console.WriteLine("OnCreatePlayerRes:" + res + " err:" + err);
            if (err == 0)
            {
                // 创建了新角色,自动登录
                Client.Send(new Gserver.PlayerEntryGameReq
                {
                    AccountId = s_LoginRes.AccountId,
                    LoginSession = s_LoginRes.LoginSession,
                    RegionId = 1,
                });
            }
        }

    }
}
