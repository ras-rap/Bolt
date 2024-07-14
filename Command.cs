using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt
{
    public class Command
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Action<byte, string[]> Run;
        public readonly string[] Parameters;
        public readonly int PermissionLevel;

        public Command(string name, Action<byte, string[]> run, string description = "", string[] parameters = null, int permissionLevel = 0)
        {
            Name = name.ToLower();
            Description = description;
            Run = run;
            Parameters = parameters ?? Array.Empty<string>();
            PermissionLevel = permissionLevel;
        }

        public string GetParameters()
            => Parameters.Length > 0 ? string.Join(" ", Parameters.Select(s => $"{{{s}}}")) : "No parameters";
    }
}
