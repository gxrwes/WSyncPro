using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Db;

namespace WSyncPro.Core.Services
{
    public interface IAppLocalDb
    {
        // Saves the AppDb as json to file
        public Task<bool> SaveDb();

        // Loads Db from file
        public Task<bool> LoadDb();

        // Provides a deep copy of the deserialized AppDb
        public Task<AppDb> GetAppDbAsync();
        public AppDb GetAppDb();
        // Gets a current AppDb state, then saves this state and reloads it
        public Task<bool> UpdateDb(AppDb appDb);

        // To get a unique id
        public string GetUUID();
    }
}
