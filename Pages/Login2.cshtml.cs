using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace NanoGo.Pages
{
    public class LoginModel : PageModel
    {
        //public void OnGet()
        //{
        //}

        public IActionResult OnGet()
        {
            loginViewModel = new LoginViewModel();
            loginViewModel.ShowWarning = false;
            return Page();
        }

        [BindProperty]
        public LoginViewModel loginViewModel { get; set; }

        public  IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                loginViewModel.ShowWarning = true;
                return Page();
            }

            if (loginViewModel.UserName == "mcakal" && loginViewModel.Password == "123")
                return RedirectToPage("./Index");
            else
            {
                loginViewModel.ShowWarning = true;
                return Page();
            }
           
        }

        public JsonResult OnGetTest()
        {

            int[] sayilar = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            return new JsonResult(  JsonConvert.SerializeObject(sayilar));
        }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool ShowWarning { get; set; }
    }
}
