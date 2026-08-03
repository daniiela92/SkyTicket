using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for PassengerEditorWindow.xaml
    /// </summary>
    public partial class PassengerEditorWindow : Window
    {
        private Passenger _passengerToEdit;
        private ApiService _apiService;

        public PassengerEditorWindow(Passenger passenger = null)
        {
            InitializeComponent();

            _apiService = new ApiService();
            _passengerToEdit = passenger;

            if (_passengerToEdit == null)
            {
                Title = "Criar Passageiro";
                LabelTitle.Text = "Criar Passageiro";
            }
            else
            {
                Title = "Editar Passageiro";
                LabelTitle.Text = "Editar Passageiro";

                TxtFirstName.Text = _passengerToEdit.FirstName;
                TxtLastName.Text = _passengerToEdit.LastName;
                TxtPhone.Text = _passengerToEdit.Phone;
                TxtEmail.Text = _passengerToEdit.Email;
                TxtPassport.Text = _passengerToEdit.Passport;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtPhone.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtPassport.Text))
            {
                LabelStatus.Text = "Todos os campos são obrigatórios.";
                return;
            }

            if (!TxtEmail.Text.Contains("@"))
            {
                LabelStatus.Text = "O email inserido tem de ser válido.";
                return;
            }

            var passenger = new Passenger
            {
                FirstName = TxtFirstName.Text,
                LastName = TxtLastName.Text,
                Phone = TxtPhone.Text,
                Email = TxtEmail.Text,
                Passport = TxtPassport.Text
            };

            Response response;

            if (_passengerToEdit == null)
            {
                response = await _apiService.PostPassenger("http://www.skyticketproject.somee.com/", "api/passengers", passenger);
            }
            else
            {
                passenger.Id = _passengerToEdit.Id;
                response = await _apiService.PutPassenger("http://www.skyticketproject.somee.com/", "api/passengers", _passengerToEdit.Id, passenger);
            }

            if (!response.IsSucess)
            {
                if (!string.IsNullOrWhiteSpace(response.Message))
                {
                    LabelStatus.Text = "Erro: " + response.Message;
                }
                else
                {
                    LabelStatus.Text = "Não foi possível guardar. Já existe um passageiro com este número de passaporte.";
                }
                return;
            }

            this.Close();
        }
    }
}


            

           
    

