﻿namespace FutbolitoManager.Models
{
    public class Administrador
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; } = string.Empty;

        public string Rol { get; set; }

    }
}
