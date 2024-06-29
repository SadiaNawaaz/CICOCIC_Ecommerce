using Blazored.LocalStorage;
using Ecommerce.Shared.Dto;

namespace Ecommerce.Web.Services;

public interface ICartService
{
    Task AddItem(CartItemDto item);
    Task<List<CartItemDto>> GetItems();
}

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private const string StorageKey = "cartItems";

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task AddItem(CartItemDto item)
    {
        try
        {
            var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(StorageKey) ?? new List<CartItemDto>();
            cart.Add(item);
            await _localStorage.SetItemAsync(StorageKey, cart);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<CartItemDto>> GetItems()
    {
        return await _localStorage.GetItemAsync<List<CartItemDto>>(StorageKey) ?? new List<CartItemDto>();
    }
}
