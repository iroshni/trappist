﻿using Promact.Trappist.DomainModel.DbContext;
using System.Linq;
using Promact.Trappist.DomainModel.Models.Test;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Promact.Trappist.Utility.GlobalUtil;
using System;

namespace Promact.Trappist.Repository.Tests
{
    public class TestsRepository : ITestsRepository
    {
        private readonly TrappistDbContext _dbContext;
        private readonly IGlobalUtil _util;
        public TestsRepository(TrappistDbContext dbContext, IGlobalUtil util)
        {
            _dbContext = dbContext;
            _util = util;
        }

        public async Task CreateTestAsync(Test test)
        {
            test.Link = _util.GenerateRandomString(10);
            _dbContext.Test.Add(test);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsTestNameUniqueAsync(string testName, int id)
        {
            var isTestExists = await (_dbContext.Test.AnyAsync(x =>
                                    x.TestName.ToLowerInvariant() == testName.ToLowerInvariant()
                                    && x.Id != id));
            return !isTestExists;
        }

        public async Task<List<Test>> GetAllTestsAsync()
        {
            return await _dbContext.Test.OrderByDescending(x => x.CreatedDateTime).ToListAsync();
        }

        public async Task UpdateTestNameAsync(int id, Test testObject)
        {
            var testSettingsToUpdate = _dbContext.Test.FirstOrDefault(x => x.Id == id);
            testSettingsToUpdate.TestName = testObject.TestName;
            _dbContext.Test.Update(testSettingsToUpdate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTestSettingsAsync(Test testObject)
        {
            _dbContext.Test.Update(testObject);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Test> GetTestSettingsAsync(int id)
        {
            string currentDate = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm");
            var testSettings = await _dbContext.Test.FirstOrDefaultAsync(x => x.Id == id);
            if (testSettings != null)
            {
                testSettings.StartDate = testSettings.StartDate == default(DateTime) ? Convert.ToDateTime(currentDate) : testSettings.StartDate; //If the StartDate field in database contains default value on visiting the Test Settings page of a Test for the first time then that default value gets replaced by current DateTime
                testSettings.EndDate = testSettings.EndDate == default(DateTime) ? Convert.ToDateTime(currentDate) : testSettings.EndDate; //If the EndDate field in database contains default value on visiting the Test Settings page of a Test for the first time then that default value gets replaced by current DateTime
                return testSettings;
            }
            else
                return null;
        }

        public async Task<bool> IsTestExists(int id)
        {
            return await _dbContext.Test.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> IsTestAttendeeExistAsync(int id)
        {
            Test test = await _dbContext.Test.Include(x => x.TestAttendees).FirstOrDefaultAsync(x => x.Id == id);
            return test.TestAttendees.Any();
        }

        public async Task DeleteTestAsync(int id)
        {
            Test test = await _dbContext.Test.FindAsync(id);
            _dbContext.Test.Remove(test);
            await _dbContext.SaveChangesAsync();
        }
    }
}