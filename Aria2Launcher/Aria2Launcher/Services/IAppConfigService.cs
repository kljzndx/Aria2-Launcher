﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.Services
{
    public interface IAppConfigService
    {
        string ProgramDir { get; set; }
    }
}