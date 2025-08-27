using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 账号登录阶段的逻辑
    internal class Login
    {
        public static void OnLoginRes(Gserver.LoginRes res, int err)
        {
            Console.WriteLine("OnLoginRes:" + res + " err:" + err);
            if(err == (int)Gserver.ErrorCode.NotReg)
            {

            }
            else if(err == 0)
            {

            }
        }

        private static void OnAccountRes(Gserver.AccountRes res, int err)
        {
            Console.WriteLine("OnAccountRes:" + res + " err:" + err);
            if (err == 0)
            {
                //Client.Instance.m_Connection.Send(new Gserver.LoginReq
                //{
                //    AccountName = res.AccountName,
                //});
            }
        }

        public void OnTest()
        {

        }
    }
}
