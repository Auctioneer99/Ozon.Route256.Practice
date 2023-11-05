using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory.Dal;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory;

public sealed class AddressRepository : IAddressRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public AddressRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<Address?> FindById(int id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address?>(token);
        }

        return _inMemoryStorage.Addresses.TryGetValue(id, out var address)
            ? Task.FromResult<Address?>(address)
            : Task.FromResult<Address?>(null);
    }

    public Task<Address?> FindByCoordinates(double latitude, double longitude, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address?>(token);
        }

        var point = 0.001;
        var value = _inMemoryStorage.Addresses.Values
            .FirstOrDefault(a => Math.Abs((double)a.Latitude - latitude) < point && Math.Abs((double)a.Longitude - longitude) < point);

        return Task.FromResult(value);
    }

    public Task<Address[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address[]>(token);
        }

        var addresses = FindDto(ids, token).ToArray();
        return Task.FromResult(addresses);
    }

    public Task<Address[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address[]>(token);
        }

        var addresses = GetDto(ids, token).ToArray();
        return Task.FromResult(addresses);
    }

    public Task<Address[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Addresses.Values.ToArray());
    }

    public Task<Address> Add(Address address, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Address>(token);
        }
        
        _inMemoryStorage.Addresses[address.Id] = address;

        _inMemoryStorage.AddressIdSequence++;
        
        return Task.FromResult(address);
    }

    private IEnumerable<Address> FindDto(IEnumerable<long> ids, CancellationToken token)
    {
        foreach (var id in ids)
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Addresses.TryGetValue(id, out var address))
            {
                yield return address;
            }
        }
    }

    private IEnumerable<Address> GetDto(IEnumerable<long> ids, CancellationToken token)
    {
        foreach (var id in ids)
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Addresses.TryGetValue(id, out var address))
            {
                yield return address;
            }
            else
            {
                throw new NotExistsException($"Адрес с ID {id} не существует");
            }
        }
    }
}