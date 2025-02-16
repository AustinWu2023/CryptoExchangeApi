using CryptoExchangeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchangeApi.Tests {
    public abstract class TestBase : IDisposable {
        protected readonly AppDbContext _dbContext;

        protected TestBase() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                //.UseInMemoryDatabase(Guid.NewGuid().ToString())  // 使用唯一 DB 避免測試衝突
                .Options;

            _dbContext = new AppDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose() {
            _dbContext?.Dispose();
        }
    }
}
