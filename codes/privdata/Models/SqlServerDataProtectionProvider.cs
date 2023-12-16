using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace privdata.Models;
public class SqlServerDataProtectionProvider : IDataProtectionProvider
{
    private readonly AppDbContext _dbContext;

    public SqlServerDataProtectionProvider(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IDataProtector CreateProtector(string purpose)
    {
        // Implement logic to retrieve or create a new key from the database
        var key = _dbContext.DataProtectionKeys.Where(o => o.FriendlyName == purpose).FirstOrDefault();
        if(key == null)
        {
            key = CreateNewKey(purpose);
        }        
        var protector = DataProtectionProvider.Create(key.FriendlyName ?? purpose)
            .CreateProtector(purpose);
        return protector;
    }

    private DataProtectionKey CreateNewKey(string purpose)
    {
        var key = new DataProtectionKey { FriendlyName = purpose };        
        _dbContext.DataProtectionKeys.Add(key);
        _dbContext.SaveChanges();
        return key;
    }
}