using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C9.Admin.Pages;

public sealed class SignInModel : PageModel
{
    public IActionResult OnGet() => User.Identity?.IsAuthenticated == true ? RedirectToPage("/Index") : Page();

    public async Task<IActionResult> OnPostAsync(string? key)
    {
        if (!HttpContext.RequestServices.GetRequiredService<OperatorKey>().Matches(key))
        {
            ModelState.AddModelError(string.Empty, "กุญแจเข้า Console ไม่ถูกต้อง");
            return Page();
        }
        ClaimsIdentity identity = new([new Claim(ClaimTypes.Name, "Local operator")], CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return RedirectToPage("/Index");
    }
}
