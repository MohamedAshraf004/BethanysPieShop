using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.Repositories;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BethanysPieShop.Controllers
{
    public class PieManagementController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;

        public PieManagementController(IPieRepository pieRepository,ICategoryRepository categoryRepository)
        {
            this._pieRepository = pieRepository;
            this._categoryRepository = categoryRepository;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var pies = _pieRepository.AllPies.OrderBy(p=>p.Name);
            return View(pies);
        }

        public IActionResult AddPie()
        {
            var categories = _categoryRepository.AllCategories;
            PieEditViewModel pieEditViewModel = new PieEditViewModel
            {
                Categories = categories.Select(c => new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList(),
                CategoryId = categories.FirstOrDefault().CategoryId
            };
            return View(pieEditViewModel);
        }
        [HttpPost]
        public IActionResult AddPie(PieEditViewModel pieEditViewModel)
        {
            if (ModelState.GetValidationState("Pie.Price")== ModelValidationState.Valid &&pieEditViewModel.Pie.Price>0)
            {
                ModelState.AddModelError(nameof(pieEditViewModel.Pie.Price), "invalid price");
            }
            if (ModelState.IsValid)
            {
                _pieRepository.CreatePie(pieEditViewModel.Pie);
                return RedirectToAction("Index");
            }
            return View(pieEditViewModel);
        }

        public IActionResult EditPie(int pieId)
        {
            var categories = _categoryRepository.AllCategories;

            var pie = _pieRepository.AllPies.FirstOrDefault(p => p.PieId == pieId);

            var pieEditViewModel = new PieEditViewModel
            {
                Categories = categories.Select(c => new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList(),
                Pie = pie,
                CategoryId = pie.CategoryId
            };
            var item = pieEditViewModel.Categories.FirstOrDefault(c => c.Value == pie.Category.CategoryId.ToString());
            item.Selected = true;
            return View(pieEditViewModel);
        }
        [HttpPost]
        public IActionResult EditPie(PieEditViewModel pieEditViewModel)
        {
            pieEditViewModel.Pie.CategoryId = pieEditViewModel.CategoryId;
            if (ModelState.GetValidationState("Pie.Price") == ModelValidationState.Valid && pieEditViewModel.Pie.Price < 0)
            {
                ModelState.AddModelError(nameof(pieEditViewModel.Pie.Price), "invalid price");
            }
            if (ModelState.IsValid)
            {
                _pieRepository.UpdatePie(pieEditViewModel.Pie);
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.AllCategories;

             pieEditViewModel = new PieEditViewModel
            {
                Categories = categories.Select(c => new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList(),
                
                CategoryId = pieEditViewModel.Pie.CategoryId
            };
            var item = pieEditViewModel.Categories.FirstOrDefault(c => c.Value == pieEditViewModel.CategoryId.ToString());
            item.Selected = true;

            return View(pieEditViewModel);
        }

        [HttpPost]
        public IActionResult DeletePie(string pieId)
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public JsonResult CheckPieNameIfAlreadyExists([Bind(Prefix = "Pie.Name")] string name)
        {
            var pie = _pieRepository.AllPies.Where(p => p.Name == name).FirstOrDefault();
            if (pie == null)
                return Json(true);
            return Json("Pie name is already taken.");
          //  return pie == null ? Json(true) : Json("Pie name is already taken.");

        }



    }
}
