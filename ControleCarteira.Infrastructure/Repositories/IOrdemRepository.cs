using ControleCarteira.Domain;
using ControleCarteira.Infrastructure.Repositories.Base;


namespace ControleCarteira.Infrastructure.Repositories;
public interface IOrdemRepository : IBaseRepository<Ordem, int>
{
    //void Cadastrar(List<Ordem> ordens);
}
