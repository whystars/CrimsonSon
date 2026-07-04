# CrimsonSon

![LabAPI](https://img.shields.io/badge/LabAPI-1.1.7-blue)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-green)
![License](https://img.shields.io/badge/License-GPLv3-red)

CrimsonSon（深红之子）是一个基于 LabAPI 的 `SCP: Secret Laboratory` 服务器插件。插件为回合加入第三方阵营“深红之子”：他们需要保护教皇进入 `SCP-049` 或 `SCP-106` 收容室完成仪式，并通过仪式终结回合。

当前插件版本：`1.1.1`

## 玩法概览

回合开始后，插件会从 D 级人员中随机选择一名玩家成为 `SCP-999-B`。`SCP-999-B` 需要前往广播室，满足条件后会从观察者中召唤深红之子阵营。

深红阵营生成后，`HolyFather`（教皇）是仪式核心。教皇需要携带 `SCP1576` 前往 `SCP-049` 或 `SCP-106` 收容室并启动仪式。倒计时期间，教皇必须保持存活并维持有效身份；仪式完成后，插件会处理非深红阵营的存活玩家并请求结束回合。

完整规则见 [DESIGN.md](DESIGN.md)。

## 依赖

Release 默认只提供 `CrimsonSon.dll`。服务器需要提前安装或具备兼容版本的前置组件：

- `Northwood.LabAPI 1.1.7`
- `SCPSL-AudioManagerAPI 2.3.6`
- `HintServiceMeow 5.5.0`
- `.NET Framework 4.8.1`

`YamlDotNet 18.1.0` 等 NuGet 依赖用于构建和运行解析，具体版本以项目文件为准。请按各上游项目说明安装 LabAPI、音频和 HUD 前置插件；本项目发布包不重新分发这些依赖。

## 安装

适用环境：LabAPI 服务器。

常用插件目录：

- 全局插件：`%AppData%\SCP Secret Laboratory\LabAPI\plugins\global\`
- 指定端口：`%AppData%\SCP Secret Laboratory\LabAPI\plugins\{port}\`

如果服务器启用了 `hoster_policy.txt`，并设置 `gamedir_for_configs: true`，LabAPI 的数据目录会改到服务器目录下的 `AppData\`。

安装步骤：

1. 安装并确认 `LabAPI`、`SCPSL-AudioManagerAPI`、`HintServiceMeow` 可正常加载。
2. 下载对应版本的 `CrimsonSon.dll`。
3. 将 `CrimsonSon.dll` 放入 LabAPI 的 `plugins\global` 或 `plugins\{port}` 目录。
4. 启动服务器一次，让插件生成默认配置和翻译文件。
5. 按需修改生成出来的 `config.yml` 与 `translations.yml`。

配置文件位置和升级处理方式见 [CONFIG_GUIDE.md](CONFIG_GUIDE.md)。

## 配置

CrimsonSon 会生成两份 YAML 文件：

- `config.yml`：召唤人数、仪式时长、出生房间、音量、HUD 坐标、角色人数上限、权重和血量。
- `translations.yml`：提示文本、CASSIE 文案、倒计时文案、角色名称和角色描述。

角色默认物品目前由插件内置，不通过配置文件修改。

## 管理命令

插件提供一个 Remote Admin 命令用于测试或手动调整深红角色：

```text
csrole list
csrole [玩家名称/玩家ID] [角色编号]
```

命令别名：

```text
CrimsonSonRole
CrimsonRole
```

## 从源码构建

项目目标框架为 `.NET Framework 4.8.1`，语言版本为 `C# 13.0`。

常用构建命令：

```powershell
dotnet build CrimsonSon.sln -c Debug
dotnet build CrimsonSon.sln -c Release
```

源码构建需要准备项目引用到的 SCP:SL、LabAPI 和相关插件 DLL。公开仓库不会提交本地服务器引用目录、NuGet 还原目录或构建产物。

## 已知说明

- 插件调用游戏原生回合结束逻辑请求结束回合，最终结算面板显示由 SCP:SL 原生逻辑决定。
- `SCPSL-AudioManagerAPI` 会生成自己的音频配置文件；那不是 CrimsonSon 的玩法配置。
- CrimsonSon 的音频资源已嵌入插件 DLL，不需要额外复制 `Audio` 目录作为发布资产。

## 许可证

本项目使用 `GNU GPL v3` 协议开源。

## 参考

- [LabAPI](https://github.com/Northwood-Studios/LabAPI)
- [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow)
- [SCP:SL 插件开发指南（非官方）](https://en.scpslgame.com/index.php?title=Plugins)
