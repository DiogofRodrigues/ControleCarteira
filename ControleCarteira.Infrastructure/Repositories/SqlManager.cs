using ControleCarteira.Infrastructure.Enums;

namespace ControleCarteira.Infrastructure.Repositories;
public class SqlManager
{
    public static string GetSql(TSqlQuery tsql)
    {
        string querySql = "";

        switch (tsql)
        {
            case TSqlQuery.CADASTRAR_ORDER:
                querySql = "INSERT INTO ORDEM (Ticker,Tipo,QUANTIDADE,precoUnitario,DataOrdem) VALUES (@Ticker,@Tipo,@QUANTIDADE,@precoUnitario,@DataOrdem)";
                break;

            case TSqlQuery.ATUALIZA_PAPEL:
                querySql = "UPDATE Carteira SET Quantidade = @Quantidade , PrecoMedio = @PrecoMedio , ValorTotal = @ValorTotal WHERE Ticker = @Ticker";
                break;
            case TSqlQuery.INSERE_PAPEL:
                querySql = "INSERT INTO Carteira (Ticker,Quantidade,PrecoMedio,ValorTotal) VALUES (@Ticker,@Quantidade,@PrecoMedio,@ValorTotal)";
                break;
            case TSqlQuery.CONSULTA_CARTEIRA:
                querySql = "SELECT * FROM Carteira WHERE Ticker = @Ticker";
                break;
            case TSqlQuery.DELETA_PAPEL:
                querySql = "DELETE FROM Carteira WHERE Ticker = @Ticker";
                break;
        }

        return querySql;
    }
}

