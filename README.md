# CrimsonSon

![LabAPI](https://img.shields.io/badge/LabAPI-1.1.5-blue)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-green)
![License](https://img.shields.io/badge/License-GPLv3-red)

CrimsonSon（深红之子）是一个给 `SCP: Secret Laboratory` 服务器使用的阵营玩法插件。

它基于 `Northwood.LabAPI` 开发，为回合加入第三方阵营“深红之子”。这套玩法的目标不是常规交战，而是保护教皇完成仪式，在 `SCP-049` 或 `SCP-106` 收容室召唤深红之王，并直接终结回合。

## 当前状态

当前仓库代码版本按项目现状记为 `1.1.0`。

已经完成的部分：

- 本地可用 `dotnet build CrimsonSon.sln -c Debug` 构建
- 仪式倒计时已改为配置项，不再依赖音频长度
- `CASSIE`、召唤、仪式音频已经接入
- 角色人数上限、权重、血量、音量、房间、HUD 坐标已支持配置覆盖
- 角色名和角色描述已支持通过 `translations.yml` 覆盖

还没做的验证：

- 还没有做真实服务器联机测试
- `Round.End(true)` 只能结束回合，不能精确指定“深红阵营胜利队”

## 玩法流程

完整设计见 [DESIGN.md](DESIGN.md)。

简化流程是这样：

1. 回合开始后，从 `ClassD` 中随机选一名玩家变成 `SCP-999-B`
2. `SCP-999-B` 进入广播室后，尝试从观察者中召唤深红之子阵营
3. 深红阵营中的 `HolyFather` 在 `049` 或 `106` 收容室使用 `SCP1576`，开始仪式
4. 仪式持续期间会显示全服倒计时，并阻止 `SCP1576` 被拾取
5. 仪式完成后，会清除非深红阵营存活玩家并结束回合

## 依赖

- `Northwood.LabAPI 1.1.5`
- `SCPSL-AudioManagerAPI 2.0.2`
- `HintServiceMeow 5.5.0`
- `YamlDotNet 16.3.0`
- `.NET Framework 4.8.1`
- `C# 13.0`

## 构建

本地构建：

```powershell
dotnet build CrimsonSon.sln -c Debug
dotnet build CrimsonSon.sln -c Release
```

注意：

公开仓库不会提交 `using/` 下的本地游戏引用，所以你直接克隆后不一定能立刻通过编译。需要你自己补齐对应的本地引用 DLL。

## 安装

1. 构建得到 `CrimsonSon.dll`
2. 放入服务器对应的插件目录
3. 首次启动后让插件自动生成 `config.yml` 和 `translations.yml`
4. 根据需要修改配置，再重启或重载

## 配置说明

当前已经支持配置化的主要内容有：

- 召唤最少观察者人数
- 最大召唤人数
- 仪式持续时间
- 召唤 / 仪式 / Cassie 音量
- `SpawnRoom`
- `Spawn999BRoom`
- 角色介绍、倒计时、通知三类 HUD 的坐标和字号
- 各角色的 `MaxPlayers`、`Weight`、`Health`

角色默认物品目前还是硬编码，没有整套配置化。

## 翻译说明

插件会生成 `translations.yml`。

目前支持覆盖：

- 命令提示
- 召唤成功 / 失败提示
- 观察者不足提示
- 仪式开始 / 中断提示
- Cassie 广播文案
- 倒计时 HUD 文案
- 终局击杀文案
- 各角色名称和描述

角色描述支持 `{Name}` 占位符。

## 开源说明

本仓库使用 `GNU GPL v3` 协议开源。

为了避免把游戏本体或本地服务器文件一起再分发，仓库不会提交 `using/` 这类本地引用目录，也不会提交 `bin/`、`obj/`、`.vs/`、`packages/` 等构建产物或还原目录。

如果你要在自己的环境里重新编译，请自行准备对应的本地引用。

## 参考

- [LabAPI](https://github.com/Northwood-Studios/LabAPI)
- [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow)
- [SCP:SL 插件开发指南（非官方）](https://en.scpslgame.com/index.php?title=Plugins)
