# CrimsonSon 配置指南

CrimsonSon 使用 LabAPI 的配置系统生成和读取配置文件。首次启动服务器时，插件会自动生成默认文件；建议先让服务器成功加载一次插件，再修改生成出来的配置。

## 配置文件

CrimsonSon 会生成两份 YAML：

- `config.yml`：玩法数值、召唤人数、仪式时长、出生房间、音量、HUD 位置、角色人数上限、权重和血量。
- `translations.yml`：命令提示、召唤提示、仪式提示、CASSIE 文案、倒计时文案、终局击杀文案、角色名称和角色描述。

角色默认物品列表当前不在配置文件中管理。

## 默认路径

CrimsonSon 使用按端口分目录的 LabAPI 配置路径。

默认路径通常是：

```text
%AppData%\SCP Secret Laboratory\LabAPI\configs\{port}\CrimsonSon\config.yml
%AppData%\SCP Secret Laboratory\LabAPI\configs\{port}\CrimsonSon\translations.yml
```

例如服务器端口为 `7777` 时：

```text
%AppData%\SCP Secret Laboratory\LabAPI\configs\7777\CrimsonSon\config.yml
%AppData%\SCP Secret Laboratory\LabAPI\configs\7777\CrimsonSon\translations.yml
```

如果服务器启用了 `hoster_policy.txt`，并设置 `gamedir_for_configs: true`，基准目录会从 `%AppData%` 切换到服务器目录下的 `AppData\`。

## 首次启动

首次安装后推荐流程：

1. 确认 LabAPI 和前置插件已经安装。
2. 将 `CrimsonSon.dll` 放入 LabAPI 插件目录。
3. 启动服务器一次。
4. 等待 `config.yml` 和 `translations.yml` 自动生成。
5. 关闭服务器或换图前，在生成文件中修改需要调整的内容。

不建议手动创建空白配置文件。空文件或格式不完整的 YAML 更容易导致加载失败。

## `config.yml`

常用可调内容包括：

- `MinimumSpectatorsToSummon`：召唤深红之子所需的最少观察者人数。
- `MaximumSummonCount`：单次最多召唤的深红阵营人数。
- `CountDummySpectatorsForSummon`：是否允许旁观状态的 dummy NPC 计入召唤候选。测试服可以开启，正式服建议关闭。
- `RitualDurationSeconds`：献祭仪式倒计时时长，单位为秒。
- `SummonAudioVolume`、`EventAudioVolume`、`CassieAudioVolume`：召唤入场、仪式和 CASSIE 配套音频音量。
- `CassieBroadcastHoldSeconds`：召唤时 CASSIE message 自动发送的英文句号数量。代码会生成 `. . . .` 这种空格分隔格式；实测约 1 个 `.` 延长 1 秒显示时间。这些句号不会追加到中文字幕后面，CASSIE 声音由 `cassie.wav` 播放。
- `SpawnRoom`：深红阵营第二波生成房间。
- `Spawn999BRoom`：`SCP-999-B` 开局生成房间。
- HUD 坐标和字号：角色介绍、仪式倒计时、全局提示的位置与字体大小。
- `Roles`：各深红角色的人数上限、分配权重和初始血量。

`Roles` 的键名使用角色枚举名：

```text
SCP999B
HolyFather
Heathen
Hallow
Follower
EvilFollower
Fanatic
```

`SCP999B` 不参与第二波召唤分配，通常不需要修改它的权重和人数上限。

## `translations.yml`

常用可调内容包括：

- `Success`、`Failed`：命令执行提示。
- `SummonSuccess`、`NotEnoughSpectators`、`SummonRetryUnavailable`：召唤相关提示。
- `RitualStarted`、`RitualInterrupted`：仪式开始和中断提示。
- `CassieBroadcast`：CASSIE 广播文案。
- `KingKillingCD`：仪式倒计时 HUD 文案。
- `KingKillReason`：仪式完成后的击杀原因。
- `RoleTranslations`：各角色名称和描述。

角色描述支持 `{Name}` 占位符，插件加载时会替换成对应角色名称。

`KingKillingCD` 支持：

```text
{Room}
{Time}
```

分别表示仪式房间和剩余秒数。

## 升级配置

配置结构变化时，建议删除旧 `config.yml` 和 `translations.yml` 重新生成，再把需要保留的自定义值迁移回去。

从旧版本升级到 `1.2.0` 时，请按配置结构变化处理：先备份并删除旧版 `config.yml` 和 `translations.yml`，启动服务器让插件重新生成，再把需要保留的自定义值迁移到新文件。旧字段 `CassieAnnouncementWords` 已废弃，不要迁移；新版使用 `CassieBroadcastHoldSeconds` 控制 CASSIE 字幕延时，并新增 `CountDummySpectatorsForSummon` 控制 dummy 旁观者是否计入召唤候选。

推荐升级流程：

1. 备份现有 `config.yml` 和 `translations.yml`。
2. 替换 `CrimsonSon.dll`。
3. 启动服务器一次。
4. 检查日志中是否有 CrimsonSon 配置或翻译加载失败的警告。
5. 如果没有警告，按需补充或调整新字段。
6. 如果出现加载失败，先备份旧文件，再删除出问题的文件让插件重新生成。

## 什么时候删除重生

出现下面情况时，删除对应文件重新生成通常更省事：

- YAML 语法被改坏。
- 数字、布尔、字典等字段类型被写成不兼容格式。
- `Roles` 或 `RoleTranslations` 的键名被改错。
- 插件日志明确提示 `config.yml` 或 `translations.yml` 加载失败。
- 需要恢复当前版本默认配置。

只删出问题的文件即可。玩法数值有问题时删 `config.yml`，显示文本有问题时删 `translations.yml`。

## 前置插件配置

`SCPSL-AudioManagerAPI` 会生成自己的音频配置，例如 `Configs/AudioConfig.json`。该文件属于 AudioManagerAPI，不是 CrimsonSon 的玩法配置。
