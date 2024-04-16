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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : Window, View
    {
        private readonly Presenter _presenter;
        public Categories()
        {
            InitializeComponent();
            _presenter = new Presenter(this);

            PopulateCategories();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ConfirmApplicationClosure())
            {
                Application.Current.Shutdown();
            }
        }

        private void PopulateCategories()
        {
            foreach (Category.CategoryType item in Enum.GetValues(typeof(Category.CategoryType)))
            {
                Type.Items.Add(item);
            }
        }

        private void Add_Category(object sender, RoutedEventArgs e)
        {
            string descr = Description.Text;
            Category.CategoryType type = (Category.CategoryType)Type.SelectedItem;
            _presenter._calendar.categories.Add(descr, type);
            ShowMessage("Category added.");
        }


        public bool ConfirmCloseApplication()
        {
            throw new NotImplementedException();
        }

        public string ShowFilePicker(string initialDirectory)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message");
        }

    }
}
