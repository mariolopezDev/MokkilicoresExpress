using MokkilicoresExpress.Models;

namespace MokkilicoresExpress.Services
{
    public class InventarioService
    {
        private List<Inventario> _inventario;

        public InventarioService()
        {
            _inventario = new List<Inventario>
            {
                new Inventario { Id = 1, TipoLicor = "Vodka", CantidadEnExistencia = 100, BodegaId = 1, FechaIngreso = DateTime.Now, FechaVencimiento = DateTime.Now.AddYears(1) },
                new Inventario { Id = 2, TipoLicor = "Whiskey", CantidadEnExistencia = 150, BodegaId = 2, FechaIngreso = DateTime.Now, FechaVencimiento = DateTime.Now.AddYears(1) }
            };
        }

        public List<Inventario> GetAll()
        {
            return _inventario;
        }

        public Inventario GetById(int id)
        {
            return _inventario.FirstOrDefault(i => i.Id == id);
        }

        public void Add(Inventario inventario)
        {
            if (_inventario.Any())
            {
                inventario.Id = _inventario.Max(i => i.Id) + 1;
            }
            else
            {
                inventario.Id = 1; // Asegurar un ID inicial si la lista está vacía
            }
            _inventario.Add(inventario);
        }

        public bool Update(int id, Inventario updatedInventario)
        {
            var inventario = _inventario.FirstOrDefault(i => i.Id == id);
            if (inventario != null)
            {
                inventario.TipoLicor = updatedInventario.TipoLicor;
                inventario.CantidadEnExistencia = updatedInventario.CantidadEnExistencia;
                inventario.BodegaId = updatedInventario.BodegaId;
                inventario.FechaIngreso = updatedInventario.FechaIngreso;
                inventario.FechaVencimiento = updatedInventario.FechaVencimiento;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var inventario = _inventario.FirstOrDefault(i => i.Id == id);
            if (inventario != null)
            {
                _inventario.Remove(inventario);
                return true;
            }
            return false;
        }

        public List<Inventario> SearchInventory(string searchTerm)
                {
                    return string.IsNullOrWhiteSpace(searchTerm) ?
                        _inventario :
                        _inventario.Where(i => i.TipoLicor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }
    }
}
