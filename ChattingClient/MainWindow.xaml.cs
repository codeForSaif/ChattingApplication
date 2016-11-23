using ChattingInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChattingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IChattingService Server;
        private static DuplexChannelFactory<IChattingService> _channelFactory;
        public MainWindow()
        {
            InitializeComponent();
            _channelFactory = new DuplexChannelFactory<IChattingService>(new ClientCallback(),"ChattingServiceEndpoint");
            Server = _channelFactory.CreateChannel();
        }

       public void TakeMessage(string message, string userName)
        {

            TextBox.Text += userName + ": " + message + "\n";
            TextBox.ScrollToEnd();

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageTextBox.Text.Length==0)
            {
                return;
            }
            Server.sendMessageToAll(MessageTextBox.Text, UserNameTextBox.Text);
            TakeMessage(MessageTextBox.Text, "You");
            MessageTextBox.Text = "";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            int returnValue = Server.Login(UserNameTextBox.Text);
            if (returnValue == 1)
            {
                MessageBox.Show("You are already logged in ! Try again");
            }
            else if (returnValue == 0)
            {

                UserNameTextBox.IsEnabled = false;
                LoginButton.IsEnabled = false;
                label.Content = "Welcome " + UserNameTextBox.Text;

                LoadUserList(Server.GetCurrentUsers());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Logout();    
        }
        public void AddUserToList(string userName)
        {
            if(UsersListBox.Items.Contains(userName))
            {
                return;
            }
            UsersListBox.Items.Add(userName); 
        }
        public void RemoveUserFromList(string userName)
        {
            if(UsersListBox.Items.Contains(userName))
            {
                UsersListBox.Items.Remove(userName);
            }
        }
        private void LoadUserList(List<string> users)
        {
            foreach(var user in users)
            {
                AddUserToList(user);
            }
        }
    }
}
