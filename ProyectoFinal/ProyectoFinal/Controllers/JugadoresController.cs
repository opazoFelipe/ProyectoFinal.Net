using System;
using System.Collections.Generic;
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
    public class JugadoresController : Controller
    {
        private FutGolEntities db = new FutGolEntities();

        // GET: Jugadores
        public ActionResult Index()
        {
            // Obtener los paises registrados, sin repetirse. Obtener el pais 2 veces para que el nuevo select list tenga como key y value el nombre del pais
            var paises = db.Equipos.Select(col => new { key = col.pais, value = col.pais} ).Distinct();

            // Devolver el select para filtrar por paises registrados
            ViewBag.Paises = new SelectList(paises.ToList(), "key", "value");
            ViewBag.paisSeleccion = null;
            var jugadores = db.Jugadores.Include(j => j.Equipos);
            return View(jugadores.ToList());
        }

        // Post: Jugadores
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Paises)
        {
            string tempPais = Paises.ToString();

            var paises = db.Equipos.Select(col => new { key = col.pais, value = col.pais }).Distinct();

            // Devolver el mismo select con el pais seleccionado
            ViewBag.Paises = new SelectList(paises.ToList(), "key", "value", Paises);

            // Devolver los jugadores por el pais seleccionado
            var jugadoresPorPais = db.Jugadores.Include(j => j.Equipos).Where(p => p.Equipos.pais == tempPais);
            return View(jugadoresPorPais.ToList());

        }

            // GET: Jugadores/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jugadores jugadores = db.Jugadores.Find(id);
            if (jugadores == null)
            {
                return HttpNotFound();
            }
            return View(jugadores);
        }

        // GET: Jugadores/Create
        public ActionResult Create()
        {
            if (TempData["mensajeError"] != null)
            {
                ViewBag.mensajeError = TempData["mensajeError"].ToString();
                if (TempData["tempNombre"] != null) ViewBag.tempNombre = TempData["tempNombre"].ToString();
                if (TempData["tempApellidos"] != null) ViewBag.tempApellidos = TempData["tempApellidos"].ToString();
                if (TempData["tempPosicion"] != null) ViewBag.tempPosicion = TempData["tempPosicion"].ToString();
            }
            ViewBag.id_equipo = new SelectList(db.Equipos, "id", "nombre");
            return View();
        }

        // POST: Jugadores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombres,apellidos,posicion,id_equipo")] Jugadores jugadores)
        {

            if (ModelState.IsValid)
            {
                // *************** Validar que el nombre del jugador haya sido ingresado y sea mayor a 2 (o no menor que 3)
                if (jugadores.nombres == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre con 3 o mas Caracteres";

                    // Si ademas se ha ingresado apellidos y posicion entonces los devuelvo para no perderlo
                    if (jugadores.apellidos != null && jugadores.apellidos.Length > 0) TempData["tempApellidos"] = jugadores.apellidos;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Create");
                }

                // Validar que el nombre de jugador sea mayor a 2 (o no menor que 3)
                if (jugadores.nombres.Length < 3)
                {
                    TempData["mensajeError"] = "Ingrese un nombre con 3 o mas Caracteres";

                    // devuelvo el nombre ingresado con menos de 3 letras
                    TempData["tempNombre"] = jugadores.nombres;

                    // Si ademas se ha ingresado apellidos y posicion entonces los devuelvo para no perderlo
                    if (jugadores.apellidos != null && jugadores.apellidos.Length > 0) TempData["tempApellidos"] = jugadores.apellidos;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Create");
                }

                // *************** Validar que apellidos del jugador hayan sido ingresado
                if (jugadores.apellidos == null)
                {
                    TempData["mensajeError"] = "El campo apellidos debe contener 3 o mas Caracteres";

                    // En estas lineas ya se ha validado que se ha ingresado un nombre valido, por lo tanto, lo devuelvo
                    TempData["tempNombre"] = jugadores.nombres;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Create");
                }

                // Validar que los apellidos sean mayor a 2 (o no menor que 3)
                if (jugadores.apellidos.Length < 3)
                {
                    TempData["mensajeError"] = "El campo apellidos debe contener 3 o mas Caracteres";

                    // devuelvo el nombre ingresado con menos de 3 letras
                    TempData["tempNombre"] = jugadores.nombres;
                    TempData["tempApellidos"] = jugadores.apellidos;

                    // Si ademas se ha ingresado posicion entonces la devuelvo para no perderla
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Create");
                }

                if (jugadores.apellidos == null)
                {
                    TempData["mensajeError"] = "El campo apellidos debe contener 3 o mas Caracteres";

                    // Si ademas se ha ingresado nombres y posicion entonces los devuelvo para no perderlo
                    TempData["tempNombres"] = jugadores.nombres;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Create");
                }

                string nuevaPosicion = "";

                if (jugadores.posicion != null)
                {
                    // Para que el programa no crashee si la posicion ingresada tiene largo mas que 10
                    nuevaPosicion = jugadores.posicion.ToString();
                    if(nuevaPosicion.Length > 10) nuevaPosicion = nuevaPosicion.Substring(0, 10);
                } 
      
                db.Database.ExecuteSqlCommand("INSERT INTO Jugadores(nombres, apellidos, posicion, id_equipo) VALUES(@par1, @par2, @par3, @par4)",
                    new SqlParameter("@par1", jugadores.nombres.ToString()),
                    new SqlParameter("@par2", jugadores.apellidos.ToString()),
                    new SqlParameter("@par3", nuevaPosicion),
                    new SqlParameter("@par4", jugadores.id_equipo));

                // Por algun motivo, me da error al guardar el nuevo jugador usando este metodo, por lo tanto use la query anterior
                //db.Jugadores.Add(jugadores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_equipo = new SelectList(db.Equipos, "id", "nombre", jugadores.id_equipo);
            return View(jugadores);
        }

        // GET: Jugadores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jugadores jugadores = db.Jugadores.Find(id);
            if (jugadores == null)
            {
                return HttpNotFound();
            }
            if (TempData["mensajeError"] != null)
            {
                ViewBag.mensajeError = TempData["mensajeError"].ToString();
                if (TempData["tempNombre"] != null) ViewBag.tempNombre = TempData["tempNombre"].ToString();
                if (TempData["tempApellidos"] != null) ViewBag.tempApellidos = TempData["tempApellidos"].ToString();
                if (TempData["tempPosicion"] != null) ViewBag.tempPosicion = TempData["tempPosicion"].ToString();
            }
            ViewBag.id_equipo = new SelectList(db.Equipos, "id", "nombre", jugadores.id_equipo);
            return View(jugadores);
        }

        // POST: Jugadores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombres,apellidos,posicion,id_equipo")] Jugadores jugadores)
        {
            if (ModelState.IsValid)
            {
                // *************** Validar que el nombre del jugador haya sido ingresado y sea mayor a 2 (o no menor que 3)
                if (jugadores.nombres == null)
                {
                    TempData["mensajeError"] = "Ingrese un nombre con 3 o mas Caracteres";

                    // Si ademas se ha ingresado apellidos y posicion entonces los devuelvo para no perderlo
                    if (jugadores.apellidos != null && jugadores.apellidos.Length > 0) TempData["tempApellidos"] = jugadores.apellidos;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Edit");
                }

                // Validar que el nombre de jugador sea mayor a 2 (o no menor que 3)
                if (jugadores.nombres.Length < 3)
                {
                    TempData["mensajeError"] = "Ingrese un nombre con 3 o mas Caracteres";

                    // devuelvo el nombre ingresado con menos de 3 letras
                    TempData["tempNombre"] = jugadores.nombres;

                    // Si ademas se ha ingresado apellidos y posicion entonces los devuelvo para no perderlo
                    if (jugadores.apellidos != null && jugadores.apellidos.Length > 0) TempData["tempApellidos"] = jugadores.apellidos;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Edit");
                }

                // *************** Validar que apellidos del jugador hayan sido ingresado
                if (jugadores.apellidos == null)
                {
                    TempData["mensajeError"] = "El campo apellidos debe contener 3 o mas Caracteres";

                    // Si ademas se ha ingresado nombres y posicion entonces los devuelvo para no perderlo
                    TempData["tempNombre"] = jugadores.nombres;
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Edit");
                }

                // Validar que los apellidos sean mayor a 2 (o no menor que 3)
                if (jugadores.apellidos.Length < 3)
                {
                    TempData["mensajeError"] = "El campo apellidos debe contener 3 o mas Caracteres";

                    // devuelvo nombres y apellidos ingresados con menos de 3 letras
                    TempData["tempNombre"] = jugadores.nombres;
                    TempData["tempApellidos"] = jugadores.apellidos;

                    // Si ademas se ha ingresado posicion entonces la devuelvo para no perderla
                    if (jugadores.posicion != null && jugadores.posicion.Length > 0) TempData["tempPosicion"] = jugadores.posicion;

                    return RedirectToAction("Edit");
                }

                // Para que no de problemas al guardar cuando el campo posicion ha sido modificado por vacio.
                if (jugadores.posicion == null) jugadores.posicion = "";

                // Para que el programa no crashee si la posicion ingresada tiene largo mas que 10
                if (jugadores.posicion != null && jugadores.posicion.Length > 10) jugadores.posicion = jugadores.posicion.Substring(0, 10);

                db.Entry(jugadores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_equipo = new SelectList(db.Equipos, "id", "nombre", jugadores.id_equipo);
            return View(jugadores);
        }

        // GET: Jugadores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jugadores jugadores = db.Jugadores.Find(id);
            if (jugadores == null)
            {
                return HttpNotFound();
            }
            return View(jugadores);
        }

        // POST: Jugadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Jugadores jugadores = db.Jugadores.Find(id);
            db.Jugadores.Remove(jugadores);
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
