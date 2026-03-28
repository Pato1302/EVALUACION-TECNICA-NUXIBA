using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaApi.Data;
using NuxibaApi.Models;
using System.Text;

namespace NuxibaApi.Controllers
{
    [ApiController]
    [Route("logins")]
    public class LoginsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /logins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            var logins = await _context.Logins.ToListAsync();
            return Ok(logins);
        }

        // GET /logins/{id}  (extra útil para probar)
        [HttpGet("{id}")]
        public async Task<ActionResult<Login>> GetLoginById(int id)
        {
            var login = await _context.Logins.FindAsync(id);

            if (login == null)
            {
                return NotFound(new { message = "Registro no encontrado." });
            }

            return Ok(login);
        }

        // POST /logins
        [HttpPost]
        public async Task<ActionResult<Login>> CreateLogin([FromBody] Login login)
        {
            // Validar TipoMov
            if (login.TipoMov != 0 && login.TipoMov != 1)
            {
                return BadRequest(new { message = "TipoMov debe ser 1 (login) o 0 (logout)." });
            }

            // Validar fecha
            if (login.fecha == default)
            {
                return BadRequest(new { message = "La fecha es obligatoria." });
            }

            // Validar que el usuario exista en ccUsers
            var userExists = await _context.Users.AnyAsync(u => u.User_id == login.User_id);
            if (!userExists)
            {
                return BadRequest(new { message = "El User_id no existe en ccUsers." });
            }

            // Obtener el último movimiento del usuario
            var lastMovement = await _context.Logins
                .Where(l => l.User_id == login.User_id)
                .OrderByDescending(l => l.fecha)
                .FirstOrDefaultAsync();

            // Validación de secuencia login/logout
            if (login.TipoMov == 1) // quiere hacer login
            {
                if (lastMovement != null && lastMovement.TipoMov == 1)
                {
                    return BadRequest(new { message = "No se puede registrar un login si no existe un logout previo." });
                }
            }
            else if (login.TipoMov == 0) // quiere hacer logout
            {
                if (lastMovement == null || lastMovement.TipoMov == 0)
                {
                    return BadRequest(new { message = "No se puede registrar un logout sin un login previo." });
                }
            }

            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoginById), new { id = login.Id }, login);
        }

        // PUT /logins/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Login>> UpdateLogin(int id, [FromBody] Login updatedLogin)
        {
            var existingLogin = await _context.Logins.FindAsync(id);

            if (existingLogin == null)
            {
                return NotFound(new { message = "Registro no encontrado." });
            }

            // Validar TipoMov
            if (updatedLogin.TipoMov != 0 && updatedLogin.TipoMov != 1)
            {
                return BadRequest(new { message = "TipoMov debe ser 1 (login) o 0 (logout)." });
            }

            // Validar fecha
            if (updatedLogin.fecha == default)
            {
                return BadRequest(new { message = "La fecha es obligatoria." });
            }

            // Validar que el usuario exista
            var userExists = await _context.Users.AnyAsync(u => u.User_id == updatedLogin.User_id);
            if (!userExists)
            {
                return BadRequest(new { message = "El User_id no existe en ccUsers." });
            }

            // Actualizar campos
            existingLogin.User_id = updatedLogin.User_id;
            existingLogin.Extension = updatedLogin.Extension;
            existingLogin.TipoMov = updatedLogin.TipoMov;
            existingLogin.fecha = updatedLogin.fecha;

            await _context.SaveChangesAsync();

            return Ok(existingLogin);
        }

        // DELETE /logins/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);

            if (login == null)
            {
                return NotFound(new { message = "Registro no encontrado." });
            }

            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportCsv()
        {
            // Traer todos los usuarios
            var users = await _context.Users.ToListAsync();

            // Traer todas las áreas
            var areas = await _context.Areas.ToListAsync();

            // Traer todos los movimientos ordenados por usuario y fecha
            var allLogins = await _context.Logins
                .OrderBy(l => l.User_id)
                .ThenBy(l => l.fecha)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Login,NombreCompleto,Area,TotalHorasTrabajadas");

            foreach (var user in users)
            {
                var userLogs = allLogins
                    .Where(l => l.User_id == user.User_id)
                    .OrderBy(l => l.fecha)
                    .ToList();

                double totalHours = 0;
                DateTime? loginStart = null;

                foreach (var log in userLogs)
                {
                    if (log.TipoMov == 1) // login
                    {
                        // Solo abrir sesión si no hay una abierta
                        if (loginStart == null)
                        {
                            loginStart = log.fecha;
                        }
                    }
                    else if (log.TipoMov == 0) // logout
                    {
                        // Solo cerrar si sí había login abierto
                        if (loginStart != null)
                        {
                            var duration = log.fecha - loginStart.Value;

                            if (duration.TotalHours > 0)
                            {
                                totalHours += duration.TotalHours;
                            }

                            loginStart = null;
                        }
                    }
                }

                var area = areas.FirstOrDefault(a => a.IDArea == user.IDArea);

                var nombreCompleto = $"{user.Nombres ?? ""} {user.ApellidoPaterno ?? ""} {user.ApellidoMaterno ?? ""}".Trim();

                // Escapar comas o comillas por si acaso
                string loginEscaped = EscapeCsv(user.Login);
                string nombreEscaped = EscapeCsv(nombreCompleto);
                string areaEscaped = EscapeCsv(area?.AreaName ?? "Sin área");

                csv.AppendLine($"{loginEscaped},{nombreEscaped},{areaEscaped},{totalHours:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, "text/csv", "reporte_horas_trabajadas.csv");
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}