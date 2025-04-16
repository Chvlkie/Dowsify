using Dowsify.Main.Data;
using Dowsify.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsify.Main.Methods
{
    public interface IFileMethods
    {
        void Export(List<HiddenItem> items, GameFamily gameFamily, List<string> itemNames);
        List<HiddenItem>? Import(GameFamily targetGameFamily);
    }
}
