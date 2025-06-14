﻿using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Repositories;

public class UserProgramRepository
{
    private readonly List<UserProgram> _programs = [];

    public Task<UserProgram?> GetById(Guid id)
    {
        return Task.FromResult(_programs.FirstOrDefault(x => x.Id == id));
    }
}
