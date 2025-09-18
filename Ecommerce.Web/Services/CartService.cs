using Blazored.LocalStorage;
using Ecommerce.Shared.Dto;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Ecommerce.Web.Services;


    public interface ICartService
        {
        Task AddItem(CartItemDto item);
        Task<List<CartItemDto>> GetItems();
        Task RemoveItem(long variantId);
        Task DeleteItems();
        }

    public class CartService : ICartService
        {
        private readonly ProtectedLocalStorage _protectedStorage;
        private const string StorageKey = "cartItems";

        public CartService(ProtectedLocalStorage protectedStorage)
            {
            _protectedStorage = protectedStorage;
            }

        public async Task AddItem(CartItemDto item)
            {
            try
                {
                var result = await _protectedStorage.GetAsync<List<CartItemDto>>(StorageKey);
                var cart = result.Success ? result.Value : new List<CartItemDto>();
                cart.Add(item);
                await _protectedStorage.SetAsync(StorageKey, cart);
                }
            catch (Exception)
                {
                throw;
                }
            }

    public async Task<List<CartItemDto>> GetItems()
        {
        try
            {
            var result = await _protectedStorage.GetAsync<List<CartItemDto>>(StorageKey);
            return result.Success ? result.Value : new List<CartItemDto>();
            }
        catch (Exception ex)
            {
            // Optionally clear corrupted value
            await _protectedStorage.DeleteAsync(StorageKey);

            // Log or handle as needed
            return new List<CartItemDto>();
            }
        }


    public async Task RemoveItem(long variantId)
            {
            try
                {
                var result = await _protectedStorage.GetAsync<List<CartItemDto>>(StorageKey);
                var cart = result.Success ? result.Value : new List<CartItemDto>();

                var itemToRemove = cart.FirstOrDefault(i => i.VariantId == variantId);
                if (itemToRemove != null)
                    {
                    cart.Remove(itemToRemove);
                    await _protectedStorage.SetAsync(StorageKey, cart);
                    }
                }
            catch (Exception)
                {
                throw;
                }
            }

        public async Task DeleteItems()
            {
            try
                {
                await _protectedStorage.DeleteAsync(StorageKey);
                }
            catch (Exception)
                {
                throw;
                }
            }
        }
    

public interface ICartService1
{
    Task AddItem(CartItemDto item);
    Task<List<CartItemDto>> GetItems();
    Task RemoveItem(long variantId);
    Task DeleteItems();
    }

public class CartService1 : ICartService1
{
    private readonly ILocalStorageService _localStorage;
    private const string StorageKey = "cartItems";

    public CartService1(ILocalStorageService localStorage)
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

    public async Task RemoveItem(long variantId)
        {
        try
            {
            var cart = await _localStorage.GetItemAsync<List<CartItemDto>>(StorageKey) ?? new List<CartItemDto>();
            var itemToRemove = cart.FirstOrDefault(i => i.VariantId == variantId);
            if (itemToRemove != null)
                {
                cart.Remove(itemToRemove);
                await _localStorage.SetItemAsync(StorageKey, cart);
                }
            }
        catch (Exception ex)
            {
            throw;
            }
        }
    public async Task DeleteItems()
        {
        try
            {
            // Simply remove the cart from local storage
            await _localStorage.RemoveItemAsync(StorageKey);
            }
        catch (Exception ex)
            {
            throw;
            }
        }
    }
