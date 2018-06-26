using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using tik4net;
using tik4net.Objects;

namespace EZ_TIK.Models
{
    public class UserManagerClient : IUserManagerClient
    {
        #region Private Members

        private readonly ITikConnection _connection; 

        #endregion

        #region Constructors

        public UserManagerClient(ITikConnection connection)
        {
            _connection = connection;
        }

        #endregion

        #region Users Section

        /// <summary>
        /// Load all users async
        /// </summary>
        /// <returns>The List of users</returns>
        public Task<IEnumerable<UserManagerUser>> LoadAllUsersAsync() => Task.Run(() => _connection.LoadAll<UserManagerUser>());
        
        /// <summary>
        /// Load users with filter
        /// </summary>
        /// <param name="filters">The filter to be used</param>
        /// <returns>The list of users</returns>
        public Task<IEnumerable<UserManagerUser>> LoadUsersWithFilterAsync(params ITikCommandParameter[] filters) => Task.Run(() => _connection.LoadList<UserManagerUser>(filters));

        /// <summary>
        /// Remove user from the user-manager server
        /// </summary>
        /// <param name="user">User to get removed</param>
        /// <returns>The status of the removing proccess</returns>
        public Task<bool> RemoveUserAsync(UserManagerUser user)
        {
            return Task.Run(() =>
            {
                try
                {
                    _connection.Delete(user);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Remove a set of users
        /// </summary>
        /// <param name="users"> A List of the removed users</param>
        /// <returns></returns>
        public Task<IEnumerable<UserManagerUser>> RemoveUsersAsync(IEnumerable<UserManagerUser> users)
        {
            return Task.Run(async () =>
            {
                IEnumerable<UserManagerUser> removed = new List<UserManagerUser>();

                foreach (var user in users)
                    if(await RemoveUserAsync(user)) ((IList)removed).Add(user);

                return removed;
            });
        }

        /// <summary>
        /// Adding user to the user-manager server
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>The status of the proccess</returns>
        public Task<bool> AddUserAsync(UserManagerUser user) => UpdateUserAsync(user);

        public Task<IEnumerable<UserManagerUser>> AddUsersAsync(IEnumerable<UserManagerUser> users)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update user with changed user
        /// </summary>
        /// <param name="user">The user to be updated</param>
        /// <returns>The status of the proccess</returns>
        public Task<bool> UpdateUserAsync(UserManagerUser user) => Task.Run(() =>
        {
            try
            {
                _connection.Save(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        });

        /// <summary>
        /// Update a list of users
        /// </summary>
        /// <param name="users">The list of the users</param>
        /// <returns>The updated users</returns>
        public Task<IEnumerable<UserManagerUser>> UpdateUsersAsync(IEnumerable<UserManagerUser> users) => Task.Run(async () =>
        {
            IEnumerable<UserManagerUser> updated = new List<UserManagerUser>();

            foreach (var user in users)
                if (await UpdateUserAsync(user)) ((IList) updated).Add(user);

            return updated;
        });

        public Task<bool> ActivateUser(UserManagerUser user, string profileName) => Task.Run(() =>
        {
            try
            {
                _connection.CreateCommand("/tool/user-manager/user/create-and-activate-profile", _connection.CreateParameter("user", user.Name), _connection.CreateParameter("customer", user.Customer), _connection.CreateParameter("profile", profileName)).ExecuteAsync(null);
                return true;
            }
            catch
            {
                return false;
            }
        });

        #endregion

        #region Profiles Section

        #region Loading Section

        /// <summary>
        /// Load all user profiles
        /// </summary>
        /// <returns>THe lsit of the profiles</returns>
        public Task<IEnumerable<UserManagerProfile>> LoadAllProfilesAsync() => Task.Run(() => _connection.LoadAll<UserManagerProfile>());


        /// <summary>
        /// Loads all profile limitations
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<UserManagerProfileLimitation>> LoadAllProfileLimitationsAsync() => Task.Run(() => _connection.LoadAll<UserManagerProfileLimitation>());

        /// <summary>
        /// Loads all profile profile limitations
        /// </summary>
        /// <returns></returns>

        public Task<IEnumerable<UserManagerProfileProfileLimitation>> LoadAllProfileProfileLimitationsAsync() => Task.Run(() => _connection.LoadAll<UserManagerProfileProfileLimitation>());

        /// <summary>
        /// Load all usermanager customers
        /// </summary>
        /// <returns>THe list of all usermanager customers</returns>
        public Task<IEnumerable<UserManagerCustomer>> LoadAllCustomersAsync() => Task.Run(() => _connection.LoadAll<UserManagerCustomer>());

        #endregion

        #region Removing section

        /// <summary>
        /// Removes a profile from the server
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        /// <returns>The status of the remving operation</returns>
        public Task<bool> RemoveProfileAsync(UserManagerProfile profile) =>
            Task.Run(() =>
            {
                try
                {
                    _connection.Delete(profile);
                    return true;
                }
                catch
                {
                    return false;
                }
            });


        #endregion

        #endregion

        #region IDisposable Section

        /// <summary>
        /// Close connections and free resources
        /// </summary>
        public void Dispose()
        {
            _connection.Close();
        }

        #endregion
    }
}
