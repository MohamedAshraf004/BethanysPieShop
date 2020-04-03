﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BethanysPieShop.Components
{
    public class SystemStatusPage:ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync("http://www.pluralsight.com");
            if (responseMessage.StatusCode==System.Net.HttpStatusCode.OK)
            {
                return View(true);
            }
            return View(false);
        }
    }
}
