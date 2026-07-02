//CommandHandler.cs
using CommandSystem;
using LabApi.Features.Wrappers;
using System;
using System.Linq;
using static CrimsonSon.CrimsonSon;

namespace CrimsonSon.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CSRoleSet : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if(arguments.Count == 2)
        {
            if(!int.TryParse(arguments.ElementAt(1), out int id))
            {
                response = Instance._trans.Failed; // 或者更具体的错误信息
                return false;
            }
            RoleID ID = (RoleID)id;
            if (!Enum.IsDefined(typeof(RoleID), ID)) // 再验证
            {
                response = Instance._trans.Failed; // 或者更具体的错误信息
                return false;
            }
            Player p = null;
            string first = arguments.ElementAt(0);
            if (int.TryParse(first, out int pid))
            {
                p = Player.Get(pid);                     // 按 Player ID  
            }
            else
            {
                p = Player.GetByDisplayName(first)        // 按显示名前缀  
                    ?? Player.GetByNickname(first);       // 按昵称前缀  
            }
            Instance.SetCustomRoles(ID, p); // 设置角色
            response = Instance._trans.Success; // 根据汉化生成成功信息
            return true;
        }
        else if((arguments.Count == 1) && (arguments.ElementAt(0) == "list" || arguments.ElementAt(0) == "LIST"))
        {
            response = Instance.RoleList; //根据汉化生成角色列表字符串
            return true;
        }
        else
        {
            response = Instance._trans.CSRoleCommandHelp; // 根据汉化生成帮助
            return false;
        }
    }

    public string Command { get; } = "csrole";
    public string[] Aliases { get; } = { "CrimsonSonRole", "CrimsonRole" };
    public string Description { get; } = "强制设置深红阵营角色";
}