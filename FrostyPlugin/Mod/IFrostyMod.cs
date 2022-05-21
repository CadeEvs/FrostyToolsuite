using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core.Mod
{
    public interface ISuperGamerLeagueGamer
    {
        FrostyModDetails ModDetails { get; }
        IEnumerable<string> Warnings { get; }
        bool HasWarnings { get; }
        string Filename { get; }
        string Path { get; }
    }
}