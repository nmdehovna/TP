using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Data.OleDb;
using System.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace Empleados
{
    public partial class MainWindow : Window
    {

        int _noOfErrorsOnScreen = 0;
        private Empleado _empleado = new Empleado();
        OleDbConnection conn;
        DataTable dt;
        bool gv_save;

        public MainWindow()
        {
            InitializeComponent();

            //Clase que valida parámetros de entrada
            TablaEmpleados.DataContext = _empleado;
            
            //Conecto la base de datos
            conn = new OleDbConnection();
            conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\Empleados.mdb";
            //Muestro tabla
            Tabla();
            Borrar();

        }

        //Muestro la base de datos en el tabla
        private void Tabla()
        {
            OleDbCommand cmd = new OleDbCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;

            if (TxtBuscar.Text != "")
            {
                cmd.CommandText = "select * from tbl_emple where " + TxtSelCampo.Text + " like '%" + TxtBuscar.Text + "%' order by Id";
            }
            else
                cmd.CommandText = "select * from tbl_emple order by Id";

            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            GNRLData.ItemsSource = dt.AsDataView();


            if (dt.Rows.Count > 0)
            {
                lbl_grid.Visibility = Visibility.Hidden;
                GNRLData.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_grid.Visibility = Visibility.Visible;
                GNRLData.Visibility = Visibility.Hidden;
            }

        }

        //Agrego registro a la tabla y a la base de datos
        private void Boton_Agregar_Click(object sender, RoutedEventArgs e)
        {
            OleDbCommand cmd = new OleDbCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;

            if (TxtNombre.Text != "" && TxtApellido.Text != "" && TxtFechaNacimiento.Text != "" &&
                TxtDNI.Text != "" && TxtDomicilio.Text != "")
            {
                if (gv_save == false)
                {
                    if (TxtGenero.Text != " ")
                    {
                        cmd.CommandText = "INSERT INTO TBL_EMPLE(Nombre,Apellido,FechaNacimiento,Genero,DNI,Email,Domicilio) Values ('" + TxtNombre.Text + "','" + TxtApellido.Text + "','" + TxtFechaNacimiento.Text + "','" + TxtGenero.Text + "','" + TxtDNI.Text + "','" + TxtEmail.Text + "','" + TxtDomicilio.Text + "')";
                        cmd.ExecuteNonQuery();
                        Tabla();
                        Borrar();
                    }
                }
                else
                {
                    DataRowView row = (DataRowView)GNRLData.SelectedItems[0];
                    cmd.CommandText = "update TBL_EMPLE set Nombre='" + TxtNombre.Text + "',Apellido='" + TxtApellido.Text + "',FechaNacimiento='" + TxtFechaNacimiento.Text + "',Genero='" + TxtGenero.Text + "',DNI='" + TxtDNI.Text + "',Email='" + TxtEmail.Text + "',Domicilio='" + TxtDomicilio.Text + "' where Id=" + TxtID.Text;
                    cmd.ExecuteNonQuery();
                    gv_save = false;
                    Tabla();
                    Borrar();
                }
            }
        }

        //Calculo y seteo ID
        private void Obtener_ID()
        {
            int lv_countid;
            lv_countid = Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["Id"]) + 1;
            TxtID.Text = lv_countid.ToString();
        }

        //Limpio todos los campos
        private void Boton_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Borrar();
        }
        
        //Limpio campos de la pantalla
        private void Borrar()
        {
            TxtID.Text = "";
            TxtNombre.Text = "";
            TxtApellido.Text = "";
            TxtFechaNacimiento.Text = "";
            TxtGenero.SelectedIndex = 0;
            TxtDNI.Text = "";
            TxtEmail.Text = "";
            TxtDomicilio.Text = "";
            TxtID.IsEnabled = false;
        }

        //Actualizo registros de la tabla
        private void Boton_Editar_Click(object sender, RoutedEventArgs e)
        {
            if (GNRLData.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)GNRLData.SelectedItems[0];
                TxtID.Text = row["Id"].ToString();
                TxtNombre.Text = row["Nombre"].ToString();
                TxtApellido.Text = row["Apellido"].ToString();
                TxtFechaNacimiento.Text = row["FechaNacimiento"].ToString();
                TxtGenero.Text = row["Genero"].ToString();
                TxtDNI.Text = row["DNI"].ToString();
                TxtEmail.Text = row["Email"].ToString();
                TxtDomicilio.Text = row["Domicilio"].ToString();
                TxtID.IsEnabled = false;
                gv_save = true;
            }
        }

        //Borro un registro de la tabla de la base de datos
        private void Boton_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GNRLData.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)GNRLData.SelectedItems[0];

                OleDbCommand cmd = new OleDbCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "delete from TBL_EMPLE where Id=" + row["Id"].ToString();
                cmd.ExecuteNonQuery();
                Tabla();
                Borrar();
            }
        }
        
        //Busco el dato ingresado
        private void Boton_Buscar_Click(object sender, RoutedEventArgs e)
        {
            Tabla();
        }

        //Cierro la aplicacion
        private void Boton_Salir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Contador de errores
        private void Validar(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        //Habilito o deshabilito boton
        private void AddEmp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddEmp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Empleado cust = TablaEmpleados.DataContext as Empleado;
            _empleado = new Empleado();
            TablaEmpleados.DataContext = _empleado;
            e.Handled = true;
        }
    }
}