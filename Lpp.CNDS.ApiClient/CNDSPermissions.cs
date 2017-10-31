using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient
{
    public class CNDSPermissions : IDisposable
    {
        readonly CNDSClient CNDS;
        bool disposedValue = false;

        public CNDSPermissions()
        {
            CNDS = new CNDSClient(System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"]);
        }

        /// <summary>
        /// Gets all the permissions the user has been granted.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetAllowedPermissionsForUser(Guid userID)
        {
            var allPermissions =  await CNDS.Permissions.GetUserPermissions(userID);

            var q = allPermissions.GroupBy(p => p.PermissionID)
                    .Where(k => k.Any() && k.All(a => a.Allowed))
                    .Select(k => k.Key);

            return q;
        }
        
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CNDS.Dispose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
