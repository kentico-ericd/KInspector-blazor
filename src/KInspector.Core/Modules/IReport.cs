using System;
using System.Collections.Generic;

using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    public interface IReport : IModule
    {
        ReportResults GetResults();
    }
}