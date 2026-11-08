using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C9.Admin.Pages;

public sealed class IndexModel(ServerOperations operations) : PageModel
{
    public ConsoleStatus Status { get; private set; } = new([], []);

    public void OnGet() => Status = operations.Snapshot();
    public JsonResult OnGetMonitor() => new(operations.Snapshot());

    public async Task<IActionResult> OnPostOperateAsync(string action, string target)
    {
        if (!ModelState.IsValid) return BadRequest();
        try
        {
            IReadOnlyList<OperationRecord> result = await operations.ExecuteAsync(action, target);
            TempData["OperationMessage"] = string.Join(" · ", result.Select(item => $"{item.Service} {item.Message}"));
            TempData["OperationFailed"] = result.Any(item => !item.Success);
        }
        catch (ArgumentException) { return BadRequest(); }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSignOutAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/SignIn");
    }
}
