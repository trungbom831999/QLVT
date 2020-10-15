using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using QLVTFinal.Models;
using ZXing;

namespace QLVTFinal.Controllers
{
    public class MaterialsController : Controller
    {
        private QLVatTuEntities db = new QLVatTuEntities();

        // GET: Materials
        public ActionResult Index()
        {
            // var categories = db.SubCategories.Include(c => c.Category);
            //var materials = db.Materials.Include(m => m.SubCategory);
            List<Material> listMaterial = new List<Material>();
            var materials = from m in db.Materials
                            join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                            join c in db.Categories on s.idCategory equals c.idCategory
                            select new
                            {
                                m.count,
                                m.idMaterial,
                                m.idSubCategory,
                                m.SubCategory,
                                c.nameCategory,
                                m.nameMaterial,
                                m.price
                            };
            //var a = materials.ToList();
            var a = materials.OrderBy(x => x.idMaterial).Skip(0).Take(5).ToList();
            foreach (var item in a)
            {
                Material m = new Material();
                m.idMaterial = item.idMaterial;
                m.idSubCategory = item.idSubCategory;
                m.nameCategory = item.nameCategory;
                m.nameMaterial = item.nameMaterial;
                m.price = item.price;
                m.count = item.count;
                m.SubCategory = item.SubCategory;
                listMaterial.Add(m);
            }
            return View(listMaterial);
        }


