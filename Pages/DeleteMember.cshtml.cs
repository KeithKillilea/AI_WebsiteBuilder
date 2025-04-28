using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AI_WebsiteBuilder.Pages
{
    public class DeleteMemberModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteMemberModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("/MembersManagement");

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);

                TempData["SuccessMessage"] = "User deleted successfully."; // success message
            }

            return RedirectToPage("/MembersManagement");
        }
    }
}
