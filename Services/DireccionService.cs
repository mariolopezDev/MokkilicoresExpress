using MokkilicoresExpress.Models;

namespace MokkilicoresExpress.Services
{
    public class DireccionService
    {
        private List<Direccion> _direcciones;

        public DireccionService()
        {
            _direcciones = new List<Direccion>{
                new Direccion { Id = 1, Provincia = "San José", Canton = "San José", Distrito = "Carmen", PuntoEnWaze = "https://waze.com/ul?ll=9.9323,-84.0776", EsCondominio = false, ClienteId = 101 },
                new Direccion { Id = 2, Provincia = "Alajuela", Canton = "Alajuela", Distrito = "Alajuela", PuntoEnWaze = "https://waze.com/ul?ll=10.0169,-84.2147", EsCondominio = false, ClienteId = 101 },
                new Direccion { Id = 3, Provincia = "Cartago", Canton = "Cartago", Distrito = "Cartago", PuntoEnWaze = "https://waze.com/ul?ll=9.8655,-83.9207", EsCondominio = false, ClienteId = 102 },
                new Direccion { Id = 4, Provincia = "Heredia", Canton = "Heredia", Distrito = "Heredia", PuntoEnWaze = "https://waze.com/ul?ll=10.0029,-84.1177", EsCondominio = false, ClienteId = 102 },
                new Direccion { Id = 5, Provincia = "Guanacaste", Canton = "Liberia", Distrito = "Liberia", PuntoEnWaze = "https://waze.com/ul?ll=10.6351,-85.4376", EsCondominio = false, ClienteId = 103 },
                new Direccion { Id = 6, Provincia = "Puntarenas", Canton = "Puntarenas", Distrito = "Puntarenas", PuntoEnWaze = "https://waze.com/ul?ll=9.9778,-84.8384", EsCondominio = false, ClienteId = 103 },
                new Direccion { Id = 7, Provincia = "Limón", Canton = "Limón", Distrito = "Limón", PuntoEnWaze = "https://waze.com/ul?ll=9.9927,-83.0307", EsCondominio = false, ClienteId = 104 },
                new Direccion { Id = 8, Provincia = "San José", Canton = "San José", Distrito = "Carmen", PuntoEnWaze = "https://waze.com/ul?ll=9.9323,-84.0776", EsCondominio = false, ClienteId = 104 },
            };
        }

         public List<Direccion> GetAll()
        {
            return _direcciones;
        }

        public Direccion GetById(int id)
        {
            return _direcciones.FirstOrDefault(d => d.Id == id);
        }

        public void Add(Direccion direccion)
        {
            direccion.Id = _direcciones.Any() ? _direcciones.Max(d => d.Id) + 1 : 1;
            _direcciones.Add(direccion);
        }

        public bool Update(Direccion direccion)
        {
            var existingDireccion = _direcciones.FirstOrDefault(d => d.Id == direccion.Id);
            if (existingDireccion != null)
            {
                existingDireccion.Provincia = direccion.Provincia;
                existingDireccion.Canton = direccion.Canton;
                existingDireccion.Distrito = direccion.Distrito;
                existingDireccion.PuntoEnWaze = direccion.PuntoEnWaze;
                existingDireccion.EsCondominio = direccion.EsCondominio;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var direccion = _direcciones.FirstOrDefault(d => d.Id == id);
            if (direccion != null)
            {
                _direcciones.Remove(direccion);
                return true;
            }
            return false;
        }

        public List<Direccion> Search(string searchTerm)
        {
            return _direcciones.Where(d => d.Provincia.Contains(searchTerm) || d.Canton.Contains(searchTerm) || d.Distrito.Contains(searchTerm)).ToList();
        }
    }
}
