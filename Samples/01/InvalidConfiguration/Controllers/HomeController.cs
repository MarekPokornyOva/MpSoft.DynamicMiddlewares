using Microsoft.AspNetCore.Mvc;

namespace InvalidConfiguration.Controllers
{
	public class HomeController:Controller
	{
		public IActionResult Index()
		{
			return new ContentResult { StatusCode=200,ContentType="text/html",Content="<h1>Success</h1>The site is configured fine now. Thank you for your effort." };
		}
	}
}
