using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Db;

namespace WSyncPro.Test.Mock
{
    public class MockAppLocalDb : IAppLocalDb
    {
        private AppDb _appDb = new AppDb();

        public Task<bool> SaveDb() => Task.FromResult(true);

        public Task<bool> LoadDb() => Task.FromResult(true);

        public Task<AppDb> GetAppDbAsync() => Task.FromResult(_appDb);

        public AppDb GetAppDb() => _appDb;

        public string GetUUID() => Guid.NewGuid().ToString();

        public Task<bool> UpdateDb(AppDb appDb)
        {
            _appDb = appDb;
            return Task.FromResult(true);
        }
    }

}
