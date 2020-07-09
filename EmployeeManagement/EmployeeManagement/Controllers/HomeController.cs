using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Route("Home")]
    //[Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;  // in the videos the deprecated type IHostingEnvironment

        public HomeController(IEmployeeRepository employeeRepository,
                                IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        //[Route("")]
        //[Route("~/")]
        //[Route("/home")]
        //[Route("/home/index")]
        //[Route("index")]
        //[Route("[action]")]
        //[Route("/baba")]  // - this becomes home/baba if Route("Home") is set on class
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployees();
            return View(model);
        }

        //[Route("home/details/{id?}")]
        //[Route("details/{id?}")]
        //[Route("[action]/{id?}")]
        //[Route("{id?}")]
        public ViewResult Details(int? id)
        {
            // used the ?? null cohalescing operator
            //if (id < 1 || id > 3)
            //{
            //    id = 1;
            //}
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id??1),
                PageTitle = "Employee Details vm"
            };
            Employee model = _employeeRepository.GetEmployee(1);
            //ViewData["Employee"] = model;
            //ViewData["PageTitle"] = "Employee Details";
            //ViewBag.Employee = model;
            ViewBag.PageTitle = "Employee Details";
            //return View(model);  // default use by NetCore - Views/*Controller Name w/ Controller*
            return View(homeDetailsViewModel);
        }

        #region custom folder structure and views if you don't want to use the default
        //public ViewResult Details()
        //{
        //    Employee model = _employeeRepository.GetEmployee(1);
        //    return View("Test");
        //}

        //public ViewResult Details()
        //{
        //    Employee model = _employeeRepository.GetEmployee(1);
        //    return View("MyVIews/Test.cshtml");
        //    //return View("/MyVIews/Test.cshtml");
        //    //return View("~/MyVIews/Test.cshtml");
        //}

        //public ViewResult Details()
        //{
        //    Employee model = _employeeRepository.GetEmployee(1);
        //    return View("../Test/Update");
        //}

        //public ViewResult Details()
        //{
        //    Employee model = _employeeRepository.GetEmployee(1);
        //    return View("../../MyViews/Test");
        //}
        #endregion

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //FileStream fs = new FileStream(filePath, FileMode.Create);
                //model.Photo.CopyTo(fs);
                //fs.Close();
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fs);
                }
            }
            Employee newEmployee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                PhotoPath = uniqueFileName
            };
            if (ModelState.IsValid)
            {
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
                        
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

    }
}
