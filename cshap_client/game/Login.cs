using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 登录阶段的逻辑(玩家选择角色进入游戏服之前)
    internal class Login
    {
        // 登录过程中的一些临时变量
        public static string s_AccountName;
        public static string s_Password;
        private static Gserver.LoginRes s_LoginRes;

        // 账号协议不要发明文密码,而且要加一些混淆词,防止被"撞库"
        public static string GetMd5Password(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                // 1. 将字符串转换为字节数组（UTF8编码）
                byte[] inputBytes = Encoding.UTF8.GetBytes(password + "gserver");
                // 2. 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                // 3. 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2"表示两位小写十六进制
                }
                return sb.ToString();
            }
        }

        // 自动账号登录
        public static void AutoLogin()
        {
            Client.Send(new Gserver.LoginReq
            {
                AccountName = Login.s_AccountName,
                Password = Login.GetMd5Password(Login.s_Password),
            });
        }

        // 账号登录返回
        public static void OnLoginRes(Gserver.LoginRes res, int err)
        {
            Console.WriteLine("OnLoginRes:" + res + " err:" + err);
            if(err == (int)Gserver.ErrorCode.NotReg)
            {
                Console.WriteLine("register a new account");
                if (!string.IsNullOrEmpty(s_AccountName))
                {
                    // 自动注册
                    Client.Send(new Gserver.AccountReg
                    {
                        AccountName = Login.s_AccountName,
                        Password = Login.GetMd5Password(Login.s_Password),
                    });
                }
            }
            else if(err == 0)
            {
                s_LoginRes = res;
                Client.Instance.Player = null;
                // 账号登录成功,自动进游戏服,假设没有选区服的流程
                Client.Send(new Gserver.PlayerEntryGameReq{
                    AccountId = res.AccountId,
                    LoginSession = res.LoginSession,
                    RegionId = 1,
                });
                // TODO: 非网关模式,需要断开和登录服的连接,重新建立和游戏服的连接
            }
        }

        // 账号注册返回
        private static void OnAccountRes(Gserver.AccountRes res, int err)
        {
            Console.WriteLine("OnAccountRes:" + res + " err:" + err);
            if (err == 0)
            {
                if (!string.IsNullOrEmpty(s_AccountName))
                {
                    AutoLogin();
                }
                else
                {
                    Console.WriteLine("create a new account,try login again");
                }
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
