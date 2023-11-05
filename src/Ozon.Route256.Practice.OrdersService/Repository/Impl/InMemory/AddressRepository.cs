using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.InMemory;

public sealed class AddressRepository : IAddressRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public AddressRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<AddressDto?> FindById(int id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto?>(token);
        }

        return _inMemoryStorage.Addresses.TryGetValue(id, out var address)
            ? Task.FromResult<AddressDto?>(address)
            : Task.FromResult<AddressDto?>(null);
    }

    public Task<AddressDto?> FindByCoordinates(double latitude, double longitude, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto?>(token);
        }

        var point = 0.001;
        var value = _inMemoryStorage.Addresses.Values
            .FirstOrDefault(a => Math.Abs((double)a.Latitude - latitude) < point && Math.Abs((double)a.Longitude - longitude) < point);

        return Task.FromResult(value);
    }

    public Task<AddressDto[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto[]>(token);
        }

        var addresses = FindDto(ids, token).ToArray();
        return Task.FromResult(addresses);
    }

    public Task<AddressDto[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto[]>(token);
        }

        var addresses = GetDto(ids, token).ToArray();
        return Task.FromResult(addresses);
    }

    public Task<AddressDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Addresses.Values.ToArray());
    }

    public Task<AddressDto> Add(AddressDto dto, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto>(token);
        }

        var id = _inMemoryStorage.AddressIdSequence;

        var toPersist = dto with { Id = id };
        
        _inMemoryStorage.Addresses[toPersist.Id] = toPersist;

        _inMemoryStorage.AddressIdSequence++;
        
        return Task.FromResult(toPersist);
    }

    private IEnumerable<AddressDto> FindDto(IEnumerable<long> ids, CancellationToken token)
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

    private IEnumerable<AddressDto> GetDto(IEnumerable<long> ids, CancellationToken token)
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