        //METHOD
        //chuyển có dấu thành không dấu
        public static string convertToUnSign(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public JsonResult LoadData(string name, int idCategory, int sortPrice, int page, int pageSize)
        {
            List<Material> listMaterial = new List<Material>();
            int totalRow = 0;
            var materials = (from m in db.Materials
                             join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                             join c in db.Categories on s.idCategory equals c.idCategory
                             //where convertToUnSign(m.nameMaterial).Contains(convertToUnSign(name))
                             select new
                             {
                                 m.count,
                                 m.idMaterial,
                                 m.idSubCategory,
                                 m.SubCategory,
                                 c.nameCategory,
                                 m.nameMaterial,
                                 m.price
                             }).OrderByDescending(x => x.idMaterial)
                            .Where(x => x.nameMaterial.Contains(name));

            if (idCategory != 0)
            {
                materials = materials.Where(x => x.SubCategory.idCategory == idCategory);
            }
            if (sortPrice == 1)
            {
                materials = materials.OrderBy(x => x.price);
            }
            else if (sortPrice == 2)
            {
                materials = materials.OrderByDescending(x => x.price);
            }
            totalRow = materials.Count();
            materials = materials.Skip((page - 1) * pageSize)
                            .Take(pageSize);
            foreach (var item in materials)
            {
                Material m = new Material();
                m.idMaterial = item.idMaterial;
                m.idCategory = item.SubCategory.idCategory;
                if (item.idSubCategory == null) m.idSubCategory = 0;
                else m.idSubCategory = item.idSubCategory;
                if (item.nameCategory == null) m.nameCategory = "Trống";
                else m.nameCategory = item.nameCategory;
                if (item.SubCategory.nameSubCategory == null) m.nameSubCategory = "Trống";
                else m.nameSubCategory = item.SubCategory.nameSubCategory;
                m.nameMaterial = item.nameMaterial;
                if (item.price == null) m.price = 0;
                else m.price = item.price;
                if (item.count == null) m.count = 0;
                else m.count = item.count;
                m.SubCategory = null;
                listMaterial.Add(m);
            }
            /*if (idCategory == 0)
            {
                totalRow = db.Materials.Where(m => m.nameMaterial.Contains(name)).Count();
                var materials = (from m in db.Materials
                                 join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                                 join c in db.Categories on s.idCategory equals c.idCategory
                                 //where m.nameMaterial.Contains(name)
                                 select new
                                 {
                                     m.count,
                                     m.idMaterial,
                                     m.idSubCategory,
                                     m.SubCategory,
                                     c.nameCategory,
                                     m.nameMaterial,
                                     m.price
                                 }).OrderByDescending(x => x.idMaterial)
                            .Where(x => x.nameMaterial.Contains(name))
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                foreach (var item in materials)
                {
                    Material m = new Material();
                    m.idMaterial = item.idMaterial;
                    m.idCategory = item.SubCategory.idCategory;
                    if (item.idSubCategory == null) m.idSubCategory = 0;
                    else m.idSubCategory = item.idSubCategory;
                    if (item.nameCategory == null) m.nameCategory = "Trống";
                    else m.nameCategory = item.nameCategory;
                    if (item.SubCategory.nameSubCategory == null) m.nameSubCategory = "Trống";
                    else m.nameSubCategory = item.SubCategory.nameSubCategory;
                    m.nameMaterial = item.nameMaterial;
                    if (item.price == null) m.price = 0;
                    else m.price = item.price;
                    if (item.count == null) m.count = 0;
                    else m.count = item.count;
                    m.SubCategory = null;
                    listMaterial.Add(m);
                }
            }
            else
            {
                totalRow = db.Materials.Where(m => m.nameMaterial.Contains(name) && m.SubCategory.idCategory == idCategory).Count();
                var materials = (from m in db.Materials
                                 join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                                 join c in db.Categories on s.idCategory equals c.idCategory
                                 select new
                                 {
                                     m.count,
                                     m.idMaterial,
                                     m.idSubCategory,
                                     m.SubCategory,
                                     c.nameCategory,
                                     m.nameMaterial,
                                     m.price
                                 }).OrderByDescending(x => x.idMaterial)
                            .Where(x => x.nameMaterial.Contains(name) && x.SubCategory.idCategory == idCategory)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                foreach (var item in materials)
                {
                    Material m = new Material();
                    m.idMaterial = item.idMaterial;
                    m.idCategory = item.SubCategory.idCategory;
                    if (item.idSubCategory == null) m.idSubCategory = 0;
                    else m.idSubCategory = item.idSubCategory;
                    if (item.nameCategory == null) m.nameCategory = "Trống";
                    else m.nameCategory = item.nameCategory;
                    if (item.SubCategory.nameSubCategory == null) m.nameSubCategory = "Trống";
                    else m.nameSubCategory = item.SubCategory.nameSubCategory;
                    m.nameMaterial = item.nameMaterial;
                    if (item.price == null) m.price = 0;
                    else m.price = item.price;
                    if (item.count == null) m.count = 0;
                    else m.count = item.count;
                    m.SubCategory = null;
                    listMaterial.Add(m);
                }
            }*/

            /*IQueryable<Material> model = db.Materials;
            if (!string.IsNullOrEmpty(name))
            {
                model = model.Where(x => x.nameMaterial.Contains(name));
            }

            if(idCategory != 0)
            {
                model = model.Where(x => x.SubCategory.idCategory == idCategory);
            }

            int totalRow = model.Count();

            model = model.OrderByDescending(x => x.idMaterial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            List<Material> listMaterial = model.ToList();*/

            return Json(new
            {
                data = listMaterial,
                total = totalRow
            }, JsonRequestBehavior.AllowGet);
        }

        public List<Material> LoadDataForExcel(string name, int idCategory, int sortPrice)
        {
            List<Material> listMaterial = new List<Material>();
            var materials = (from m in db.Materials
                             join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                             join c in db.Categories on s.idCategory equals c.idCategory
                             //where convertToUnSign(m.nameMaterial).Contains(convertToUnSign(name))
                             select new
                             {
                                 m.count,
                                 m.idMaterial,
                                 m.idSubCategory,
                                 m.SubCategory,
                                 c.nameCategory,
                                 m.nameMaterial,
                                 m.price
                             })
                            .Where(x => x.nameMaterial.Contains(name));

            if (idCategory != 0)
            {
                materials = materials.Where(x => x.SubCategory.idCategory == idCategory);
            }
            if (sortPrice == 1)
            {
                materials = materials.OrderBy(x => x.price);
            }
            else if (sortPrice == 2)
            {
                materials = materials.OrderByDescending(x => x.price);
            }
            else
            {
                materials = materials.OrderByDescending(x => x.idMaterial);
            }
            foreach (var item in materials)
            {
                Material m = new Material();
                m.idMaterial = item.idMaterial;
                m.idCategory = item.SubCategory.idCategory;
                if (item.idSubCategory == null) m.idSubCategory = 0;
                else m.idSubCategory = item.idSubCategory;
                if (item.nameCategory == null) m.nameCategory = "Trống";
                else m.nameCategory = item.nameCategory;
                if (item.SubCategory.nameSubCategory == null) m.nameSubCategory = "Trống";
                else m.nameSubCategory = item.SubCategory.nameSubCategory;
                m.nameMaterial = item.nameMaterial;
                if (item.price == null) m.price = 0;
                else m.price = item.price;
                if (item.count == null) m.count = 0;
                else m.count = item.count;
                m.SubCategory = null;
                listMaterial.Add(m);
            }

            return listMaterial;
        }

        public List<SubCategory> GetSubCategoriesByIdCategory(int? id)
        {
            List<SubCategory> listSub = new List<SubCategory>();
            listSub = (db.SubCategories.Include(c => c.Category).Where(c => c.idCategory == id)).ToList();
            //var a = listSub.Where(c => c.Materials.Contains("aa")).ToList();
            return listSub;
        }

        public Material GetFirstMaterial(int? id)
        {
            string nameAdmin = "Chưa phân quyền";
            var mater = (from m in db.Materials
                         join s in db.SubCategories on m.idSubCategory equals s.idSubCategory
                         join c in db.Categories on s.idCategory equals c.idCategory
                         //join a in db.tblAdmins on m.idAdmin equals a.Admin_ID
                         where m.idMaterial == id
                         select new
                         {
                             m.count,
                             m.idMaterial,
                             c.idCategory,
                             m.idSubCategory,
                             m.SubCategory,
                             c.nameCategory,
                             s.nameSubCategory,
                             m.nameMaterial,
                             m.price,
                             m.idAdmin,
                             m.qrcode,
                         }).First();
            if (mater.idAdmin != null)
            {
                var materExtra = (from m in db.Materials
                                  join a in db.tblAdmins on m.idAdmin equals a.Admin_ID
                                  where m.idMaterial == id
                                  select new
                                  {
                                      a.UserName
                                  }).First();
                nameAdmin = materExtra.UserName;
            }
            Material material = new Material();
            material.idMaterial = mater.idMaterial;
            material.idCategory = mater.idCategory;
            material.idSubCategory = mater.idSubCategory;
            material.nameMaterial = mater.nameMaterial;
            material.price = mater.price;
            material.count = mater.count;
            material.nameCategory = mater.nameCategory;
            material.nameSubCategory = mater.nameSubCategory;
            material.SubCategory = mater.SubCategory;
            material.idAdmin = mater.idAdmin;
            material.nameAdmin = nameAdmin;
            material.qrcode = mater.qrcode;

            return material;
        }

        public JsonResult LoadCategory()
        {
            List<Category> listCategory = new List<Category>();
            var categories = db.Categories.Where(c => c.actived == 1).ToList();
            foreach (var item in categories)
            {
                Category cater = new Category();
                cater.idCategory = item.idCategory;
                cater.nameCategory = item.nameCategory;
                cater.actived = 1;
                cater.SubCategories = null;
                listCategory.Add(cater);
            }
            return Json(listCategory, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadSubCategory(int? id)
        {
            List<SubCategory> listSub = new List<SubCategory>();
            var l = (from s in db.SubCategories
                     join c in db.Categories on s.idCategory equals c.idCategory
                     where c.idCategory == id && s.actived == 1
                     select s).ToList();
            foreach (var item in l)
            {
                SubCategory sub = new SubCategory();
                sub.idCategory = item.idCategory;
                sub.idSubCategory = item.idSubCategory;
                sub.nameCategory = item.Category.nameCategory;
                sub.nameSubCategory = item.nameSubCategory;
                sub.actived = 1;
                listSub.Add(sub);
            }
            //var listSub = (db.SubCategories.Include(c => c.Category).Where(c => c.idCategory == 2)).ToList();
            return Json(listSub, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadListAdmin()
        {
            List<tblAdmin> listAdmin = new List<tblAdmin>();
            var admins = db.tblAdmins.ToList();
            foreach (var item in admins)
            {
                tblAdmin admin = new tblAdmin();
                admin.Admin_ID = item.Admin_ID;
                admin.UserName = item.UserName;
                listAdmin.Add(admin);
            }
            return Json(listAdmin, JsonRequestBehavior.AllowGet);
        }

        //Excel
        public ActionResult Export(string name, int idCategory, int sortPrice)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("QLVT");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Tên vật tư";
                worksheet.Cell(currentRow, 2).Value = "Đơn giá";
                worksheet.Cell(currentRow, 3).Value = "Số lượng tồn";
                worksheet.Cell(currentRow, 4).Value = "Danh mục cha";
                worksheet.Cell(currentRow, 5).Value = "Danh mục con";

                List<Material> list = LoadDataForExcel(name, idCategory, sortPrice);
                foreach (var item in list)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.nameMaterial;
                    worksheet.Cell(currentRow, 2).Value = item.price;
                    worksheet.Cell(currentRow, 3).Value = item.count;
                    worksheet.Cell(currentRow, 4).Value = item.nameCategory;
                    worksheet.Cell(currentRow, 5).Value = item.nameSubCategory;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "QLVT.xlsx");
                }
            }
        }

        //public ActionResult Generate(QRCodeModel qrcode)
        //{
        //    try
        //    {
        //        qrcode.QRCodeImagePath = GenerateQRCode(qrcode.QRCodeText);
        //        ViewBag.Message = "QR Code Created successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        //catch exception if there is any
        //    }
        //    return View("Index", qrcode);
        //}


        //QR Code
        private string GenerateQRCode(string qrcodeText) // qrcodeText is id of material
        {
            string folderPath = "~/QRCode/";
            string imagePath = String.Format("~/QRCode/{0}.jpg", qrcodeText);
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        public JsonResult LoadQRCode(int? id)
        {
            var mater = (from m in db.Materials
                            where m.idMaterial == id
                            select new
                            {
                                m.qrcode
                            }).First();
            if (mater.qrcode != null)
            {
                Material material = GetFirstMaterial(id);
                material.SubCategory = null;
                return Json(material, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string qrcodeText = id.ToString();
                string folderPath = "/QRCode/";
                string imagePath = String.Format("/QRCode/{0}.jpg", qrcodeText);

                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }

                var barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;
                var result = barcodeWriter.Write(qrcodeText);

                string barcodePath = Server.MapPath(imagePath);
                var barcodeBitmap = new Bitmap(result);
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                Material materUpdate = (from m in db.Materials
                                        where m.idMaterial == id
                                        select m).First();
                materUpdate.qrcode = imagePath;
                db.Entry(materUpdate).State = EntityState.Modified;
                db.SaveChanges();

                Material material = GetFirstMaterial(id);
                material.SubCategory = null;

                return Json(material, JsonRequestBehavior.AllowGet);
            }
        }

        //END Method

        // GET: Materials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Material material = db.Materials.Find(id);
            Material material = GetFirstMaterial(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        public JsonResult LoadDetails(int? id)
        {
            Material material = GetFirstMaterial(id);
            material.SubCategory = null;
            return Json(material, JsonRequestBehavior.AllowGet);
        }

        // GET: Materials/Create
        /*public ActionResult Create()
        {
            ViewBag.idSubCategory = new SelectList(db.SubCategories, "idSubCategory", "nameSubCategory");
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "nameCategory");
            return View();
        }

        // POST: Materials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMaterial,nameMaterial,price,count,idSubCategory")] Material material)
        {
            if (ModelState.IsValid)
            {
                db.Materials.Add(material);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idSubCategory = new SelectList(db.SubCategories, "idSubCategory", "nameSubCategory", material.idSubCategory);
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "nameCategory", material.idCategory);
            return View(material);
        }*/

        public JsonResult Create(string strMaterial)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Material material = serializer.Deserialize<Material>(strMaterial);
            if (material.idAdmin == 0)
            {
                material.idAdmin = null;
            }
            material.SubCategory = null;
            material.idCategory = null;
            material.nameCategory = null;
            material.nameSubCategory = null;
            //material.qrcode = GenerateQRCode(material.idMaterial.ToString());
            bool status = true;
            string message = "OK";

            db.Materials.Add(material);
            try
            {
                db.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message;
            }

            return Json(new
            {
                status = status,
                message = message
            });
        }

        // GET: Materials/Edit/5
        /*public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Material material = db.Materials.Find(id);
            Material material = GetFirstMaterial(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            List<SubCategory> l = GetSubCategoriesByIdCategory(material.idCategory);
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "nameCategory", material.idCategory);
            ViewBag.idSubCategory = new SelectList(l, "idSubCategory", "nameSubCategory", material.idSubCategory);
            //ViewBag.idSubCategory = GetSubCategorySelectlist(mater.idCategory);
            return View(material);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMaterial,nameMaterial,price,count,idSubCategory,idCategory")] Material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.idSubCategory = GetSubCategorySelectlist(material.idCategory);
            ViewBag.idCategory = new SelectList(db.Categories, "idSubCategory", "nameSubCategory", material.idSubCategory);
            ViewBag.idSubCategory = new SelectList(db.SubCategories, "idSubCategory", "nameSubCategory", material.idSubCategory);
            return View(material);
        }*/

        public JsonResult Edit(string strMaterial)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Material material = serializer.Deserialize<Material>(strMaterial);
            if (material.idAdmin == 0)
            {
                material.idAdmin = null;
            }
            material.SubCategory = null;
            material.idCategory = null;
            material.nameCategory = null;
            material.nameSubCategory = null;
            bool status = true;
            string message = "OK";

            db.Entry(material).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message;
            }


            return Json(new
            {
                status = status,
                message = message
            });
        }

        // GET: Materials/Delete/5
        /*public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Material material = db.Materials.Find(id);
            Material material = GetFirstMaterial(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        public JsonResult Delete(int id)
        {
            bool status = true;
            string message = "OK";

            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
            try
            {
                db.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message;
            }


            return Json(new
            {
                status = status,
                message = message
            });
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
