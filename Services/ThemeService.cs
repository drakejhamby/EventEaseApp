using Microsoft.JSInterop;

namespace EventEaseApp.Services
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        event Action? ThemeChanged;
        Task ToggleThemeAsync();
        Task InitializeThemeAsync();
    }

    public class ThemeService : IThemeService
    {
        private bool _isDarkMode = false;
        private bool _isInitialized = false;
        private readonly IJSRuntime _jsRuntime;
        private readonly SemaphoreSlim _initSemaphore = new(1, 1);

        public bool IsDarkMode => _isDarkMode;
        public event Action? ThemeChanged;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeThemeAsync()
        {
            if (_isInitialized) return;

            await _initSemaphore.WaitAsync();
            try
            {
                if (_isInitialized) return;

                // Check if there's a saved preference in localStorage
                var savedTheme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "eventease-theme");

                if (!string.IsNullOrEmpty(savedTheme))
                {
                    _isDarkMode = savedTheme == "dark";
                }
                else
                {
                    // Check system preference with better error handling
                    try
                    {
                        var mediaQuery = await _jsRuntime.InvokeAsync<bool>("eval", "window.matchMedia('(prefers-color-scheme: dark)').matches");
                        _isDarkMode = mediaQuery;
                    }
                    catch
                    {
                        _isDarkMode = false;
                    }
                }

                await ApplyThemeAsync();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Theme initialization error: {ex.Message}");
                _isDarkMode = false;
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        public async Task ToggleThemeAsync()
        {
            if (!_isInitialized)
            {
                await InitializeThemeAsync();
            }

            _isDarkMode = !_isDarkMode;
            await ApplyThemeAsync();
            _ = SaveThemePreferenceAsync(); // Fire and forget
            ThemeChanged?.Invoke();
        }

        private async Task ApplyThemeAsync()
        {
            try
            {
                var theme = _isDarkMode ? "dark" : "light";
                await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Theme apply error: {ex.Message}");
            }
        }

        private async Task SaveThemePreferenceAsync()
        {
            try
            {
                var theme = _isDarkMode ? "dark" : "light";
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "eventease-theme", theme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Theme save error: {ex.Message}");
            }
        }
    }
}