using ControleCarteira.Domain;
using ControleCarteira.Infrastructure.Enums;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using ControleCarteira.Infrastructure.Repositories;
using System.Runtime.Intrinsics.Arm;

namespace GameStoreFase4.Infrastructure.Repositories;
public class OrdemRepository : IOrdemRepository
{
    private readonly string _connectionString;

    public OrdemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("CarteiraDB");
    }
    public void Cadastrar(Ordem ordem)
    {
        SqlConnection connection = new SqlConnection(_connectionString);
        

        try
        {
            string query = SqlManager.GetSql(TSqlQuery.CADASTRAR_ORDER);


            connection.Open();

            AtualizaCarteira(ordem);


            int returnedId = connection.ExecuteScalar<int>(query, ordem);


        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (connection.State.Equals(ConnectionState.Open))
                connection.Close();

            
        }


    }

    public void AtualizaCarteira(Ordem ordem)
    {

        SqlConnection connection = new SqlConnection(_connectionString);
        string query;


        bool newTicker = false;

        Console.WriteLine("Diogo");

        Carteira carteira = PesquisarPeloId(ordem.Ticker);

        if (carteira == null) { newTicker = true; }

        carteira = AtualizaValoresCarteira(carteira,ordem);

        if (newTicker)
        {
            query = SqlManager.GetSql(TSqlQuery.INSERE_PAPEL);
        }
        else if (carteira.Quantidade == 0) 
        {
            query = SqlManager.GetSql(TSqlQuery.DELETA_PAPEL);
        }
        else
        {
            query = SqlManager.GetSql(TSqlQuery.ATUALIZA_PAPEL);

        }


        Console.WriteLine(query.ToString());
        try
        {

            if (!connection.State.Equals(ConnectionState.Open))
                connection.Open();

            connection.Execute(query, carteira);



        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (connection.State.Equals(ConnectionState.Open))
                connection.Close();
        }


    }

    public Carteira AtualizaValoresCarteira(Carteira carteira , Ordem ordem)
    {
        
        if (carteira == null)
        {
            carteira = new Carteira
            {
                Ticker = ordem.Ticker,
                Quantidade = ordem.Quantidade,
                PrecoMedio = ordem.PrecoUnitario,
                ValorTotal = ordem.PrecoUnitario * ordem.Quantidade,
            };
        }
        else
        {
            if (ordem.Tipo.ToUpper() == "C")
            {
                carteira.Quantidade += ordem.Quantidade;
                carteira.ValorTotal += ordem.PrecoUnitario * ordem.Quantidade;
                carteira.PrecoMedio = carteira.ValorTotal / carteira.Quantidade;

            }
            else
            {
                carteira.Quantidade -= ordem.Quantidade;
                carteira.ValorTotal -= ordem.PrecoUnitario * ordem.Quantidade;

            }

        }

        return carteira;

    }

    public Carteira PesquisarPeloId(string ticker)
    {
        SqlConnection connection = new SqlConnection(_connectionString);

        try
        {
            if (!connection.State.Equals(ConnectionState.Open))
                connection.Open();
            string query = SqlManager.GetSql(TSqlQuery.CONSULTA_CARTEIRA);
            Carteira carteira = connection.QueryFirstOrDefault<Carteira>(query, new { Ticker = ticker });
            return carteira;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (connection.State.Equals(ConnectionState.Open))
                connection.Close();
        }
    }

}
