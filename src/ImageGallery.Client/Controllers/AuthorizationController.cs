﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Client.Controllers
{
    public class AuthorizationController : Controller
    {
	    public AuthorizationController()
	    {
		    
	    }

	    public IActionResult AccessDenied()
	    {
		    return View();
	    }
    }
}
