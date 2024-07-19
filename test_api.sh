#!/bin/bash

BASE_URL="http://localhost:5045/api"
USER_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMjM0NTY3ODkiLCJyb2xlIjoiVXNlciIsIm5iZiI6MTcyMTM1NzUzOCwiZXhwIjoxNzIxMzYxMTM4LCJpYXQiOjE3MjEzNTc1MzgsImlzcyI6Ik1va2tpbGljb3Jlc0V4cHJlc3NBUEkiLCJhdWQiOiJNb2traWxpY29yZXNFeHByZXNzQVBJVXNlcnMifQ.60Et1HVdT2KxbYOtTsEaHBEQQw9WzDiRR_5V9AETEL0"
ADMIN_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhZG1pbiIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTcyMTM1NzUxMSwiZXhwIjoxNzIxMzYxMTExLCJpYXQiOjE3MjEzNTc1MTEsImlzcyI6Ik1va2tpbGljb3Jlc0V4cHJlc3NBUEkiLCJhdWQiOiJNb2traWxpY29yZXNFeHByZXNzQVBJVXNlcnMifQ.-S0j2Jv-DZlNhR-GZdm557RUit-m4dPLhz4iYsC2UU0"
OUTPUT_FILE="api_test_results_$(date +%Y%m%d_%H%M%S).log"

# Función para imprimir separadores

print_separator() {
echo "----------------------------------------" | tee -a "$OUTPUT_FILE"
}

# Función para ejecutar y registrar una prueba

run_test() {
local test_name="$1"
local curl_command="$2"

```
print_separator
echo "Testing $test_name" | tee -a "$OUTPUT_FILE"
echo "Command: $curl_command" >> "$OUTPUT_FILE"
echo "Response:" | tee -a "$OUTPUT_FILE"
eval "$curl_command" | tee -a "$OUTPUT_FILE"
echo "" | tee -a "$OUTPUT_FILE"

```

}

# Iniciar el archivo de log

echo "API Test Results - $(date)" > "$OUTPUT_FILE"
echo "Base URL: $BASE_URL" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"

# Test Login

run_test "Login" "curl -X POST \"${BASE_URL}/account/login\" -H \"Content-Type: application/json\" -d '{\"identificacion\":\"admin\",\"password\":\"admin\"}'"

# Test GET Clientes

run_test "GET Clientes" "curl -X GET \"${BASE_URL}/cliente\" -H \"Authorization: Bearer \$ADMIN_TOKEN\""

# Test POST Cliente

run_test "POST Cliente" "curl -X POST \"${BASE_URL}/cliente\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$ADMIN_TOKEN\" -d '{\"identificacion\":\"123456789\",\"nombre\":\"Juan\",\"apellido\":\"Perez\",\"provincia\":\"San Jose\",\"canton\":\"Escazu\",\"distrito\":\"Escazu\"}'"

# Test PUT Cliente

run_test "PUT Cliente" "curl -X PUT \"${BASE_URL}/cliente/1\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"nombre\":\"Juan Actualizado\",\"apellido\":\"Perez\",\"provincia\":\"San Jose\",\"canton\":\"Escazu\",\"distrito\":\"Escazu\"}'"

# Test DELETE Cliente

run_test "DELETE Cliente" "curl -X DELETE \"${BASE_URL}/cliente/2\" -H \"Authorization: Bearer \$ADMIN_TOKEN\""

# Test GET Direcciones

run_test "GET Direcciones" "curl -X GET \"${BASE_URL}/direccion\" -H \"Authorization: Bearer \$USER_TOKEN\""

# Test POST Direccion

run_test "POST Direccion" "curl -X POST \"${BASE_URL}/direccion\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"clienteId\":1,\"provincia\":\"Alajuela\",\"canton\":\"Alajuela\",\"distrito\":\"Alajuela\",\"puntoEnWaze\":\"waze://?ll=10.0162,-84.2138\",\"esCondominio\":false,\"esPrincipal\":false}'"

# Test PUT Direccion

