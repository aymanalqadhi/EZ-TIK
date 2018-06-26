using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik4net;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.Models
{
    public class HotspotClient : IHotspotClient
    {
        private readonly ITikConnection _connection;

        #region Constructors

        public HotspotClient(ITikConnection connection)
        {
            _connection = connection;
        }

        #endregion

        #region IHotspotClient Members

        #region Hotspot User Section
        public Task<IEnumerable<HotspotUser>> LoadAllUsersAsync() => Task.Run(() => _connection.LoadAll<HotspotUser>());

        public Task<IEnumerable<HotspotUser>> LoadUsersWithFilterAsync(params ITikCommandParameter[] filters) => Task.Run(() => _connection.LoadList<HotspotUser>(filters));

        public Task<bool> RemoveUserAsync(HotspotUser user)
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

        public Task<IEnumerable<HotspotUser>> RemoveUsersAsync(IEnumerable<HotspotUser> users)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> AddUserAsync(HotspotUser user) => UpdateUserAsync(user);

        public Task<IEnumerable<HotspotUser>> AddUsersAsync(IEnumerable<HotspotUser> users) => UpdateUsersAsync(users);

        public Task<bool> UpdateUserAsync(HotspotUser user)
        {
            return Task.Run(() =>
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
        }

        public Task<IEnumerable<HotspotUser>> UpdateUsersAsync(IEnumerable<HotspotUser> users)
        {
            return Task.Run(async () =>
            {
                var success = new List<HotspotUser>();

                foreach (var user in users)
                {
                    if (await AddUserAsync(user)) success.Add(user);
                }

                return (IEnumerable<HotspotUser>)success;
            });
        }

        #endregion

        #region Hotspot UserProfiles section

        /// <summary>
        /// Load all Hotspot user profile from the server
        /// </summary>
        /// <returns>The list of the profiles</returns>
        public Task<IEnumerable<HotspotUserProfile>> LoadAllProfilesAsync() => Task.Run(() => _connection.LoadAll<HotspotUserProfile>());

        /// <summary>
        /// Removes a certain profile
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        /// <returns>The status of the deletion process</returns>
        public Task<bool> RemoveProfileAsync(HotspotUserProfile profile) => Task.Run(() =>
        {
            try { _connection.Delete(profile); return true; }
            catch { return false; }
        });

        /// <summary>
        /// Adds a certain profile
        /// </summary>
        /// <param name="profile">The profile to add</param>
        /// <returns>The status of the adding process</returns>
        public Task<bool> AddProfileAsync(HotspotUserProfile profile) => Task.Run(() =>
        {
            try
            {
                //_connection.Save(profile);
                _connection.CreateCommand("/ip/hotspot/user/profile/add",
                    _connection.CreateParameter("name", profile.Name),
                    _connection.CreateParameter("shared-users", profile.SharedUsers),
                    _connection.CreateParameter("rate-limit", profile.RateLimit)
                ).ExecuteNonQuery();

                return true;
            }
            catch { return false; }
        });

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        #endregion

    }
}
