# cshap_client
c# test client for [gserver](https://github.com/fish-tennis/gserver)

c#写的测试客户端(控制台程序) 演示和gserver的交互流程

可以将c#代码直接拷贝到unity工程中,以开发和gserver适配的客户端

## 演示功能
- 网络协议的消息号自动生成
- 消息回调的自动注册
- (TODO)客户端直连模式和网关模式可选
- (TODO)配置数据管理模块
- 采用Entity-Component设计,模块解耦
- (TODO)Entity事件分发
- (TODO)任务模块,演示了如何实现一个通用且扩展性强的[任务系统](https://github.com/fish-tennis/gserver/blob/main/Design_Quest.md)
- (TODO)活动模块,演示了如何设计一个通用且支持扩展的活动模块
- (TODO)背包模块,演示了如何设计一个通用且支持扩展的容器模块

## 控制台输入protobuf
编译运行后,在控制台输入命令,可以实现和gserver的交互

在控制台输入如下格式的字符串,按回车,就会自动发送proto消息给gserver

格式为: messageName fieldName fieldValue fieldName fieldValue ...

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

## 控制台输入测试命令
格式为: @testcmd,如
```
@AddExp 100
```
就会自动组装TestCmd(Cmd=AddExp)消息,并发给gserver,效果等同于
```c#
Client.Send(new Gserver.TestCmd{
    Cmd = "AddExp 100",
});
```
gserver目前支持的测试命令: https://github.com/fish-tennis/gserver/blob/main/game/test_cmd.go