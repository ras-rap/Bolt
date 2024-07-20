using _scripts._multiplayer._controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Parameters { get; }
        int PermissionLevel { get; }
        string Run(PlayerInfo playerInfo, string[] args);
    }
}
