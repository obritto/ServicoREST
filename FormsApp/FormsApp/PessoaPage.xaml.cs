using ClientPCL.ServicoREST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FormsApp
{
    public partial class PessoaPage : ContentPage
    {
        public PessoaPage()
        {
            InitializeComponent();
        }

        public PessoaPage(Pessoas p)
        {
            InitializeComponent();

            this.BindingContext = p;
        }

        public async void ProntoAction(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}
