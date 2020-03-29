using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers
{
    public class EquiposController : Controller
    {
        private FutGolEntities db = new FutGolEntities();

        // GET: Equipos
        public ActionResult Index()
        {
            return View(db.Equipos.ToList());
        }

        // GET: Equipos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipos equipos = db.Equipos.Find(id);
            if (equipos == null)
            {
                return HttpNotFound();
            }
            return View(equipos);
        }

        // GET: Equipos/Create
        public ActionResult Create()
        {
            if (TempData["mensajeError"] != null)
            {
                ViewBag.mensajeError = TempData["mensajeError"].ToString();
                if (TempData["tempEquipo"] != null) ViewBag.tempEquipo = TempData["tempEquipo"].ToString();
                if (TempData["tempPais"] != null) ViewBag.tempPais = TempData["tempPais"].ToString();
            }

            return View();
        }

        // POST: Equipos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombre,pais")] Equipos equipos)
        {
            if (ModelState.IsValid)
            {
                // *************** Validar que el nombre de equipo haya sido ingresado
                if (equipos.nombre == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre de Equipo con 3 o mas Caracteres";

                    // Si ademas se ha ingresado un pais, entonces lo devuelvo para no perderlo
                    if (equipos.pais != null && equipos.pais.Length > 0) TempData["tempPais"] = equipos.pais;

                    return RedirectToAction("Create");
                }

                // Validar que el nombre de equipo sea mayor a 2 (o no menor que 3)
                if (equipos.nombre.Length < 3)
                {
                   // Si se ha ingresado un nombre de equipo con 1 o 2 caracteres, entonces lo devuelvo para no perderlo
                   TempData["mensajeError"] = "Ingrese un nombre de Equipo con 3 o mas Caracteres";
                   TempData["tempEquipo"] = equipos.nombre;

                   // Si ademas se ha ingresado un pais, entonces lo devuelvo para no perderlo
                   if(equipos.pais != null && equipos.pais.Length > 0) TempData["tempPais"] = equipos.pais;
                 
                   return RedirectToAction("Create");
                }

                // ************** Validar que el nombre de pais haya sido ingresado
                if (equipos.pais == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre de pais con 1 o mas Caracteres";

                    // En estas lineas, ya se ha asegurado que se ha ingresado un nombre de equipo valido, entonces lo devuelvo para no perderlo
                    TempData["tempEquipo"] = equipos.nombre;

                    return RedirectToAction("Create");
                }

                // Le da el mismo formato (Primera letra de cada palabra en mayuscula) a todos los paises ingresados para que el filtro select de paises, no muestre dos veces o mas el mismo pais que se ha escrito, de distinta manera.
                equipos.pais = new CultureInfo("en-US", false).TextInfo.ToTitleCase(equipos.pais);
                string nuevoNombre = equipos.nombre.ToLower();

                // ************** Validar que el nombre de equipo no este repetido si ya existe el pais ingresado
                List<Equipos> equiposRegistrados = db.Equipos.ToList();
                Boolean existePais = equiposRegistrados.Exists(x => x.pais == equipos.pais && x.nombre.ToLower() == nuevoNombre);

                if(existePais)
                {
                    TempData["mensajeError"] = "El nombre de Equipo ingresado ya existe en el pais ingresado, ingrese otro";
                    TempData["tempEquipo"] = equipos.nombre;
                    TempData["tempPais"] = equipos.pais;

                    return RedirectToAction("Create");
                }

                db.Equipos.Add(equipos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipos);
        }

        // GET: Equipos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipos equipos = db.Equipos.Find(id);
            if (equipos == null)
            {
                return HttpNotFound();
            }
            if (TempData["mensajeError"] != null)
            {
                ViewBag.mensajeError = TempData["mensajeError"].ToString();
                if (TempData["tempEquipo"] != null) ViewBag.tempEquipo = TempData["tempEquipo"].ToString();
                if (TempData["tempPais"] != null) ViewBag.tempPais = TempData["tempPais"].ToString();
            }
            return View(equipos);
        }

        // POST: Equipos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre,pais")] Equipos equipos)
        {
            if (ModelState.IsValid)
            {

                // *************** Validar que el nombre de equipo haya sido ingresado
                if (equipos.nombre == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre de Equipo con 3 o mas Caracteres";

                    // Si ademas se ha ingresado un pais, entonces lo devuelvo para no perderlo
                    if (equipos.pais != null && equipos.pais.Length > 0) TempData["tempPais"] = equipos.pais;

                    return RedirectToAction("Edit");
                }

                // Validar que el nombre de equipo sea mayor a 2 (o no menor que 3)
                if (equipos.nombre.Length < 3)
                {
                    // Si se ha ingresado un nombre de equipo con 1 o 2 caracteres, entonces lo devuelvo para no perderlo
                    TempData["mensajeError"] = "Ingrese un nombre de Equipo con 3 o mas Caracteres";
                    TempData["tempEquipo"] = equipos.nombre;

                    // Si ademas se ha ingresado un pais, entonces lo devuelvo para no perderlo
                    if (equipos.pais != null && equipos.pais.Length > 0) TempData["tempPais"] = equipos.pais;

                    return RedirectToAction("Edit");
                }

                // ************** Validar que el nombre de pais haya sido ingresado
                if (equipos.pais == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre de pais con 1 o mas Caracteres";

                    // En estas lineas, ya se ha asegurado que se ha ingresado un nombre de equipo valido, entonces lo devuelvo para no perderlo
                    TempData["tempEquipo"] = equipos.nombre;

                    return RedirectToAction("Edit");
                }

                // ************** Validar que el nombre de equipo no este repetido si ya existe el pais ingresado

                // Primero obtener el nombre original del equipo
                var equipoOriginal = db.Equipos.Where(x => x.id == equipos.id).Select(col => new { col.nombre, col.pais }).First();
                string nombreOriginal = equipoOriginal.nombre.ToLower();
                string paisOriginal = equipoOriginal.pais.ToLower();

                string nuevoNombre = equipos.nombre.ToLower();

                // Le da el mismo formato (Primera letra de cada palabra en mayuscula) a todos los paises ingresados para que el filtro select de paises, no muestre dos veces o mas el mismo pais que se ha escrito, de distinta manera.
                equipos.pais = new CultureInfo("en-US", false).TextInfo.ToTitleCase(equipos.pais);

                string nuevoPais = equipos.pais.ToLower();

                // Validar si han cambiado los atributos
                if(nombreOriginal.Equals(nuevoNombre) && paisOriginal.Equals(nuevoPais))
                {
                    // No ha habido cambio alguno
                    db.Entry(equipos).State = EntityState.Unchanged;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                } else
                {
                    // ha cambiado el nombre de Equipo o el Pais
                    
                    if (db.Equipos.Any(a => a.nombre == nuevoNombre && a.pais == nuevoPais))
                    {
                        TempData["mensajeError"] = "El nombre de Equipo ingresado ya existe en el pais ingresado, ingrese otro";
                        TempData["tempEquipo"] = equipos.nombre;
                        TempData["tempPais"] = equipos.pais;

                        return RedirectToAction("Edit");
                    }
                }
                db.Entry(equipos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipos);
        }

        // GET: Equipos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipos equipos = db.Equipos.Find(id);
            if (equipos == null)
            {
                return HttpNotFound();
            }
            return View(equipos);
        }

        // POST: Equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipos equipos = db.Equipos.Find(id);

            // Elimina todos los jugadores con el id_equipo,el cual se va a eliminar para que la aplicacion no de errores
            db.Database.ExecuteSqlCommand("DELETE FROM Jugadores WHERE id_equipo = @par1",
                    new SqlParameter("@par1", equipos.id));

            db.Equipos.Remove(equipos);
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
