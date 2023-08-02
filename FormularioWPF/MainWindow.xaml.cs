using System.Collections.Generic;
using System.Data;
using System;
using System.Windows;

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
                // Chame o método da classe DAL para obter os dados da tabela "tbl_endereco".
                DataTable dtEnderecos = obj.RetornaTabela("SELECT * FROM tbl_endereco");

                // Crie uma lista de objetos "Endereco" para armazenar os dados convertidos.
                List<Endereco> enderecos = new List<Endereco>();

                // Percorra os dados da tabela e converta para objetos "Endereco".
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
                        // Faça a conversão para outras propriedades, se houver.
                    };

                    // Adicione o objeto "Endereco" à lista.
                    enderecos.Add(endereco);
                }

                // Preencher o DataGrid com a lista de objetos "Endereco".
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
                // Chame o método ExcluirEndereco para excluir o endereço.
                obj.ExcluirEndereco(id);

                // Atualizar o DataGrid após a exclusão.
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

            // Se estiver editando um registro existente, forneça o ID do endereço.
            // Caso contrário, passe null para que o método saiba que é uma inserção.
            int? id = null; // Substitua por um valor válido do ID, se necessário.

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

                // Chame o método ComandoSql para executar a gravação.
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

                // Realizar a exclusão do endereço com o ID obtido.
                Excluir(idEnderecoSelecionado);
            }
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            Gravar();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            // Verificar se algum item foi selecionado no DataGrid.
            if (dgEndereco.SelectedItem != null && dgEndereco.SelectedItem is Endereco enderecoSelecionado)
            {
                int idEnderecoSelecionado = enderecoSelecionado.Id;

                // Realizar a exclusão do endereço com o ID obtido.
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
    }
}
