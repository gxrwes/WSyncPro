using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WSyncPro.Models.Db;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public class AppLocalDb : IAppLocalDb
    {
        private readonly string _dbFilePath;
        private readonly ILogger<AppLocalDb> _logger;
        private AppDb _appDb;

        public AppLocalDb(string dbFilePath, ILogger<AppLocalDb> logger)
        {
            _dbFilePath = dbFilePath;
            _logger = logger;
            _appDb = new AppDb();
        }

        public async Task<bool> SaveDb()
        {
            try
            {
                string json = JsonSerializer.Serialize(_appDb, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_dbFilePath, json);
                _logger.LogInformation("Database saved successfully to {DbFilePath}", _dbFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving database to {DbFilePath}", _dbFilePath);
                return false;
            }
        }

        public async Task<bool> LoadDb()
        {
            try
            {
                if (!File.Exists(_dbFilePath))
                {
                    _logger.LogWarning("Database file not found at {DbFilePath}, initializing new database", _dbFilePath);
                    _appDb = new AppDb();
                    return false;
                }

                string json = await File.ReadAllTextAsync(_dbFilePath);
                _appDb = JsonSerializer.Deserialize<AppDb>(json) ?? new AppDb();
                _logger.LogInformation("Database loaded successfully from {DbFilePath}", _dbFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading database from {DbFilePath}", _dbFilePath);
                return false;
            }
        }

        public Task<AppDb> GetAppDb()
        {
            try
            {
                var appDbCopy = JsonSerializer.Deserialize<AppDb>(JsonSerializer.Serialize(_appDb));
                _logger.LogInformation("Database copy created successfully");
                return Task.FromResult(appDbCopy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database copy");
                throw;
            }
        }

        public async Task<bool> UpdateDb(AppDb appDb)
        {
            try
            {
                _appDb = appDb;
                _logger.LogInformation("Updating database with new state");
                return await SaveDb();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating database");
                return false;
            }
        }
    }
}