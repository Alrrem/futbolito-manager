using FutbolitoManager.Data;
using FutbolitoManager.Models;
using FutbolitoManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;

namespace FutbolitoManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AdministradorManager _adminManager;

        // Constructor con inyección de dependencia
        public HomeController(AppDbContext context)
        {
            _context = context;
            _adminManager = new AdministradorManager(context);
        }

        //INDEX
        public IActionResult Index(int? editarId = null)
        {
            var equipos = _context.Equipos.ToList();
            var noticias = _context.Noticias
                .OrderByDescending(n => n.FechaPublicacion)
                .ToList();

            ViewBag.Noticias = noticias;
            ViewBag.EsAdmin = HttpContext.Session.GetString("EsAdmin") == "true";
            ViewBag.EditarId = editarId;

            return View(equipos);
        }

        //------------------------------------------------------------------------

        //NOTICIAS
        //ELIMINAR NOTICIA
        [HttpPost]
        public IActionResult EliminarNoticia(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var noticia = _context.Noticias.Find(id);
            if (noticia != null)
            {
                _context.Noticias.Remove(noticia);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        //---------------------------------------------------------
        //EDITAR NOTICA
        [HttpPost]
        public IActionResult EditarNoticia(int id, string titulo, string contenido)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var noticia = _context.Noticias.Find(id);
            if (noticia != null)
            {
                noticia.Titulo = titulo;
                noticia.Contenido = contenido;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        //---------------------------------------------------------------
        // Guardar nueva noticia
        [HttpPost]
        public IActionResult AgregarNoticia(string titulo, string contenido)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var noticia = new Noticia
            {
                Titulo = titulo,
                Contenido = contenido,
                FechaPublicacion = DateTime.Now
            };

            _context.Noticias.Add(noticia);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //----------------------------------------------------------

        //EQUIPOS 
        public IActionResult Equipos()
        {
            var equipos = _context.Equipos.ToList();
            ViewBag.EsAdmin = HttpContext.Session.GetString("EsAdmin") == "true";
            return View(equipos);
        }


        //BORRAR EQUIPO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BorrarEquipo(int id)
        {
            // 1) Sólo el admin puede borrar
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            // 2) Cargamos el equipo junto con sus jugadores
            var equipo = _context.Equipos
                .Include(e => e.Jugadores)
                .FirstOrDefault(e => e.Id == id);
            if (equipo == null)
                return NotFound();

            // 3) Eliminamos primero los registros de goles de esos jugadores
            var jugadorIds = equipo.Jugadores.Select(j => j.Id).ToList();
            var golesJugadores = _context.PartidoJugadores
                .Where(pj => jugadorIds.Contains(pj.JugadorId));
            _context.PartidoJugadores.RemoveRange(golesJugadores);

            // 4) Eliminamos los propios jugadores
            _context.Jugadores.RemoveRange(equipo.Jugadores);

            // 5) Ahora eliminamos los partidos en los que participó este equipo
            //    (evita conflict FK con Partidos→Equipos)
            var partidos = _context.Partidos
                .Where(p => p.EquipoLocalId == id || p.EquipoVisitanteId == id);
            // Antes de borrar cada partido, eliminamos sus detalles en PartidoJugadores
            var partidoIds = partidos.Select(p => p.Id).ToList();
            var detalles = _context.PartidoJugadores
                .Where(d => partidoIds.Contains(d.PartidoId));
            _context.PartidoJugadores.RemoveRange(detalles);
            _context.Partidos.RemoveRange(partidos);

            // 6) Finalmente, eliminamos el equipo
            _context.Equipos.Remove(equipo);

            // 7) Persistimos todos los cambios en una sola transacción
            _context.SaveChanges();

            return RedirectToAction("Equipos");
        }



        //-----------------------------------------------

        //CANCHAS

        public IActionResult Canchas()
        {
            var canchas = _context.Canchas.ToList();
            ViewBag.EsAdmin = HttpContext.Session.GetString("EsAdmin") == "true";
            return View(canchas);
        }
        //AGREGAR CANCHA

        public IActionResult AgregarCancha()
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");
            return View();
        }
        //EDITAR CANCHA

        [HttpGet]
        public IActionResult EditarCancha(int id)
        {
            // Solo admin
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            var cancha = _context.Canchas.Find(id);
            if (cancha == null)
                return NotFound();

            return View(cancha);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCancha(int id, string nombre, string ubicacion, string resena, IFormFile imagen)
        {
            // Solo admin
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var cancha = _context.Canchas.FirstOrDefault(c => c.Id == id);
            if (cancha == null)
                return NotFound();

            // Actualizamos campos
            cancha.Nombre = nombre;
            cancha.Ubicacion = ubicacion;
            cancha.Resena = resena;

            // Si subieron nueva imagen, la reemplazamos
            if (imagen != null && imagen.Length > 0)
            {
                using var ms = new MemoryStream();
                imagen.CopyTo(ms);
                cancha.Imagen = ms.ToArray();
            }

            _context.SaveChanges();
            return RedirectToAction("FichaCancha", new { id = cancha.Id });
        }

        //-----------------------------------------------
        //GUARDAR CANCHA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarCancha(string nombre, string ubicacion, string resena, IFormFile imagen)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            byte[] imgBytes = null;
            if (imagen != null && imagen.Length > 0)
            {
                using var ms = new MemoryStream();
                imagen.CopyTo(ms);
                imgBytes = ms.ToArray();
            }

            var nueva = new Cancha
            {
                Nombre = nombre,
                Ubicacion = ubicacion,
                Resena = resena,
                Imagen = imgBytes
            };

            _context.Canchas.Add(nueva);
            _context.SaveChanges();

            return RedirectToAction("Canchas");
        }


        //------------------------------------------------

        //FICHA CANCHA

        public IActionResult FichaCancha(int id)
        {
            var cancha = _context.Canchas.FirstOrDefault(c => c.Id == id);
            if (cancha == null) return NotFound();
            return View(cancha);
        }

        //------------------------------------------------
        //BORRAR CANCHA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BorrarCancha(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var cancha = _context.Canchas.Find(id);
            if (cancha != null)
            {
                _context.Canchas.Remove(cancha);
                _context.SaveChanges();
            }
            return RedirectToAction("Canchas");
        }

        //----------------------------------------------
        //ESTADISTICAS
        public IActionResult Estadisticas()
        {
            var equipos = _context.Equipos.ToList();

            // Solo partidos finalizados
            var partidos = _context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Where(p => p.Finalizado) // ← Esto filtra SOLO los finalizados
                .ToList();

            var tablaPosiciones = equipos.Select(e =>
            {
                var local = partidos.Where(p => p.EquipoLocalId == e.Id);
                var visitante = partidos.Where(p => p.EquipoVisitanteId == e.Id);

                int pj = local.Count() + visitante.Count();
                int pg = local.Count(p => p.GolesLocal > p.GolesVisitante)
                        + visitante.Count(p => p.GolesVisitante > p.GolesLocal);
                int pe = local.Count(p => p.GolesLocal == p.GolesVisitante)
                        + visitante.Count(p => p.GolesVisitante == p.GolesLocal);
                int pp = pj - pg - pe;
                int gf = local.Sum(p => p.GolesLocal) + visitante.Sum(p => p.GolesVisitante);
                int gc = local.Sum(p => p.GolesVisitante) + visitante.Sum(p => p.GolesLocal);
                int pts = pg * 3 + pe;

                return new TablaPosicion
                {
                    Equipo = e.Nombre,
                    PJ = pj,
                    PG = pg,
                    PE = pe,
                    PP = pp,
                    GF = gf,
                    GC = gc,
                    DIF = gf - gc,
                    PTS = pts
                };
            })
            .OrderByDescending(x => x.PTS)
            .ThenByDescending(x => x.DIF)
            .ThenByDescending(x => x.GF)
            .ToList();

            List<object> goleadores = new();

            if (partidos.Any())
            {
                var mapaEquipos = equipos.ToDictionary(x => x.Id, x => x.Nombre);
                goleadores = _context.Jugadores
                    .Where(j => j.Goles > 0)
                    .OrderByDescending(j => j.Goles)
                    .Select(j => new
                    {
                        j.Nombre,
                        Equipo = mapaEquipos.ContainsKey(j.EquipoId) ? mapaEquipos[j.EquipoId] : "Sin equipo",
                        j.Goles
                    })
                    .ToList<object>();
            }

            ViewBag.Posiciones = tablaPosiciones;
            ViewBag.Goleadores = goleadores;
            ViewBag.HasPartidos = partidos.Any();

            return View();
        }


        //----------------------------------------------------------

        //FECHAS
        public IActionResult Fechas()
        {
            // Para el dropdown de equipos (viejo) y ahora de canchas:
            ViewBag.Equipos = _context.Equipos.ToList();
            ViewBag.Canchas = _context.Canchas.ToList();
            ViewBag.EsAdmin = HttpContext.Session.GetString("EsAdmin") == "true";

            // Traemos partidos incluyendo cancha
            var partidos = _context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Cancha)           // ← incluimos la cancha
                .OrderBy(p => p.Fecha)
                .ToList();

            ViewBag.Partidos = partidos;
            return View();
        }



        //--------------------------------------------------

        //LOGIN LOGUOT
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Administradores.FirstOrDefault(a => a.Email == email);

            if (admin != null && SecurityHelper.VerifyPassword(password, admin.Password, admin.Salt))
            {
                HttpContext.Session.SetString("EsAdmin", "true");
                HttpContext.Session.SetString("AdminEmail", admin.Email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("EsAdmin");
            HttpContext.Session.Remove("AdminEmail");
            return RedirectToAction("Index", "Home");
        }



        //-----------------------------------------------------------------------

        // CODIGOS PARA EQUIPOS!!

        //Agregar un equipo

        public IActionResult AgregarEquipo()
        {
            // Solo permitir acceso si es administrador
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            return View();
        }
        //------------------------------

        //Actualizar Equipo

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEquipo(
            int id,
            string Nombre,
            string Vestimenta,
            string Portero,
            string Capitan,
            int Jugadores,      // ahora a JugadoresEnBanca
            string Balon,
            IFormFile Logo)
        {
            var equipo = _context.Equipos.FirstOrDefault(e => e.Id == id);
            if (equipo == null) return NotFound();

            equipo.Nombre = Nombre;
            equipo.Vestimenta = Vestimenta;
            equipo.Portero = Portero;
            equipo.Capitan = Capitan;
            equipo.JugadoresEnBanca = Jugadores;   // ← aquí
            equipo.Balon = Balon;

            if (Logo != null && Logo.Length > 0)
            {
                using var stream = new MemoryStream();
                Logo.CopyTo(stream);
                equipo.Logo = stream.ToArray();
            }

            _context.SaveChanges();
            return RedirectToAction("FichaEquipo", new { id = equipo.Id });
        }

        //-----------------------------



        //--------------------------------------------------
        //Guardar Partido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPartido(
            int equipoLocalId,
            int equipoVisitanteId,
            int canchaId,
            int golesLocal,
            int golesVisitante,
            DateTime fecha,
            bool finalizado)
        {
            // 1) Sólo admin
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            // 2) Validaciones: 
            //    - dos equipos distintos y válidos
            //    - cancha válida
            if (equipoLocalId == equipoVisitanteId
                || !_context.Equipos.Any(e => e.Id == equipoLocalId)
                || !_context.Equipos.Any(e => e.Id == equipoVisitanteId)
                || !_context.Canchas.Any(c => c.Id == canchaId))
            {
                ModelState.AddModelError("", "Debe elegir dos equipos distintos y una cancha válida.");
            }

            if (!ModelState.IsValid)
            {
                // Re-llenar dropdowns y lista de partidos
                ViewBag.Equipos = _context.Equipos.ToList();
                ViewBag.Canchas = _context.Canchas.ToList();
                ViewBag.Partidos = _context.Partidos
                                           .Include(p => p.EquipoLocal)
                                           .Include(p => p.EquipoVisitante)
                                           .Include(p => p.Cancha)
                                           .OrderBy(p => p.Fecha)
                                           .ToList();
                ViewBag.EsAdmin = true;
                return View("Fechas");
            }

            // 3) Creamos el partido incluyendo la cancha
            var partido = new Partido
            {
                EquipoLocalId = equipoLocalId,
                EquipoVisitanteId = equipoVisitanteId,
                CanchaId = canchaId,
                GolesLocal = golesLocal,
                GolesVisitante = golesVisitante,
                Fecha = fecha,
                Finalizado = finalizado
            };

            _context.Partidos.Add(partido);
            _context.SaveChanges();

            // 4) Si finalizado, procesamos goles por jugador (igual que antes)
            if (finalizado)
            {
                var form = Request.Form;

                // Locales
                foreach (var idStr in form["jugadores_local"])
                {
                    if (int.TryParse(idStr, out int jugadorId))
                    {
                        var goles = form[$"goles_local_{jugadorId}"];
                        if (int.TryParse(goles, out int cantidad) && cantidad > 0)
                        {
                            _context.PartidoJugadores.Add(new PartidoJugador
                            {
                                PartidoId = partido.Id,
                                JugadorId = jugadorId,
                                Goles = cantidad
                            });
                            var jugador = _context.Jugadores.Find(jugadorId);
                            if (jugador != null) jugador.Goles += cantidad;
                        }
                    }
                }

                // Visitantes
                foreach (var idStr in form["jugadores_visitante"])
                {
                    if (int.TryParse(idStr, out int jugadorId))
                    {
                        var goles = form[$"goles_visitante_{jugadorId}"];
                        if (int.TryParse(goles, out int cantidad) && cantidad > 0)
                        {
                            _context.PartidoJugadores.Add(new PartidoJugador
                            {
                                PartidoId = partido.Id,
                                JugadorId = jugadorId,
                                Goles = cantidad
                            });
                            var jugador = _context.Jugadores.Find(jugadorId);
                            if (jugador != null) jugador.Goles += cantidad;
                        }
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Fechas");
        }

        private string NormalizarRut(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut))
                return string.Empty;

            return rut
                .Replace(".", "")
                .Replace("-", "")
                .Trim()
                .ToUpperInvariant();
        }




        //--------------------------------------------------
        //BORRAR PARTIDO

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BorrarPartido(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var partido = _context.Partidos
                .Include(p => p.PartidoJugadores)
                .FirstOrDefault(p => p.Id == id);

            if (partido != null)
            {
                // Eliminar primero los hijos
                _context.PartidoJugadores.RemoveRange(partido.PartidoJugadores);

                // Luego eliminar el padre
                _context.Partidos.Remove(partido);

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        //--------------------------------------------------
        //EDITAR PARTIDO
        // GET: EditarPartido

        [HttpGet]
        public IActionResult EditarPartido(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            var partido = _context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Cancha)
                .Include(p => p.PartidoJugadores)
                .FirstOrDefault(p => p.Id == id);

            if (partido == null)
                return NotFound();

            ViewBag.Equipos = _context.Equipos.ToList();
            ViewBag.Canchas = _context.Canchas.ToList();

            // Diccionario JugadorId → Goles para pre-llenar los inputs
            ViewBag.ExistingGoals = partido.PartidoJugadores
                .ToDictionary(pj => pj.JugadorId, pj => pj.Goles);

            return View(partido);
        }

        // POST: Home/EditarPartido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPartido(
            int id,
            int equipoLocalId,
            int equipoVisitanteId,
            int canchaId,
            int golesLocal,
            int golesVisitante,
            DateTime fecha,
            bool finalizado)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var partido = _context.Partidos
                .Include(p => p.PartidoJugadores)
                .FirstOrDefault(p => p.Id == id);
            if (partido == null)
                return NotFound();

            // Actualizar datos básicos
            partido.EquipoLocalId = equipoLocalId;
            partido.EquipoVisitanteId = equipoVisitanteId;
            partido.CanchaId = canchaId;
            partido.GolesLocal = golesLocal;
            partido.GolesVisitante = golesVisitante;
            partido.Fecha = fecha;
            partido.Finalizado = finalizado;

            // Limpiar goles antiguos
            if (partido.PartidoJugadores.Any())
                _context.PartidoJugadores.RemoveRange(partido.PartidoJugadores);

            // Procesar goles de jugadores si finalizado
            if (finalizado)
            {
                var form = Request.Form;

                foreach (var idStr in form["jugadores_local"])
                {
                    if (int.TryParse(idStr, out int jugadorId))
                    {
                        var val = form[$"goles_local_{jugadorId}"];
                        if (int.TryParse(val, out int cnt) && cnt > 0)
                        {
                            _context.PartidoJugadores.Add(new PartidoJugador
                            {
                                PartidoId = id,
                                JugadorId = jugadorId,
                                Goles = cnt
                            });
                            var j = _context.Jugadores.Find(jugadorId);
                            if (j != null) j.Goles += cnt;
                        }
                    }
                }

                foreach (var idStr in form["jugadores_visitante"])
                {
                    if (int.TryParse(idStr, out int jugadorId))
                    {
                        var val = form[$"goles_visitante_{jugadorId}"];
                        if (int.TryParse(val, out int cnt) && cnt > 0)
                        {
                            _context.PartidoJugadores.Add(new PartidoJugador
                            {
                                PartidoId = id,
                                JugadorId = jugadorId,
                                Goles = cnt
                            });
                            var j = _context.Jugadores.Find(jugadorId);
                            if (j != null) j.Goles += cnt;
                        }
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Fechas");
        }



        //--------------------------------------------------
        public IActionResult Reglamento()
        {
            return View();
        }
        //ficha equipo
        public IActionResult FichaEquipo(int id)
        {
            var equipo = _context.Equipos.FirstOrDefault(e => e.Id == id);
            if (equipo == null)
                return NotFound();

            var jugadores = _context.Jugadores
                .Where(j => j.EquipoId == id)
                .ToList();

            ViewBag.Jugadores = jugadores;

            return View(equipo);
        }

        //----------------------------------------


        //editar equipo

        public IActionResult EditarEquipo(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            var equipo = _context.Equipos.FirstOrDefault(e => e.Id == id);
            if (equipo == null)
                return NotFound();

            return View(equipo);
        }


        //-----------------------------------------


        //guardar equipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarEquipo(
    string nombre,
    string vestimenta,
    string portero,
    string capitan,
    int jugadoresEnBanca,
    string balon,
    IFormFile logo)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrWhiteSpace(nombre) ||
                    string.IsNullOrWhiteSpace(vestimenta) ||
                    string.IsNullOrWhiteSpace(portero) ||
                    string.IsNullOrWhiteSpace(capitan) ||
                    string.IsNullOrWhiteSpace(balon) ||
                    jugadoresEnBanca < 7 || jugadoresEnBanca > 10)
                {
                    ModelState.AddModelError("", "Por favor, complete todos los campos correctamente.");
                    return View("AgregarEquipo");
                }

                byte[] imagenEnBytes = null;
                if (logo != null && logo.Length > 0)
                {
                    using var stream = new MemoryStream();
                    logo.CopyTo(stream);
                    imagenEnBytes = stream.ToArray();
                }

                var nuevoEquipo = new Equipo
                {
                    Nombre = nombre,
                    Vestimenta = vestimenta,
                    Portero = portero,
                    Capitan = capitan,
                    JugadoresEnBanca = jugadoresEnBanca,
                    Balon = balon,
                    Logo = imagenEnBytes
                };

                _context.Equipos.Add(nuevoEquipo);
                _context.SaveChanges();

                return RedirectToAction("Equipos");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al guardar el equipo: " + ex.Message);
            }
        }

        //JUGADORES!!!
        //Agregar Jugador

        [HttpGet]

        public IActionResult AgregarJugador(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            ViewBag.EquipoId = id;
            return View();
        }

        //----------------------------------------------------------


        //Editar Jugador

        // GET: muestra el formulario de edición
        [HttpGet]
        public IActionResult EditarJugador(int id)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            var jugador = _context.Jugadores.Find(id);
            if (jugador == null) return NotFound();

            return View(jugador);  // Views/Home/EditarJugador.cshtml
        }

        // POST: guarda los cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarJugador(int id, string nombre, string rut, int edad, string posicion)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            var jugador = _context.Jugadores.Find(id);
            if (jugador == null) return NotFound();

            // Validar duplicados de RUT (salvo él mismo)
            if (_context.Jugadores.Any(j => j.Rut == rut && j.Id != id))
            {
                ModelState.AddModelError(nameof(rut), $"Ya existe otro jugador con RUT {rut}.");
            }
            if (edad < 10 || edad > 12)
            {
                ModelState.AddModelError(nameof(edad), "La edad debe estar entre 10 y 12 años.");
            }
            if (!ModelState.IsValid)
            {
                return View(jugador);
            }

            jugador.Nombre = nombre;
            jugador.Rut = rut;
            jugador.Edad = edad;
            jugador.Posicion = posicion;
            _context.SaveChanges();

            return RedirectToAction("FichaEquipo", new { id = jugador.EquipoId });
        }


        //----------------------------------------------------------


        //Importar Jugadores

        // GET: muestra el formulario de importación
        [HttpGet]
        public IActionResult ImportarJugadores(int id)
        {
            // Solo admin
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return RedirectToAction("Login");

            ViewBag.EquipoId = id;
            return View();  // busca Views/Home/ImportarJugadores.cshtml
        }

        // POST: /Home/ImportarJugadores/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarJugadores(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ImportResult"] = "⚠️ Debes seleccionar un archivo válido.";
                return RedirectToAction("FichaEquipo", new { id });
            }

            var jugadoresImportados = new List<Jugador>();
            var errores = new List<string>();

            // Asegúrate de haber llamado a:
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            // Si usas EPPlus:
            using (var package = new ExcelPackage(stream))
            {
                var hoja = package.Workbook.Worksheets.FirstOrDefault();
                if (hoja == null)
                {
                    TempData["ImportResult"] = "⚠️ El archivo no contiene hojas válidas.";
                    return RedirectToAction("FichaEquipo", new { id });
                }

                int filas = hoja.Dimension.End.Row;
                for (int fila = 2; fila <= filas; fila++)
                {
                    var nombre = hoja.Cells[fila, 1].Text.Trim();
                    var rut = hoja.Cells[fila, 2].Text.Trim();
                    var edadStr = hoja.Cells[fila, 3].Text.Trim();
                    var pos = hoja.Cells[fila, 4].Text.Trim();

                    if (string.IsNullOrWhiteSpace(nombre) ||
                        string.IsNullOrWhiteSpace(rut) ||
                        string.IsNullOrWhiteSpace(edadStr) ||
                        string.IsNullOrWhiteSpace(pos))
                    {
                        errores.Add($"Fila {fila}: Campos incompletos.");
                        continue;
                    }
                    if (!int.TryParse(edadStr, out int edad) || edad < 10 || edad > 12)
                    {
                        errores.Add($"Fila {fila}: Edad inválida ({edadStr}). Debe ser 10–12.");
                        continue;
                    }
                    if (_context.Jugadores.Any(j => j.Rut == rut))
                    {
                        errores.Add($"Fila {fila}: Ya existe jugador con RUT {rut}.");
                        continue;
                    }

                    jugadoresImportados.Add(new Jugador
                    {
                        Nombre = nombre,
                        Rut = rut,
                        Edad = edad,
                        Posicion = pos,
                        EquipoId = id,
                        Goles = 0
                    });
                }
            }

            if (jugadoresImportados.Any())
            {
                _context.Jugadores.AddRange(jugadoresImportados);
                await _context.SaveChangesAsync();
                TempData["ImportResult"] = $"✅ Se importaron {jugadoresImportados.Count} jugador(es).";
            }

            if (errores.Any())
            {
                TempData["ImportResult"] += " Errores: " + string.Join(" | ", errores);
            }

            return RedirectToAction("FichaEquipo", new { id });
        }


        //----------------------------------------------------------

        //Guardar Jugador


        [HttpPost]
        [HttpPost]
        public IActionResult GuardarJugador(JugadorFormModel datos)
        {
            // 1) Validación del modelo
            if (!ModelState.IsValid)
            {
                ViewBag.EquipoId = datos.EquipoId;
                return View("AgregarJugador", datos);
            }

            // 2) Validar que el equipo exista
            if (!_context.Equipos.Any(e => e.Id == datos.EquipoId))
                return NotFound();

            // 3) Normalizamos el RUT de entrada
            var rutLimpio = NormalizarRut(datos.Rut);

            // 4) Comprobamos duplicados sobre la columna Rut existente,
            //    normalizando también la comparación en la base de datos.
            bool existe = _context.Jugadores
                .Any(j => j.Rut
                    .Replace(".", "")
                    .Replace("-", "")
                    .ToUpper() == rutLimpio);

            if (existe)
            {
                ModelState.AddModelError(nameof(datos.Rut), "Ya existe un jugador con ese RUT.");
                ViewBag.EquipoId = datos.EquipoId;
                return View("AgregarJugador", datos);
            }

            // 5) Creamos el jugador usando el RUT tal cual vino (o, si prefieres,
            //    guardarlo siempre en formato limpio, sustitúyelo aquí por rutLimpio).
            var nuevoJugador = new Jugador
            {
                Nombre = datos.Nombre,
                Rut = datos.Rut, // o bien Rut = rutLimpio
                Edad = datos.Edad,
                Posicion = datos.Posicion,
                Goles = datos.Goles,
                EquipoId = datos.EquipoId
            };

            _context.Jugadores.Add(nuevoJugador);
            _context.SaveChanges();

            return RedirectToAction("FichaEquipo", new { id = datos.EquipoId });
        }

        //Buscar JUGADOR

        public IActionResult BuscarJugador(string termino)
        {
            var encontrados = _context.Jugadores
                .Where(j => j.Rut.Contains(termino))
                .ToList();

            ViewBag.Termino = termino;
            return View(encontrados);
        }

        //-------------------------------


        //-------------------------------
        //Borrar Jugador

        public IActionResult BorrarJugador(int id)
        {
            // Busca el jugador
            var jugador = _context.Jugadores.FirstOrDefault(j => j.Id == id);
            if (jugador == null)
                return NotFound();

            // Guarda el equipo para redirigir después
            var equipoId = jugador.EquipoId;

            // Elimina y guarda cambios
            _context.Jugadores.Remove(jugador);
            _context.SaveChanges();

            // Vuelve a la ficha de ese equipo
            return RedirectToAction("FichaEquipo", new { id = equipoId });
        }

        //-------------------------------

        //Agregar Varios Jugadores

        // GET: Muestra el formulario para agregar múltiples jugadores
        [HttpGet]
        public IActionResult AgregarJugadoresMultiple(int id)
        {
            // id = EquipoId
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            ViewBag.EquipoId = id;
            return View();
        }

        // POST: Procesa la lista de jugadores enviados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarJugadoresMultiple(int equipoId, List<JugadorFormModel> jugadores)
        {
            if (HttpContext.Session.GetString("EsAdmin") != "true")
                return Forbid();

            // Validación de cada modelo
            for (int i = 0; i < jugadores.Count; i++)
            {
                var model = jugadores[i];
                if (!TryValidateModel(model, $"jugadores[{i}]"))
                {
                    ViewBag.EquipoId = equipoId;
                    return View(jugadores);
                }
            }

            // Guardar todos los jugadores
            foreach (var datos in jugadores)
            {
                var nuevo = new FutbolitoManager.Models.Jugador
                {
                    Nombre = datos.Nombre,
                    Rut = datos.Rut,
                    Edad = datos.Edad,
                    Posicion = datos.Posicion,
                    Goles = datos.Goles,
                    EquipoId = equipoId
                };
                _context.Jugadores.Add(nuevo);
            }
            _context.SaveChanges();

            return RedirectToAction("FichaEquipo", new { id = equipoId });
        }

        //-----------------------------------------------

        //obtenerjugadorespor equipo
        [HttpGet]
        public JsonResult ObtenerJugadoresPorEquipo(int equipoId)
        {
            var jugadores = _context.Jugadores
                .Where(j => j.EquipoId == equipoId)
                .Select(j => new
                {
                    id = j.Id,
                    nombre = j.Nombre
                })
                .ToList();

            return Json(jugadores);
        }
    }


}
