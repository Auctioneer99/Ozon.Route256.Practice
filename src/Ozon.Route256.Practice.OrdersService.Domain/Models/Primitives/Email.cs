using System.Text.RegularExpressions;

namespace Ozon.Route256.Practice.OrdersService.Domain.Models.Primitives;

public sealed class Email
{
    public string Value
    {
        get => _value;
        private set
        {
            if (Regex.IsMatch(value, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") == false)
            {
                throw new ArgumentException("Email");
            }

            _value = value;
        }
    }
    private string _value = null!;
    
    public Email(string mail)
    {
        Value = mail;
    }
    
    public static implicit operator string(Email e) => e.Value;
    
    public static implicit operator Email(string s) => new (s);
}