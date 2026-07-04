# CrimsonSon 配置教程

这个文件只讲怎么管理 CrimsonSon 的配置文件、它们会生成到哪里、什么时候该删掉重生。

不重复解释每个字段本身的注释，因为 `config.yml` 和 `translations.yml` 里已经有默认值和说明了。

## 会生成哪些文件

CrimsonSon 自己会生成两份 YAML：

- `config.yml`
- `translations.yml`

它们分别负责：

- `config.yml`：玩法数值、刷新人数、权重、血量、房间、音量、HUD 位置这些
- `translations.yml`：提示文本、Cassie 文案、角色名称、角色描述这些

## 默认生成路径

你的插件代码调用 `TryLoadConfig(..., isGlobal: false)`，所以这两个文件默认走 **按端口分目录** 的配置路径，不走 `global`。

LabAPI 默认配置目录规则是：

- `config.yml`：`%AppData%\SCP Secret Laboratory\LabAPI\configs\{port}\CrimsonSon\config.yml`
- `translations.yml`：`%AppData%\SCP Secret Laboratory\LabAPI\configs\{port}\CrimsonSon\translations.yml`

举例，如果服务器端口是 `7777`，那通常会是：

- `config.yml`：`%AppData%\SCP Secret Laboratory\LabAPI\configs\7777\CrimsonSon\config.yml`
- `translations.yml`：`%AppData%\SCP Secret Laboratory\LabAPI\configs\7777\CrimsonSon\translations.yml`

如果服务器启用了 `hoster_policy.txt`，并且里面设置了 `gamedir_for_configs: true`，那基准目录会从 `%AppData%` 变成服务器目录下的：

- `AppData\SCP Secret Laboratory\LabAPI\configs\{port}\CrimsonSon\`

## 首次启动时会发生什么

第一次加载 CrimsonSon 时，如果这两个文件不存在，LabAPI 会自动创建默认文件。

也就是说：

- 不需要你手动先建空白 `config.yml`
- 不需要你先复制一份模板出来
- 最稳的做法就是先让插件成功启动一次，再去改生成出来的文件

## 更新插件后要不要删配置文件

大多数时候，不用删。

因为 LabAPI 在成功读到旧配置后，会把它重新保存一次，把新字段补进去，旧字段保留。

所以像下面这种情况，通常直接保留原文件就行：

- 只是插件版本更新了
- 新增了几个配置项
- 默认值改了，但旧结构还能正常反序列化
- 你只是继续沿用原来的自定义文本或数值

## 什么时候建议删掉重生

下面这些情况，删掉对应文件重生通常更省事：

- 你把 YAML 改坏了，导致语法错误
- 你把字段类型改错了，比如数字写成了完全不兼容的格式
- 你把字典键名改乱了，导致角色配置或角色翻译无法正确匹配
- 插件启动日志明确提示 `config.yml` 或 `translations.yml` 加载失败
- 你想完全回到当前版本的默认配置
- 后续如果插件配置结构发生了比较大的重构，旧文件虽然还在，但已经不值得手工修

## 删的时候删哪个

不用一股脑全删。

按问题删就行：

- 只有玩法数值有问题，就删 `config.yml`
- 只有文本有问题，就删 `translations.yml`
- 两边都乱了，再一起删

删之前最好先备份一份，尤其是你已经写了很多自定义文本的时候。

## 推荐的更新流程

比较稳的流程是这样：

1. 先确认 `LabAPI`、`SCPSL-AudioManagerAPI`、`HintServiceMeow` 等前置依赖已经按上游说明安装好。
2. 替换 `CrimsonSon.dll`。
3. 保留现有 `config.yml` / `translations.yml`。
4. 启动服务器一次。
5. 看日志里有没有 CrimsonSon 配置加载失败的警告。
6. 如果没有警告，再打开配置文件补新字段或微调旧值。
7. 如果有警告，先备份旧文件，再删除出问题的那一份，让它自动重生。
8. 用新生成的文件对照旧文件，把你真正需要保留的改动手动合回去。

## 什么时候不要直接覆盖旧文件

如果你服务器已经改过很多自定义文本，不建议拿仓库里的说明直接去“整份覆盖”运行中的配置。

更稳的是：

- 先让新版本自己生成或回写一遍
- 再把你旧服里真正有用的改动一点点合回去

这样比较不容易把新字段弄没，也不容易把旧版本遗留的坏格式继续带着跑。

## 这个项目当前最该关注的两类配置

从现有代码看，最容易影响实际流程的是两块：

- `config.yml` 里的召唤人数、仪式时长、房间、角色权重和上限
- `translations.yml` 里的角色描述、召唤提示、仪式提示、Cassie 文案

如果你更新完插件后感觉“玩法没按预期跑”，先看 `config.yml`。

如果你更新完后只是“显示文字不对、角色介绍没改、广播文本不对”，先看 `translations.yml`。

## 额外说明

`SCPSL-AudioManagerAPI` 自己也会生成配置，但那不是 CrimsonSon 的玩法配置。

按它仓库里的说明，AudioManagerAPI 会生成：

- `Configs/AudioConfig.json`

这个文件主要是音频缓存和默认淡入淡出之类的设置，和你这里的 `config.yml` / `translations.yml` 不是一回事。
