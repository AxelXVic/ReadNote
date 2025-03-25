using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReadNote.Datos;
using ReadNote.Tablas;
using System.IO;
using System.Collections.ObjectModel;
using ReadNote;
using SQLite;
using ReadNote.Vistas.Material.Futuro;

namespace ReadNote.Vistas.Material.Futuro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioMatFutVista : ContentPage
    {
        SQLiteAsyncConnection conexion;
        public ObservableCollection<T_Material> TablaMaterialFuturo { get; set; }
        private T_Material materialSeleccionado;

        public InicioMatFutVista()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            conexion = DependencyService.Get<ISQLiteDB>().GetConnection();

            // Inicializar la colección
            TablaMaterialFuturo = new ObservableCollection<T_Material>();

            // Cargar los datos iniciales de la base de datos
            CargarDatos();

            // Enlazar la colección con el BindingContext
            BindingContext = this;
        }

        private async void CargarDatos()
        {
            var listaMateriales = await conexion.Table<T_Material>().Where(m => m.estado_material == true).ToListAsync();
            TablaMaterialFuturo.Clear();
            foreach (var material in listaMateriales)
            {
                TablaMaterialFuturo.Add(material);
            }
        }

        public async Task AgregarRegistro(T_Material nuevoMaterial)
        {
            await conexion.InsertAsync(nuevoMaterial);
            TablaMaterialFuturo.Add(nuevoMaterial);
        }

        // Mostrar el popup
        private void OnNameTapped(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var material = button.CommandParameter as T_Material;
            if (material == null) return;

            materialSeleccionado = material;

            popupContainer.IsVisible = true;
            popupMenu.IsVisible = true;
        }

        // Ocultar el popup
        public void HidePopup()
        {
            popupContainer.IsVisible = false;
            popupMenu.IsVisible = false;
        }

        private void OnPopupBackgroundTapped(object sender, EventArgs e)
        {
            HidePopup();
        }

        public async Task EliminarRegistro(T_Material material)
        {
            await conexion.DeleteAsync(material);
            TablaMaterialFuturo.Remove(material);
        }

        // Eliminar Material
        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar este material?", "Sí", "No");
            if (confirm && materialSeleccionado != null)
            {
                await EliminarRegistro(materialSeleccionado);
                HidePopup();
            }
        }

        // Actualizar Material
        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            HidePopup();
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