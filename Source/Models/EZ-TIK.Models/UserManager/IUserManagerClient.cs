using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tik4net;

namespace EZ_TIK.Models
{
    public interface IUserManagerClient : IDisposable
    {
        #region Users Section

        #region Public Methods

        /// <summary>
        /// Loads All users async
        /// </summary>
        /// <returns>List of all user in the server</returns>
        Task<IEnumerable<UserManagerUser>> LoadAllUsersAsync();

        /// <summary>
        /// Loads filterd list of all hotspot users async 
        /// </summary>
        /// <param name="filters">Formats</param>
        /// <returns>List of filterd users list</returns>
        Task<IEnumerable<UserManagerUser>> LoadUsersWithFilterAsync(params ITikCommandParameter[] filters);

        /// <summary>
        /// Removes a user from the server async 
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <returns>True if the user is removed</returns>
        Task<bool> RemoveUserAsync(UserManagerUser user);

        /// <summary>
        /// Remove a list of users from the server async
        /// </summary>
        /// <param name="users">The users to remove</param>
        /// <returns>The successfully removed users</returns>
        Task<IEnumerable<UserManagerUser>> RemoveUsersAsync(IEnumerable<UserManagerUser> users);

        /// <summary>
        /// Adds a user to the server
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>True if the user is added</returns>
        Task<bool> AddUserAsync(UserManagerUser user);

        /// <summary>
        /// Add a list of users to the server async
        /// </summary>
        /// <param name="users">The users to add</param>
        /// <returns>The successfully added users</returns>
        Task<IEnumerable<UserManagerUser>> AddUsersAsync(IEnumerable<UserManagerUser> users);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns></returns>
        Task<bool> UpdateUserAsync(UserManagerUser user);

        /// <summary>
        /// Update a list of users
        /// </summary>
        /// <param name="users">The users to update</param>
        /// <returns>The successfully updated users</returns>
        Task<IEnumerable<UserManagerUser>> UpdateUsersAsync(IEnumerable<UserManagerUser> users);

        /// <summary>
        /// Activate a user with a profile
        /// </summary>
        /// <param name="user">THe user to activate</param>
        /// <param name="profileName">The profile to be activated for the user</param>
        /// <returns></returns>
        Task<bool> ActivateUser(UserManagerUser user, string profileName);

        #endregion

        #endregion

        #region User Profiles Section

        #region Public Methods

        #region Loading section

        /// <summary>
        /// Load all user profiles from the server
        /// </summary>
        /// <returns>the user profiles list</returns>
        Task<IEnumerable<UserManagerProfile>> LoadAllProfilesAsync();

        /// <summary>
        /// Loads all profile limitations from the server
        /// </summary>
        /// <returns>The profiles limitations list</returns>
        Task<IEnumerable<UserManagerProfileLimitation>> LoadAllProfileLimitationsAsync();

        /// <summary>
        /// Loads all records of the profile limitations
        /// </summary>
        /// <returns>The list of the profile limitations</returns>
        Task<IEnumerable<UserManagerProfileProfileLimitation>> LoadAllProfileProfileLimitationsAsync();

        #endregion

        #region Removing section

        /// <summary>
        /// Removes the specified profile from the server
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        /// <returns>The status of the operation</returns>
        Task<bool> RemoveProfileAsync(UserManagerProfile profile);

        #endregion

        #endregion

        #endregion

        #region Customers

        #region Public Methods

        /// <summary>
        /// Loads the user manager customers
        /// </summary>
        /// <returns>List of usermanager customers</returns>
        Task<IEnumerable<UserManagerCustomer>> LoadAllCustomersAsync();

        #endregion

        #endregion
    }
}
