﻿using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public class UndefinedUserProgramDatabaseEntry : UserProgramDatabaseEntry
{
    public long Size { get; }

    public UndefinedUserProgramDatabaseEntry(uint address, RuntimeDatabaseType type, long programSize) : base(address, type)
    {
        Size = programSize;
    }
}
