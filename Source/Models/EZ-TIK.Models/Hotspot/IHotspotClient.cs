using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tik4net;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.Models
{
    public interface IHotspotClient : IDisposable
    {
        #region Users Section

        #region Public Methods

        /// <summary>
        /// Loads All users async
        /// </summary>
        /// <returns>List of all user in the server</returns>
        Task<IEnumerable<HotspotUser>> LoadAllUsersAsync();

        /// <summary>
        /// Loads filterd list of all hotspot users async 
        /// </summary>
        /// <param name="filters">Formats</param>
        /// <returns>List of filterd users list</returns>
        Task<IEnumerable<HotspotUser>> LoadUsersWithFilterAsync(params ITikCommandParameter[] filters);

        /// <summary>
        /// Removes a user from the server async 
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <returns>True if the user is removed</returns>
        Task<bool> RemoveUserAsync(HotspotUser user);

        /// <summary>
        /// Remove a list of users from the server async
        /// </summary>
        /// <param name="users">The users to remove</param>
        /// <returns>The successfully removed users</returns>
        Task<IEnumerable<HotspotUser>> RemoveUsersAsync(IEnumerable<HotspotUser> users);

        /// <summary>
        /// Adds a user to the server
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>True if the user is added</returns>
        Task<bool> AddUserAsync(HotspotUser user);

        /// <summary>
        /// Add a list of users to the server async
        /// </summary>
        /// <param name="users">The users to add</param>
        /// <returns>The successfully added users</returns>
        Task<IEnumerable<HotspotUser>> AddUsersAsync(IEnumerable<HotspotUser> users);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns></returns>
        Task<bool> UpdateUserAsync(HotspotUser user);

        /// <summary>
        /// Update a list of users
        /// </summary>
        /// <param name="users">The users to update</param>
        /// <returns>The successfully updated users</returns>
        Task<IEnumerable<HotspotUser>> UpdateUsersAsync(IEnumerable<HotspotUser> users);

        #endregion

        #endregion

        #region User Profiles Section

        #region Public Methods

        /// <summary>
        /// Load all user profiles from the server
        /// </summary>
        /// <returns>the user profiles list</returns>
        Task<IEnumerable<HotspotUserProfile>> LoadAllProfilesAsync();

        /// <summary>
        /// Remove a given profile
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        /// <returns>The status of the deletion process</returns>
        Task<bool> RemoveProfileAsync(HotspotUserProfile profile);

        Task<bool> AddProfileAsync(HotspotUserProfile profile);

        #endregion

        #endregion
    }
}
