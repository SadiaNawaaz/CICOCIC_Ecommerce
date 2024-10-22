using Microsoft.AspNetCore.Components;

namespace Ecommerce.Web.Services;

public class CartStateService
    {
    public event Action OnChange;
    private int _cartItemCount;
    private readonly ICartService _cartService;
    public CartStateService(ICartService cartService)
        {
        _cartService = cartService;
        }
    public int CartItemCount
        {
        get => _cartItemCount;
        set
            {
            _cartItemCount = value;
            NotifyStateChanged();
            }
        }
    public async Task InitializeCartCountAsync()
        {
        // Load the items from local storage and update the count
        var cartItems = await _cartService.GetItems();
        CartItemCount = cartItems.Count;  // Initialize the cart count
        }
    private void NotifyStateChanged()
        {
        try
            {
            OnChange?.Invoke();  // Trigger state change in the components
            }
        catch (Exception ex)
            {
            }
        }
    }



