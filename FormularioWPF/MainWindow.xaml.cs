using FormularioWPF.Models;
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FormularioWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConnDbContext _dbconn;

        public MainWindow()
        {
            InitializeComponent();
            _dbconn = new ConnDbContext();
            dgEndereco.ItemsSource = _dbconn.clientes.ToList();
        }

        private void Gravar()
        {
            if (!string.IsNullOrEmpty(txtNome.Text) &&
                !string.IsNullOrEmpty(txtCidade.Text) &&
                !string.IsNullOrEmpty(txtCep.Text) && !string.IsNullOrEmpty(txtRua.Text))
            {

                var campos = new Endereco
                {
                    Nome = txtNome.Text,
                    Cidade = txtCidade.Text,
                    CEP = txtCep.Text,
                    Rua = txtRua.Text,
                };
                _dbconn.clientes.Add(campos);
                _dbconn.SaveChanges();
                dgEndereco.ItemsSource = _dbconn.clientes.ToList();
            }
            else
            {
                MessageBox.Show("Abasteça os campos corretamente");
            }
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            Gravar();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (dgEndereco.SelectedItem != null && dgEndereco.SelectedItem is Endereco enderecoSelecionado)
            {
                int idEnderecoSelecionado = (int)enderecoSelecionado.Id;

                // Buscar a entidade completa no banco de dados
                var enderecoParaRemover = _dbconn.clientes.Find(idEnderecoSelecionado);

                if (enderecoParaRemover != null)
                {
                    // Remover a entidade do contexto
                    _dbconn.Remove(enderecoParaRemover);
                    _dbconn.SaveChanges();

                    // Atualizar a fonte de dados do DataGrid
                    dgEndereco.ItemsSource = _dbconn.clientes.ToList();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] estados = {
        "AC", "AL", "AP", "AM", "BA", "CE", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE",
        "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP",
        "SE", "TO", "DF"
    };

            // Adicionar os estados ao ComboBox.
            foreach (string estado in estados)
            {
                cmbEstado.Items.Add(estado);
            }
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Planilha1");

                DataTable dt = new DataTable();
                foreach (var column in dgEndereco.Columns)
                {
                    dt.Columns.Add(column.Header.ToString());
                }

                foreach (var item in dgEndereco.Items)
                {
                    DataRow row = dt.NewRow();
                    foreach (var column in dgEndereco.Columns)
                    {
                        var binding = (column as DataGridTextColumn).Binding as System.Windows.Data.Binding;
                        var col = binding.Path.Path;
                        row[col] = GetProperty(item, col);
                    }
                    dt.Rows.Add(row);
                }

                worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                string caminho_do_arquivo = "Planilha1.xlsx";
                File.WriteAllBytes(caminho_do_arquivo, package.GetAsByteArray());

                string excelPath = FindExcelExecutablePath();

                if (!string.IsNullOrEmpty(excelPath))
                {
                    Process.Start(excelPath, caminho_do_arquivo);
                }
                else
                {
                    MessageBox.Show("O Excel não foi encontrado no sistema.");
                }

                MessageBox.Show("Dados exportados para o Excel com sucesso!");
            }
        }


        private string FindExcelExecutablePath()
        {
            // Tentar encontrar o caminho do executável do Excel no registro do sistema
            string excelPath = null;

            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\excel.exe"))
            {
                if (key != null)
                {
                    object value = key.GetValue("");
                    if (value != null)
                    {
                        excelPath = value.ToString();
                    }
                }
            }

            return excelPath;
        }


        private object GetProperty(object obj, string propName)
        {
            return obj.GetType().GetProperty(propName).GetValue(obj, null);
        }
    }
}
