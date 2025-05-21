using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;

namespace AI_WebsiteBuilder.Pages
{
    public class EditMemberModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditMemberModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public EditInputModel Input { get; set; }

        public string Id { get; set; }

        public class EditInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public string Role { get; set; }

            [MinLength(6, ErrorMessage = "The new password must be at least 6 characters long.")]
            public string? NewPassword { get; set; } // Nullable, optional
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("/MembersManagement");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            Input = new EditInputModel
            {
                Email = user.Email ?? string.Empty, // Fix nullable assignment - GitHub AI Recommended from Scan
                Role = roles.FirstOrDefault() ?? string.Empty // Default to empty string if no role - GitHub AI Recommended from Scan
            };

            Id = user.Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Email = Input.Email;
            user.UserName = Input.Email;
            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
            await _userManager.AddToRoleAsync(user, Input.Role);

            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page(); // Stay on page and show errors if reset fails
                }
            }

            TempData["SuccessMessage"] = "Member account updated successfully.";
            return RedirectToPage("/MembersManagement");
        }
    }
}
