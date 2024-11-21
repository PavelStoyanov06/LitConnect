using LitConnect.Data.Models;
using LitConnect.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using static LitConnect.Common.ValidationConstants;
using System.ComponentModel.DataAnnotations;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public ProfileViewModel Input { get; set; } = null!;

    [Required(ErrorMessage = "Password is required to save changes")]
    [BindProperty]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        Input = new ProfileViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BookClubsCount = user.BookClubs?.Count ?? 0
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Verify password
        if (!await _userManager.CheckPasswordAsync(user, CurrentPassword))
        {
            ModelState.AddModelError(string.Empty, "Incorrect password.");
            return Page();
        }

        // Update user data
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            StatusMessage = "Profile updated successfully.";
            return RedirectToPage();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}