using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppRRHH.Data;
using AppRRHH.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppRRHH.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        const string DEFAULT_AVATAR = "default-profile.png";

        public EmployeesController(ApplicationDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string NameSearch, string SurnameSearch, string DepartSearch)
        {
            var applicationDbContext = _context.Employee.Include(e => e.Department);

            List<Employee> employees = await applicationDbContext.ToListAsync();

            if (!String.IsNullOrEmpty(NameSearch) && !String.IsNullOrEmpty(SurnameSearch))
            {
                employees = await _context.Employee.Where(c => c.Name.Contains(NameSearch) && c.Surname.Contains(SurnameSearch)).ToListAsync();
            }
            else if (!String.IsNullOrEmpty(NameSearch))
            {
                employees = await _context.Employee.Where(c => c.Name.Contains(NameSearch)).ToListAsync();
            }
            else if (!String.IsNullOrEmpty(SurnameSearch))
            {
                employees = await _context.Employee.Where(c => c.Surname.Contains(SurnameSearch)).ToListAsync();
            }

            if (!String.IsNullOrEmpty(DepartSearch))
            {
                employees = employees.Where(c => c.DepartmentId == Convert.ToInt32(DepartSearch)).ToList();
            }

            ViewData["Department"] = new SelectList(_context.Department, "Id", "Namespace");

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["Image"] = employee.Image;
            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "admin, subadmin")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Namespace");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin, subadmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Seniority,DepartmentId")] Employee employee, IFormFile imageFile, string GrossSalary, string NetSalary)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    // If img folder not exist, create it
                    string path = Path.Combine(_environment.WebRootPath, "img");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // get the file name and upload to img folder
                    string fileName = Path.GetFileName(imageFile.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        imageFile.CopyTo(stream);

                    employee.Image = fileName;
                }
                else
                {
                    // if postedFile is null, save the default image
                    employee.Image = DEFAULT_AVATAR;
                }

                // Process Gross and Net salary
                if (double.TryParse(GrossSalary, out double _grossSalary))
                {
                    employee.GrossSalary = _grossSalary;
                }
                else
                {
                    TempData["msg"] = $"Error, Gross Salary value not admited";
                    return RedirectToAction(nameof(Create));
                }

                if (double.TryParse(NetSalary, out double _netSalary))
                {
                    employee.NetSalary = _netSalary;
                }
                else
                {
                    TempData["msg"] = $"Error, Net Salary value not admited";
                    return RedirectToAction(nameof(Create));
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Namespace", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "admin, subadmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["GrossSalary"] = employee.GrossSalary;
            ViewData["NetSalary"] = employee.NetSalary;
            ViewData["Image"] = employee.Image;
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Namespace", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin, subadmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Seniority,Image,DepartmentId")] Employee employee, IFormFile imageFile, string GrossSalary, string NetSalary)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        // If img folder not exist, create it
                        string path = Path.Combine(_environment.WebRootPath, "img");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        // get the file name and upload to img folder
                        string fileName = Path.GetFileName(imageFile.FileName);
                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                            imageFile.CopyTo(stream);

                        employee.Image = fileName;
                    }

                    if (double.TryParse(GrossSalary, out double _grossSalary))
                    {
                        employee.GrossSalary = _grossSalary;
                    }
                    else
                    {
                        TempData["msg"] = $"Error, Gross Salary value not admited";
                        return RedirectToAction(nameof(Edit));
                    }

                    if (double.TryParse(NetSalary, out double _netSalary))
                    {
                        employee.NetSalary = _netSalary;
                    }
                    else
                    {
                        TempData["msg"] = $"Error, Net Salary value not admited";
                        return RedirectToAction(nameof(Edit));
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Namespace", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "admin, subadmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["Image"] = employee.Image;
            return View(employee);
        }

        // POST: Employees/Delete/5
        [Authorize(Roles = "admin, subadmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
