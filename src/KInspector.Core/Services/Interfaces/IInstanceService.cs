﻿using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for getting additional instance information.
    /// </summary>
    public interface IInstanceService : IService
    {
        /// <summary>
        /// Gets details about the instance.
        /// </summary>
        Task<InstanceDetails> GetInstanceDetails(Guid instanceGuid);

        /// <summary>
        /// Gets details about the instance.
        /// </summary>
        Task<InstanceDetails> GetInstanceDetails(Instance? instance);
    }
}