using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Collections;

namespace Juego_Formulario.Formularios
{
    internal class CRUD_SQL
    {
        string connectionString = string.Empty; //"Server=localhost;Database=juegos_datos;Uid=root;Pwd=852456;";
        string nombreTabla = string.Empty;

        private DataTable tablaSQL = new DataTable();
        private DataTable imgSQL = new DataTable();

        public DataTable TablaSQL => tablaSQL;
        //public DataTable ImgSQL => imgSQL;

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Image ByteToImage(byte[] byteimg)
        {
            using (var ms = new MemoryStream(byteimg))
            {
                return Image.FromStream(ms);
            }
        }

        public CRUD_SQL(string BaseDeDatos, string Tabla)
        {
            connectionString = $"Server=localhost;Database={BaseDeDatos};Uid=root;Pwd=852456;";
            imgSQL.Columns.Add("ID", typeof(int));
            imgSQL.Columns.Add("Imagen", typeof(byte[]));
            imgSQL.PrimaryKey = new DataColumn[] { imgSQL.Columns["ID"] };
            nombreTabla = Tabla;
        }

        //Crear
        public void CreateSQL(DataRow valor)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                //Crea el query dinamico
                StringBuilder nombresColumnas = new StringBuilder();
                StringBuilder variablesNombresColumnas = new StringBuilder();
                for (int i = 0; i < tablaSQL.Columns.Count; i++)
                {
                    nombresColumnas.Append($"{tablaSQL.Columns[i].ColumnName}");
                    variablesNombresColumnas.Append($"@{tablaSQL.Columns[i].ColumnName}");
                    if (i < (tablaSQL.Columns.Count - 1))
                    {
                        nombresColumnas.Append(", ");
                        variablesNombresColumnas.Append(", ");
                    }
                }
                string query = $"INSERT INTO {nombreTabla} ({nombresColumnas}) VALUES ({variablesNombresColumnas});";
                MySqlCommand command = new MySqlCommand(query, connection);

                //Inserta los valores de la nueva fila
                for (int i = 0; i < tablaSQL.Columns.Count; i++)
                {
                    string parameterName = $"@{tablaSQL.Columns[i].ColumnName}";
                    object value = valor[i];
                    command.Parameters.AddWithValue(parameterName, value);
                }

                //Se ejectuta y envia a MYSQL
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Datos insertados correctamente en la base de datos.");
                    ReadSQL();
                }
                else
                {
                    MessageBox.Show("No se pudo insertar los datos en la base de datos.");
                }
            }
        }

        public DataTable ReadSQL()
        {
            tablaSQL.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = $"SELECT * FROM {nombreTabla};";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(tablaSQL);

                //string colImg = string.Empty;
                //for (int i = 0; i < tablaSQL.Columns.Count; i++)
                //{
                //    if (tablaSQL.Columns[i].DataType == typeof(byte[]))
                //        colImg = tablaSQL.Columns[i].ColumnName;
                //}

                //foreach (DataRow row in tablaSQL.Rows)
                //{
                //    DataRow newRow = imgSQL.NewRow();
                //    newRow["Imagen"] = row[colImg];
                //    newRow["ID"] = row[0];
                //    imgSQL.Rows.Add(newRow);
                //}
                //tablaSQL.Columns.Remove(colImg);
            }
            return tablaSQL;
        }

        //Actualizar
        public void UpdateSQL(int id, DataRow valor)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                //Crea el query dinamico
                string where = $"{tablaSQL.Columns[0].ColumnName} = @{tablaSQL.Columns[0].ColumnName};";
                StringBuilder nombresColumnas = new StringBuilder();
                for (int i = 0; i < tablaSQL.Columns.Count; i++)
                {
                    nombresColumnas.Append($"{tablaSQL.Columns[i].ColumnName} = @{tablaSQL.Columns[i].ColumnName}");
                    if (i < (tablaSQL.Columns.Count - 1))
                        nombresColumnas.Append(", ");
                }
                string query = $"UPDATE juego SET {nombresColumnas} WHERE {where}";

                //Inserta los valores modificados de la fila
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue($"@{tablaSQL.Columns[0].ColumnName}", id);
                foreach (DataColumn column in tablaSQL.Columns)
                {
                    if (column.ColumnName != tablaSQL.Columns[0].ColumnName)
                        command.Parameters.AddWithValue($"@{column.ColumnName}", valor[column.ColumnName]);
                }
                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Registro modificado correctamente de la base de datos.");
                }
                else
                {
                    MessageBox.Show("No se pudo modificar el registro de la base de datos.");
                }
            }
        }

        //Eliminar
        public void DeleteSQL(int id) 
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string where = $"{tablaSQL.Columns[0].ColumnName} = @{tablaSQL.Columns[0].ColumnName};";
                string query = $"DELETE FROM {nombreTabla} WHERE {where};";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue($"@{tablaSQL.Columns[0].ColumnName}", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Registro eliminado correctamente de la base de datos.");
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el registro de la base de datos.");
                }
                ReadSQL();
            }
        }
    }
}
