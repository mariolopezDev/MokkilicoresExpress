using MokkilicoresExpress.Models;

namespace MokkilicoresExpress.Services
{
    public class ClienteService
    {
        private List<Cliente> _clientes;

        public ClienteService()
        {
            _clientes = new List<Cliente>
            {
                // Inicializa con algunos clientes de prueba
                new Cliente { Identificacion = "101", Nombre = "Juan", Apellido = "Pérez", Provincia = "San José", Canton = "Central", Distrito = "Carmen", DineroCompradoTotal = 5000, DineroCompradoUltimoAnio = 3000, DineroCompradoUltimosSeisMeses = 1500 },
                new Cliente { Identificacion = "102", Nombre = "María", Apellido = "Martínez", Provincia = "Heredia", Canton = "Heredia", Distrito = "Ulloa", DineroCompradoTotal = 7500, DineroCompradoUltimoAnio = 4500, DineroCompradoUltimosSeisMeses = 2000 }
            };
        }

        public List<Cliente> GetAll()
        {
            return _clientes;
        }

        public Cliente GetById(string id)
        {
            return _clientes.FirstOrDefault(c => c.Identificacion == id);
        }

        public void Add(Cliente cliente)
        {
            if (!_clientes.Any(c => c.Identificacion == cliente.Identificacion))
            {
                _clientes.Add(cliente);
            }
            else
            {
                throw new Exception("Un cliente con la misma identificación ya existe.");
            }
        }

        public bool Update(Cliente cliente)
        {
            var existingCliente = _clientes.FirstOrDefault(c => c.Identificacion == cliente.Identificacion);
            if (existingCliente != null)
            {
                existingCliente.Nombre = cliente.Nombre;
                existingCliente.Apellido = cliente.Apellido;
                existingCliente.Provincia = cliente.Provincia;
                existingCliente.Canton = cliente.Canton;
                existingCliente.Distrito = cliente.Distrito;
                existingCliente.DineroCompradoTotal = cliente.DineroCompradoTotal;
                existingCliente.DineroCompradoUltimoAnio = cliente.DineroCompradoUltimoAnio;
                existingCliente.DineroCompradoUltimosSeisMeses = cliente.DineroCompradoUltimosSeisMeses;
                return true;
            }
            return false;
        }

        public bool Delete(string id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Identificacion == id);
            if (cliente != null)
            {
                _clientes.Remove(cliente);
                return true;
            }
            return false;
        }

        public List<Cliente> Search(string searchTerm)
        {
            var clientes = GetAll(); // Asume que existe un método GetAll() que retorna todos los clientes
            return string.IsNullOrWhiteSpace(searchTerm) ? 
                clientes : 
                clientes.Where(c => c.Nombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    c.Apellido.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    c.Identificacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

    }
}
