﻿using KInspector.Core.Modules;
using KInspector.Core.Repositories.Interfaces;

namespace KInspector.Infrastructure.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly IEnumerable<IAction> actions;

        public ActionRepository(IEnumerable<IAction> actions)
        {
            this.actions = actions;
        }

        public IAction GetAction(string codename)
        {
            var allReports = LoadActions();
            return allReports.FirstOrDefault(x => x.Codename.ToLower() == codename.ToLower());
        }

        public IEnumerable<IAction> GetActions()
        {
            return LoadActions();
        }

        private IEnumerable<IAction> LoadActions()
        {
            return actions;
        }
    }
}