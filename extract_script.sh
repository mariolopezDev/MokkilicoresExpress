#!/bin/bash

# Archivo de salida
output="output.txt"

# Función para procesar directorios
process_directory() {
    echo "Directorio: $1" >> $output
    for file in "$1"/*; do
        if [ -d "$file" ]; then
            echo "  Subdirectorio: $(basename "$file")" >> $output
            process_directory "$file"
        else
            echo "  Archivo: $(basename "$file")" >> $output
            echo "    Contenido:" >> $output
            cat "$file" >> $output
            echo "" >> $output
        fi
    done
}

# Lista de directorios a procesar
directories=("Controllers" "Models" "Views")

# Procesar cada directorio
for dir in "${directories[@]}"; do
    if [ -d "$dir" ]; then
        process_directory "$dir"
    fi
done

# Procesar el archivo Program.cs si existe
if [ -f "Program.cs" ]; then
    echo "Archivo: Program.cs" >> $output
    echo "  Contenido:" >> $output
    cat "Program.cs" >> $output
    echo "" >> $output
fi

echo "Extracción completada."

