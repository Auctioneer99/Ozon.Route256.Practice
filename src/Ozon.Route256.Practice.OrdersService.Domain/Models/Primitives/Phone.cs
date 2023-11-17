using System.Text.RegularExpressions;

namespace Ozon.Route256.Practice.OrdersService.Domain.Models.Primitives;

public sealed class Phone
{
    public string Value
    {
        get => _value;
        set
        {
            if (Regex.IsMatch(value, @"^(\+7|8)(\s|-)?(\(\d{3}\)|\d{3})(\s|-)?\d{3}(\s|-)?\d{2}(\s|-)?\d{2}$") == false)
            {
                throw new ArgumentException("Phone");
            }

            _value = value;
        }
    }
    private string _value = null!;

    public Phone(string value)
    {
        Value = value;
    }

    public static implicit operator string(Phone p) => p.Value;

    public static implicit operator Phone(string s) => new(s);
}