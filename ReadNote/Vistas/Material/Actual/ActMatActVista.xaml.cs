using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReadNote.Datos;
using ReadNote.Tablas;
using SQLite;

namespace ReadNote.Vistas.Material.Actual
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActMatActVista : ContentPage
    {
        private SQLiteAsyncConnection conexion;
        private int idMaterial;
        private T_Material materialActual;

        public ActMatActVista(int id, string nombre, string autor, string descripcion, string categoria, string numPaginas, bool estado, string nivelLector, DateTime fechaCreacion, string tipoMaterial)
        {
            InitializeComponent();
            conexion = DependencyService.Get<ISQLiteDB>().GetConnection();
            idMaterial = id;

            // Llenar los campos con los datos actuales
            entryNombre.Text = nombre;
            entryAutor.Text = autor;
            entryDescripcion.Text = descripcion;
            entryCategoria.Text = categoria;
            entryNumPaginas.Text = numPaginas;
            switchEstado.IsToggled = estado;
            entryNivelLector.Text = nivelLector;
            datePickerFechaCreacion.Date = fechaCreacion;
            entryTipoMaterial.Text = tipoMaterial;
        }

        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            // Verificar si el material existe en la base de datos
            materialActual = await conexion.Table<T_Material>().Where(m => m.id_Material == idMaterial).FirstOrDefaultAsync();

            if (materialActual != null)
            {
                materialActual.nombre_material = entryNombre.Text;
                materialActual.autor_material = entryAutor.Text;
                materialActual.descripcion_material = entryDescripcion.Text;
                materialActual.categoria_material = entryCategoria.Text;
                materialActual.nopag_material = int.Parse(entryNumPaginas.Text);
                materialActual.estado_material = switchEstado.IsToggled;
                materialActual.nivel_lector = entryNivelLector.Text;
                materialActual.fecha_creacion = datePickerFechaCreacion.Date;
                materialActual.tipo_material = entryTipoMaterial.Text;

                await conexion.UpdateAsync(materialActual);

                await DisplayAlert("Éxito", "Material actualizado correctamente", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se encontró el material", "OK");
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}