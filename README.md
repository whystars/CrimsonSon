# CrimsonSon

![LabAPI](https://img.shields.io/badge/LabAPI-1.1.7-blue)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-green)
![License](https://img.shields.io/badge/License-GPLv3-red)

CrimsonSon（深红之子）是一个给 `SCP: Secret Laboratory` 服务器使用的阵营玩法插件。

它基于 `Northwood.LabAPI` 开发，为回合加入第三方阵营“深红之子”。这套玩法的目标不是常规交战，而是保护教皇完成仪式，在 `SCP-049` 或 `SCP-106` 收容室召唤深红之王，并直接终结回合。

## 当前状态

当前仓库代码版本按项目现状记为 `1.1.1`。

已经完成的部分：

- `1.1.1` 已完成本地 `Release` 编译
- 项目内依赖版本声明已经同步刷新
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

- `Northwood.LabAPI 1.1.7`
- `SCPSL-AudioManagerAPI 2.3.6`
- `HintServiceMeow 5.5.0`
- `YamlDotNet 18.1.0`
- `.NET Framework 4.8.1`
- `C# 13.0`

另外这轮还同步引入或更新了若干兼容性和音频相关 NuGet 包，完整版本以 `packages.config` 和 `CrimsonSon.csproj` 为准。

## 构建

本地构建：

```powershell
dotnet build CrimsonSon.sln -c Debug
dotnet build CrimsonSon.sln -c Release
```

上面这两个命令仍然是当前项目的构建入口。

注意：

公开仓库不会提交 `using/` 下的本地游戏引用，所以你直接克隆后不一定能立刻通过编译。需要你自己补齐对应的本地引用 DLL。

## 安装

适用对象：`LabAPI` 服务器。

常用目录：

- 插件目录（全局）：`%AppData%\SCP Secret Laboratory\LabAPI\plugins\global\`
- 插件目录（单端口）：`%AppData%\SCP Secret Laboratory\LabAPI\plugins\{port}\`
- 依赖目录（全局）：`%AppData%\SCP Secret Laboratory\LabAPI\dependencies\global\`
- 依赖目录（单端口）：`%AppData%\SCP Secret Laboratory\LabAPI\dependencies\{port}\`

如果启用了 `hoster_policy.txt`，并且设置了 `gamedir_for_configs: true`，上面的 `%AppData%` 会改成服务器目录下的 `AppData\`。

安装步骤：

1. 到本仓库的 `v1.1.1` release 页面下载 `CrimsonSon.dll` 和随 release 一起附带的依赖 DLL。
2. 把 `CrimsonSon.dll` 放进 `plugins\global` 或 `plugins\{port}`。
3. 把 release 里除 `CrimsonSon.dll` 之外的依赖 DLL 放进 `dependencies\global` 或 `dependencies\{port}`。
4. 不要把依赖 DLL 放进 `plugins` 目录。
5. 如果你的服务器本来已经装了同版本的 `LabApi.dll`、`AudioManagerAPI.dll`、`HintServiceMeow.dll` 等依赖，可以不用重复覆盖；但版本最好和当前 release 保持一致。
6. 首次启动后再去改配置，不要先手搓空白配置文件。

另外，按 AudioManagerAPI 仓库说明，它首次运行后会在服务器侧生成自己的音频配置文件 `Configs/AudioConfig.json`。这个文件属于 AudioManagerAPI，不是 CrimsonSon 自己的玩法配置。

## 配置说明

CrimsonSon 会自动生成：

- `config.yml`
- `translations.yml`

完整配置教程看 [CONFIG_GUIDE.md](CONFIG_GUIDE.md)。

这里就不把配置项逐条重复抄一遍了。简单说：

- 玩法数值、房间、音量、HUD 坐标这些改 `config.yml`
- 文本提示、Cassie 文案、角色名称和描述这些改 `translations.yml`
- 角色默认物品目前还是硬编码，没有整套配置化

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
