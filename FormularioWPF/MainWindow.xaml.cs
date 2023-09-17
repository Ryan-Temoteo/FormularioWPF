using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FormularioWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PreencheDataGrid();
        }

        DAL obj = new DAL();

        public class Endereco
        {
            public int Id { get; set; }
            public string Rua { get; set; }
            public string Cidade { get; set; }
            public string Estado { get; set; }
            public string CEP { get; set; }
            public string Nome { get; set; }
        }


        private void PreencheDataGrid()
        {
            try
            {
                DataTable dtEnderecos = obj.RetornaTabela("SELECT * FROM tbl_endereco");

                List<Endereco> enderecos = new List<Endereco>();

                foreach (DataRow row in dtEnderecos.Rows)
                {
                    Endereco endereco = new Endereco
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Rua = row["rua"].ToString(),
                        Cidade = row["cidade"].ToString(),
                        Estado = row["estado"].ToString(),
                        CEP = row["cep"].ToString(),
                        Nome = row["nome"].ToString()
                    };

                    enderecos.Add(endereco);
                }

                dgEndereco.ItemsSource = enderecos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message);
            }
        }

        private void Excluir(int id)
        {
            

            try
            {
                obj.ExcluirEndereco(id);

                PreencheDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir o endereço: " + ex.Message);
            }
        }

        private void Gravar()
        {
            string nome = txtNome.Text;
            string rua = txtRua.Text;
            string cidade = txtCidade.Text;
            string estado = cmbEstado.SelectedItem.ToString();
            string cep = txtCep.Text;

            
            int? id = null;

            DAL dal = new DAL();

            try
            {
                string query;
                Dictionary<string, object> parametros = new Dictionary<string, object>
                {
                    { "@Nome", nome },
                    { "@Rua", rua },
                    { "@Cidade", cidade },
                    { "@Estado", estado },
                    { "@CEP", cep }
                };

                // Verificar se é uma inserção ou atualização.
                if (id == null)
                {
                    query = "INSERT INTO tbl_endereco (Nome, Rua, Cidade, Estado, CEP) VALUES (@Nome, @Rua, @Cidade, @Estado, @CEP)";
                }
                else
                {
                    query = "UPDATE tbl_endereco SET Nome = @Nome, Rua = @Rua, Cidade = @Cidade, Estado = @Estado, CEP = @CEP WHERE Id = @Id";
                    parametros.Add("@Id", id);
                }

                obj.ComandoSql(query, parametros);

                // Atualizar o DataGrid após a gravação.
                PreencheDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gravar o endereço: " + ex.Message);
            }
        }

        private void dgEndereco_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgEndereco.SelectedItem != null && dgEndereco.SelectedItem is Endereco enderecoSelecionado)
            {
                int idEnderecoSelecionado = enderecoSelecionado.Id;

                Excluir(idEnderecoSelecionado);
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
                int idEnderecoSelecionado = enderecoSelecionado.Id;

                Excluir(idEnderecoSelecionado);
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
