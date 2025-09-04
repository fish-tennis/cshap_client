# cshap_client
c# test client for [gserver](https://github.com/fish-tennis/gserver)

c#写的测试客户端(控制台程序),演示和gserver的交互流程,并演示了核心的业务接口

可以将c#代码直接拷贝到unity工程中,以开发和gserver适配的客户端

## 演示功能
- 网络协议的消息号自动生成
- 消息回调的自动注册
- 客户端支持TCP和WebSocket
- 配置数据管理模块
- 采用Entity-Component设计,模块解耦
- (TODO)Entity事件分发
- 通用且扩展性强的条件接口
- 任务模块,演示了如何实现一个通用且扩展性强的任务系统[设计文档](/Design_Quest.md)
- 兑换模块,演示了如何实现一个通用且扩展性强的兑换功能
- 活动模块,演示了如何设计一个通用且支持扩展的活动模块
- 背包模块,演示了如何设计一个通用且支持扩展的容器模块

## 控制台输入protobuf
编译运行后,在控制台输入命令,可以实现和gserver的交互

在控制台输入如下格式的字符串,按回车,就会自动发送proto消息给gserver(暂不支持repeated,map,struct等复杂字段)

格式为: messageName fieldName1 fieldValue1 fieldName2 fieldValue2 ...

如账号登录请求的proto:
```protobuf
message LoginReq {
  string accountName = 1;
  string password = 2;
}
```
在控制台输入如下字符串
```
LoginReq accountName test password 123
```
就会自动组装LoginReq(accountName=test,password=123)消息,并发给gserver,效果等同于
```c#
Client.Send(new Gserver.LoginReq{
    AccountName = "test",
    Password = "123",
});
```

## 测试命令(GM命令)
控制台输入字符串以@开头 表示向服务器发送测试命令,如
```
@AddExp 100
```
gserver目前支持的测试命令: https://github.com/fish-tennis/gserver/blob/main/game/test_cmd.go

- 加经验: @AddExp 经验值
- 加物品: @AddItem 物品配置id 物品数量[可选] 限时秒数[可选]
- 完成所有任务: @FinishQuest all
- 完成某个任务: @FinishQuest 任务配置id
- 兑换请求: @Exchange 兑换配置id 数量
- 添加所有活动: @AddActivity all
- 添加某个活动: @AddActivity 活动配置id
- 模拟一个战斗事件: @Fight 是否是Pvp[可选] 是否获胜[可选]
- 分发一个事件: @FireEvent 事件名称 事件字段名 事件字段值

## 命令行参数
```shell
-server 127.0.0.1:10001 -account test -password 123
```

如果server是ws://127.0.0.1:10001/ws或者wss://127.0.0.1:10001/wss的格式,则表示使用Websocket
```shell
-server ws://127.0.0.1:10001/ws -account test -password 123
```
命令行参数可以设置账号名和密码,运行控制台程序,将会自动登录

## 配置表和proto
https://github.com/fish-tennis/gserver
- 配置表及导表工具: gserver/excel
- proto及生成工具: gserver/proto

## 其他版本的客户端
golang测试客户端[gtestclient](https://github.com/fish-tennis/gtestclient)
