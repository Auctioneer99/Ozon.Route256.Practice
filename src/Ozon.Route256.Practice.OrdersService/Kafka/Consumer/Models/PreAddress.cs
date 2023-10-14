namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;

public sealed record PreAddress(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);