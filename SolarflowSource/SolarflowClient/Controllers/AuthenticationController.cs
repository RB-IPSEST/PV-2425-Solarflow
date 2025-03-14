using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SolarflowClient.Models.ViewModels.Authentication;
using SolarflowClient.Models;

namespace SolarflowClient.Controllers;

public class AuthenticationController : Controller
{
    private readonly HttpClient _httpClient;

    public AuthenticationController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7280/api/auth/");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        var errorMessage = await response.Content.ReadAsStringAsync();

        var errors = JsonConvert.DeserializeObject<List<ErrorDetail>>(errorMessage);

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("login", content);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("AuthToken", token);
            return RedirectToAction("Index", "Home");
        }

        var errorMessage = await response.Content.ReadAsStringAsync();

        ModelState.AddModelError(string.Empty, errorMessage);
        return View(model);
    }

    public IActionResult AccountRecovery()
    {
        return View();
    }

    //[HttpPost]
    //public async Task<IActionResult> AccountRecovery(AccountRecoveryViewModel model)
    //{
    //    // TODO: Implement account recovery.
    //}
}
