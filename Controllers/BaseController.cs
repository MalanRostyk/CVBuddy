using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly CVBuddyContext _context;
        public BaseController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm)
        {
            _userManager = u;
            _context = c;
            _signInManager = sm;
            
        }


        //Lägger till i Controller klassens OnActionMetod funktionaliteten att läsa antal olästa meddelanden
        //Så varje gång en action executas så skickas det med en ViewBag med count som kan användas för notisen
        //På så sätt slipper vi ha en GetReadCount viewbag i varje actionmetod i all controllers för att det ska synas i all views
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                if(User == null)
                {
                    _signInManager.SignOutAsync(); //RADERA COOKIES FRÅN SENSAT INLOGGADE ANVÄNDARE EFTER ATT MAN LAGT TILL NY MIGRATIONS
                }
                ViewBag.NotReadCount = _context.Messages
                    .Where(m => m.RecieverId == userId && !m.IsRead)
                    .Count();
            }
            else
            {
                ViewBag.NotReadCount = 0;
            }

            base.OnActionExecuting(context);
        }
    }
}
