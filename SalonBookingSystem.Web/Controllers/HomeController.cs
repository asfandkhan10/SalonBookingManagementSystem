using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Web.Models;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IApiService apiService, ILogger<HomeController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var servicesResponse = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<ServiceViewModel>>>("services?pageSize=100");
            var barbersResponse = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<BarberViewModel>>>("barbers?pageSize=100");

            var viewModel = new HomeViewModel
            {
                Services = servicesResponse?.Data?.Data ?? new List<ServiceViewModel>(),
                Barbers = barbersResponse?.Data?.Data ?? new List<BarberViewModel>()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load home page data");
            return View(new HomeViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
