using System.Collections.Generic;
using System.ComponentModel;

namespace CrimsonSon;

public class CSTranslation
{
    [Description("设置角色操作成功信息:")]
    public string Success { get; set; } = "<color=green>操作成功!</color>";

    [Description("设置报错信息:")]
    public string Failed { get; set; } = "<color=red>未定义的参数!</color>";

    [Description("SCP-999-B 召唤成功信息:")]
    public string SummonSuccess { get; set; } = "<color=red><b>SCP-999-B</b></color> 的呼唤已被触发, 深红之子正在集结!";

    [Description("SCP-999-B 召唤失败信息:")]
    public string SummonFailed { get; set; } = "<color=red><b>SCP-999-B</b></color> 的呼唤失败了, 死人不足!";

    [Description("观察者人数不足时的提示。")]
    public string NotEnoughSpectators { get; set; } = "<color=red>当前观察者人数不足，深红之子的呼唤尚未成形。</color>";

    [Description("无法再次尝试召唤时的提示。")]
    public string SummonRetryUnavailable { get; set; } = "<color=red>SCP-999-B 已无法继续维持呼唤，本回合不能再次尝试召唤。</color>";

    [Description("仪式开始时的全服提示。")]
    public string RitualStarted { get; set; } = "<color=red><b>深红献祭仪式已经开始！</b></color>";

    [Description("仪式中断时的全服提示。")]
    public string RitualInterrupted { get; set; } = "<color=red><b>深红献祭仪式被打断了！</b></color>";

    [Description("C.A.S.S.I.E 广播信息:")]
    public string CassieBroadcast { get; set; } = "请注意！站点战斗人员，一支<color=red>未知武装小队</color> 已从 <color=#00FFFF>Gate A</color> 进入设施，所有站点作战单位高度警戒！";

    [Description("深红倒计时提示信息({Room} 表示房间，{Time} 表示剩余时间，单位:秒)：")]
    public string KingKillingCD { get; set; } = "<color=red>深红之王</color>召唤仪式举行中，倒计时：<color=yellow>{Time}</color> 秒。\n定位到 [<color=#00FFFF>{Room}</color>] ！！！";

    [Description("终局击杀提示信息:")]
    public string KingKillReason { get; set; } = "深红之王降临，你已死亡！";

    [Description("角色文本覆盖配置。键名使用 RoleID 枚举名。")]
    public Dictionary<string, CSRoleTranslation> RoleTranslations { get; set; } = new()
    {
        ["SCP999B"] = new CSRoleTranslation
        {
            Name = "SCP-999-B",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你是SCP-999的复制体, 但你是知道你自己是深红之王的儿子, 相比于那个忘本仔, 你更坚信自己是对的。\n所以, 你保护好自己并前往广播室召唤深红之子!"
        },
        ["HolyFather"] = new CSRoleTranslation
        {
            Name = "深红之子 教皇",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>] 举行深红献祭仪式召唤深红之王!"
        },
        ["Heathen"] = new CSRoleTranslation
        {
            Name = "深红之子 异教徒",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。"
        },
        ["Hallow"] = new CSRoleTranslation
        {
            Name = "深红之子 圣徒",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。"
        },
        ["Follower"] = new CSRoleTranslation
        {
            Name = "深红之子 信徒",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。"
        },
        ["EvilFollower"] = new CSRoleTranslation
        {
            Name = "深红之子 恶信徒",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。"
        },
        ["Fanatic"] = new CSRoleTranslation
        {
            Name = "深红之子 狂信徒",
            Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。"
        },
    };

    [Description("csrole 命令提示信息:")]
    public string CSRoleCommandHelp { get; set; } =
    "\n<color=yellow>获取深红阵营角色ID列表：</color>\n" +
    "   <color=green>csrole list</color>\n" +
    "<color=yellow>设置角色格式：</color>\n" +
    "   <color=green>csrole [玩家名称/玩家ID] [角色编号]</color>\n" +
    "<color=yellow>示例：</color>\n" +
    "  - <color=green>csrole list</color> --> <color=grey>获取当前深红阵营角色ID列表</color>\n" +
    "  - <color=green>csrole Crystal 0</color> --> <color=grey>设置</color><color=#00FFFF>[玩家名称为Crystal的玩家]</color><color=grey>当前玩家角色为</color><color=#00FFFF>[ID是0的角色]</color>\n" +
    "  - <color=green>csrole 2 1</color> --> <color=grey>设置</color><color=#00FFFF>[玩家ID为2(02)的玩家]</color><color=grey>为</color><color=#00FFFF>[ID是1的角色]</color>";
}

public class CSRoleTranslation
{
    [Description("角色名称覆盖。")]
    public string Name { get; set; }

    [Description("角色描述覆盖。支持 {Name} 占位符。")]
    public string Description { get; set; }
}
