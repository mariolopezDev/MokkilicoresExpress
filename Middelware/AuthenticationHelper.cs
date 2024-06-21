using MokkilicoresExpress.Models;
using System.Collections.Generic;
using System.Linq;

namespace MokkilicoresExpress.Helpers
{
    public static class AuthenticationHelper
    {
        public static bool ValidateCredentials(string clientId, string password, List<Cliente> clients)
        {
            var client = clients.FirstOrDefault(c => c.Identificacion == clientId);
            if (client != null)
            {
                // Genera la contraseña esperada basada en la lógica especificada
                var expectedPassword = clientId + client.Nombre.Substring(0, 2) + char.ToUpper(client.Apellido[0]);
                return password == expectedPassword;
            }
            return false;
        }
    }
}
