using ClientPCL.ServicoREST.Models;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace FormsApp
{
    public class App : Application
    {

        ClientPCL.Default.Container container = null;

        ObservableCollection<Pessoas> lista = new ObservableCollection<Pessoas>();

        public ObservableCollection<Pessoas> Lista
        {
            get { return lista; }
            set { lista = value; }
        }


        ListView listViewPessoas = null;
        Button btnBuscar = null;
        Entry txtBusca = null;

       

        public App()
        {
            container = new ClientPCL.Default.Container(
                new Uri("http://192.168.0.28:81/odata/"));


            listViewPessoas = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(TextCell))
                {
                    Bindings = {
                     {
                            TextCell.TextProperty, new Binding ("Nome") },
                       {
                            TextCell.DetailProperty, new Binding ("Telefone") }
                        }
                }
            };


            listViewPessoas.ItemSelected += ListViewPessoas_ItemSelected;
            listViewPessoas.ItemsSource = Lista;

            btnBuscar = new Button
            {
                Text = "Buscar"
            };
            btnBuscar.Clicked += BtnBuscar_Clicked;

            txtBusca = new Entry
            {
                Placeholder = "Buscar..."
            };


            var conteudo = new ContentPage
            {

                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        txtBusca, btnBuscar, listViewPessoas

                    }
                }
            };

            // toolbar
            ToolbarItem tbSalvar = new ToolbarItem();
            tbSalvar.Text = "SALVAR";
            tbSalvar.Clicked += TbSalvar_Clicked;

            ToolbarItem tbIncluir = new ToolbarItem();
            tbIncluir.Text = "INCLUIR";
            tbIncluir.Clicked += TbIncluir_Clicked;

            ToolbarItem tbRemover = new ToolbarItem();
            tbRemover.Text = "REMOVER";
            tbRemover.Clicked += TbRemover_Clicked;

            conteudo.ToolbarItems.Add(tbSalvar);
            conteudo.ToolbarItems.Add(tbIncluir);
            conteudo.ToolbarItems.Add(tbRemover);


            // The root page of your application
            MainPage = new NavigationPage(conteudo);
        }


        public async void Buscar()
        {
            try
            {
                string busca = txtBusca.Text ?? "";
                // monta query
                IQueryable<Pessoas> query = container.PessoasOData.
                    Where(it => it.Nome.Contains(busca)).OrderBy(IT => IT.Nome);

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

        Pessoas current = null;

        private void ListViewPessoas_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listViewPessoas.SelectedItem != null)
            {
                current = listViewPessoas.SelectedItem as Pessoas;

                current.PropertyChanged += Current_PropertyChanged;

                this.MainPage.Navigation.PushModalAsync(new PessoaPage(current));
            }
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var entidade = container.Entities.FirstOrDefault(it => it.Entity == current);

            if (entidade != null && entidade.State == EntityStates.Unchanged)
            {
                container.ChangeState(current, EntityStates.Modified);
            }
        }

        private void BtnBuscar_Clicked(object sender, EventArgs e)
        {
            Buscar();
        }

        private void TbRemover_Clicked(object sender, EventArgs e)
        {
            if (listViewPessoas.SelectedItem != null)
            {
                container.DeleteObject(listViewPessoas.SelectedItem as Pessoas);
                lista.Remove(listViewPessoas.SelectedItem as Pessoas);
            }
        }

        private void TbIncluir_Clicked(object sender, EventArgs e)
        {
            var novo = new Pessoas();

            container.AddToPessoasOData(novo);

            Lista.Add(novo);
            listViewPessoas.SelectedItem = novo;
        }

        private void TbSalvar_Clicked(object sender, EventArgs e)
        {
            try
            {
                container.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
