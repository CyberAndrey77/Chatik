using System.Collections.Generic;
using Client.Enums;

namespace Client.NetWork
{
    public interface IPackageHelper
    {
        Dictionary<string, EnumKey> Keys { get; set; }
    }
}