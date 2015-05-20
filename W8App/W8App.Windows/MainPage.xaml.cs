using ClientPCL.ServicoREST.Models;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace W8App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ClientPCL.Default.Container container = null;

        ObservableCollection<Pessoas> lista = new ObservableCollection<Pessoas>();

        public ObservableCollection<Pessoas> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        public MainPage()
        {
            InitializeComponent();

            listBoxPessoa.ItemsSource = Lista;


            container = new ClientPCL.Default.Container(
                new Uri("http://192.168.0.28:81/odata/"));
        }





        public async void Buscar()
        {
            try
            {
                // monta query
                IQueryable<Pessoas> query = container.PessoasOData.
                    Where(it => it.Nome.Contains(txtBusca.Text)).OrderBy(IT => IT.Nome);

                var executaQuery = await ((DataServiceQuery<Pessoas>)query.
                   Skip(0).Take(10)).
                   IncludeTotalCount().ExecuteAsync();

                // inclui na lista
                lista.Clear();

                foreach (var item in executaQuery)
                {
                    this.Lista.Add(item);
                }

                long tot = (executaQuery as QueryOperationResponse<Pessoas>).TotalCount;
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.ToString());
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                container.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var novo = new Pessoas();

            container.AddToPessoasOData(novo);

            Lista.Add(novo);
            listBoxPessoa.SelectedItem = novo;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (listBoxPessoa.SelectedItem != null)
            {
                container.DeleteObject(listBoxPessoa.SelectedItem as Pessoas);
                lista.Remove(listBoxPessoa.SelectedItem as Pessoas);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Buscar();
        }

        Pessoas current = null;

        private void listBoxPessoa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxPessoa.SelectedItem != null)
            {
                current = listBoxPessoa.SelectedItem as Pessoas;

                current.PropertyChanged += current_PropertyChanged;
            }

        }

        void current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entidade = container.Entities.FirstOrDefault(it => it.Entity == current);

            if (entidade != null && entidade.State == EntityStates.Unchanged)
            {
                container.ChangeState(current, EntityStates.Modified);
            }

        }
    }
}
