using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using ReadNote.Datos;
using ReadNote.Tablas;
using SQLite;

namespace ReadNote.Vistas.Material.Actual
{
    public partial class InicioMatActVista : ContentPage
    {
        SQLiteAsyncConnection conexion;
        public ObservableCollection<T_Material> TablaMaterialActual { get; set; }
        private T_Material materialSeleccionado;

        public InicioMatActVista()
        {
            InitializeComponent();
            conexion = DependencyService.Get<ISQLiteDB>().GetConnection();

            // Inicializar la colección
            TablaMaterialActual = new ObservableCollection<T_Material>();

            // Cargar los datos iniciales de la base de datos
            CargarDatos();

            // Enlazar la colección con el BindingContext
            BindingContext = this;
        }

        private async void CargarDatos()
        {
            // Obtener datos de SQLite y agregarlos a la ObservableCollection
            var listaMateriales = await conexion.Table<T_Material>().Where(m => m.estado_material == false).ToListAsync();

            // Limpiar la colección para evitar que los datos anteriores se dupliquen
            TablaMaterialActual.Clear();

            // Agregar cada material a la ObservableCollection
            foreach (var material in listaMateriales)
            {
                TablaMaterialActual.Add(material);
            }
        }

        private void OnNameTapped(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var material = button.CommandParameter as T_Material;
            if (material == null) return;

            // Asignar el material seleccionado
            materialSeleccionado = material;

            // Mostrar el popup
            popupContainer.IsVisible = true;
            popupMenu.IsVisible = true;
        }

        private void OnPopupBackgroundTapped(object sender, EventArgs e)
        {
            popupContainer.IsVisible = false;
            popupMenu.IsVisible = false;
        }

        public async Task EliminarRegistro(T_Material material)
        {
            // Eliminar el registro de la base de datos
            await conexion.DeleteAsync(material);

            // Eliminar el registro de la ObservableCollection
            TablaMaterialActual.Remove(material);
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar este material?", "Sí", "No");
            if (confirm && materialSeleccionado != null)
            {
                await EliminarRegistro(materialSeleccionado);
                popupContainer.IsVisible = false;
                popupMenu.IsVisible = false;
            }
        }

        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            popupContainer.IsVisible = false;
            popupMenu.IsVisible = false;

            if (materialSeleccionado != null)
            {
                await Navigation.PushAsync(new ActualizarMatFutVista(
                    materialSeleccionado.id_Material,
                    materialSeleccionado.nombre_material,
                    materialSeleccionado.autor_material,
                    materialSeleccionado.descripcion_material,
                    materialSeleccionado.categoria_material,
                    materialSeleccionado.nopag_material.ToString(),
                    materialSeleccionado.estado_material,
                    materialSeleccionado.nivel_lector,
                    materialSeleccionado.fecha_creacion,
                    materialSeleccionado.tipo_material));
            }
        }

        private async void OnVolverTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Vistas.Material.IndexMaterialVista());
        }
    }
}