run_test "PUT Direccion" "curl -X PUT \"${BASE_URL}/direccion/1\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"clienteId\":1,\"provincia\":\"Alajuela\",\"canton\":\"Alajuela\",\"distrito\":\"San Rafael\",\"puntoEnWaze\":\"waze://?ll=10.0162,-84.2138\",\"esCondominio\":true,\"esPrincipal\":true}'"

# Test DELETE Direccion

run_test "DELETE Direccion" "curl -X DELETE \"${BASE_URL}/direccion/2\" -H \"Authorization: Bearer \$USER_TOKEN\""

# Test GET Inventarios

run_test "GET Inventarios" "curl -X GET \"${BASE_URL}/inventario\" -H \"Authorization: Bearer \$USER_TOKEN\""

# Test POST Inventario

run_test "POST Inventario" "curl -X POST \"${BASE_URL}/inventario\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"cantidadEnExistencia\":50,\"bodegaId\":1,\"fechaIngreso\":\"2024-07-18T00:00:00\",\"fechaVencimiento\":\"2025-07-18T00:00:00\",\"tipoLicor\":\"Ron\"}'"

# Test PUT Inventario

run_test "PUT Inventario" "curl -X PUT \"${BASE_URL}/inventario/1\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"cantidadEnExistencia\":75,\"bodegaId\":1,\"fechaIngreso\":\"2024-07-18T00:00:00\",\"fechaVencimiento\":\"2025-07-18T00:00:00\",\"tipoLicor\":\"Ron Premium\"}'"

# Test DELETE Inventario

run_test "DELETE Inventario" "curl -X DELETE \"${BASE_URL}/inventario/2\" -H \"Authorization: Bearer \$ADMIN_TOKEN\""

# Test GET Pedidos

run_test "GET Pedidos" "curl -X GET \"${BASE_URL}/pedido\" -H \"Authorization: Bearer \$USER_TOKEN\""

# Test POST Pedido

run_test "POST Pedido" "curl -X POST \"${BASE_URL}/pedido\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"clienteId\":1,\"inventarioId\":1,\"direccionId\":1,\"cantidad\":2,\"costoSinIVA\":100,\"estado\":\"Pendiente\"}'"

# Test PUT Pedido

run_test "PUT Pedido" "curl -X PUT \"${BASE_URL}/pedido/1\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer \$USER_TOKEN\" -d '{\"clienteId\":1,\"inventarioId\":1,\"direccionId\":1,\"cantidad\":3,\"costoSinIVA\":150,\"estado\":\"En Proceso\"}'"

# Test DELETE Pedido

run_test "DELETE Pedido" "curl -X DELETE \"${BASE_URL}/pedido/2\" -H \"Authorization: Bearer \$USER_TOKEN\""

print_separator
echo "All tests completed. Results saved in $OUTPUT_FILE" | tee -a "$OUTPUT_FILE"

# Añadir información para troubleshooting al final del archivo

echo "" >> "$OUTPUT_FILE"
echo "Troubleshooting Information for Claude AI:" >> "$OUTPUT_FILE"
echo "1. If you see 'curl: (7) Failed to connect', check if the API is running and accessible." >> "$OUTPUT_FILE"
echo "2. 401 Unauthorized errors may indicate invalid or expired tokens." >> "$OUTPUT_FILE"
echo "3. 404 Not Found errors could mean the endpoint doesn't exist or the resource wasn't found." >> "$OUTPUT_FILE"
echo "4. 400 Bad Request errors often indicate invalid input data." >> "$OUTPUT_FILE"
echo "5. 500 Internal Server Error suggests a problem on the server side." >> "$OUTPUT_FILE"
echo "6. Check that the USER_TOKEN and ADMIN_TOKEN are correctly set and not expired." >> "$OUTPUT_FILE"
echo "7. Ensure that the BASE_URL is correct and the API is running on that address." >> "$OUTPUT_FILE"
echo "8. For detailed error messages, check the API logs on the server side." >> "$OUTPUT_FILE"