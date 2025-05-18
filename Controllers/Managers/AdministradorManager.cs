using FutbolitoManager.Data;
using FutbolitoManager.Models;

namespace FutbolitoManager.Services
{
    public class AdministradorManager
    {
        private readonly AppDbContext _context;

        public AdministradorManager(AppDbContext context)
        {
            _context = context;
        }

        // Método para registrar un nuevo administrador (solo para pruebas o futuro)
        public void RegistrarAdministrador(string correo, string contraseña)
        {
            string salt = SecurityHelper.GenerateSalt();
            string hash = SecurityHelper.HashPassword(contraseña, salt);

            var admin = new Administrador
            {
                Email = correo,
                Password = hash,
                Salt = salt
            };

            _context.Administradores.Add(admin);
            _context.SaveChanges();
        }

        // Método para verificar si el login es correcto
        public bool VerificarLogin(string correo, string contraseña)
        {
            var admin = _context.Administradores.FirstOrDefault(a => a.Email == correo);
            if (admin == null) return false;

            return SecurityHelper.VerifyPassword(contraseña, admin.Password, admin.Salt);
        }
    }
}
