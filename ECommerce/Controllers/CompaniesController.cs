﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECommerce.Classes;
using ECommerce.Models;

namespace ECommerce.Controllers
{
    public class CompaniesController : Controller
    {
        private ECommerceContext db = new ECommerceContext();

        //UPLOAD DE IMAGENS


        //CONTROLE DE LIST VIEW EM CASCATA
        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(c => c.DepartmentsId == departmentId);
            return Json(cities);
        }

        // GET: Companies
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.Cities).Include(c => c.Departments);
            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(ComboHelper.GetCities(), "CityId", "Name");
            ViewBag.DepartmentsId = new SelectList(ComboHelper.GetDepartments(), "DepartmentsId", "Name");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
              
                
                db.Companies.Add(company);
                db.SaveChanges();

                if (company.LogoFile != null)
                {
                    var pic = string.Empty;
                    var folder = "~/Content/Logos";
                    var response = FilesHelper.UploadPhoto(company.LogoFile, folder, string.Format("{0}.jpg", company.CompanyId));

                    if (response)
                    {
                        pic = string.Format("{0}/{1}.jpg", folder, company.CompanyId);
                        company.Logo = pic;
                    }
                    
                }
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(), "CityId", "Name", company.CityId);
            ViewBag.DepartmentsId = new SelectList(ComboHelper.GetDepartments(), "DepartmentsId", "Name", company.DepartmentsId);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(ComboHelper.GetCities(), "CityId", "Name", company.CityId);
            ViewBag.DepartmentsId = new SelectList(ComboHelper.GetDepartments(), "DepartmentsId", "Name", company.DepartmentsId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Logos";

                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                db.Companies.Add(company);

                if (company.LogoFile != null)
                {
                    var response = FilesHelper.UploadPhoto(company.LogoFile, folder, string.Format("{0}.jpg", company.CompanyId));

                    if (response)
                    {
                        pic = string.Format("{0}/{1}.jpg", folder, company.CompanyId);
                        company.Logo = pic;
                        
                    }

                }
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(ComboHelper.GetCities(), "CityId", "Name", company.CityId);
            ViewBag.DepartmentsId = new SelectList(ComboHelper.GetDepartments(), "DepartmentsId", "Name", company.DepartmentsId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
