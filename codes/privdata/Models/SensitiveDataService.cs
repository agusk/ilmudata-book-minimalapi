using Microsoft.AspNetCore.DataProtection;

namespace privdata.Models;
public class SensitiveDataService
{
    private readonly IDataProtector _protector;

    public SensitiveDataService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("EmployeeDataProtector");
    }

    public Employee EncryptEmployeeData(Employee employee)
    {
        employee.Email = _protector.Protect(employee.Email);
        employee.Phone = _protector.Protect(employee.Phone);
        employee.Birthdate = _protector.Protect(employee.Birthdate);
        return employee;
    }

    public Employee MaskEmployeeData(Employee employee)
    {
        
        employee.Email = MaskEmail(_protector.Unprotect(employee.Email));
        employee.Phone = MaskPhone(_protector.Unprotect(employee.Phone));
        employee.Birthdate = "*****"; // Simple mask for birthdate
        return employee;
    }   
    public static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex == -1 || atIndex == 0) return email; // Invalid or empty email, return as is

        var accountPart = email.Substring(0, atIndex);
        var domainPart = email.Substring(atIndex);

        var maskedLength = accountPart.Length / 2;
        var maskedPart = new string('*', maskedLength);
        var visiblePart = accountPart.Substring(maskedLength);

        return maskedPart + visiblePart + domainPart;
    }

    private string MaskPhone(string phone)
    {
        // Implement phone masking logic
        return "*******" + phone.Substring(phone.Length - 4); // Example
    }
}