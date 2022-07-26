using Dapper;
using Dataplace.Imersao.Core.Application.Orcamentos.ViewModels;
using Dataplace.Imersao.Core.Domain.Orcamentos.Repositories;
using dpLibrary05;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dataplace.Imersao.Core.Application.Orcamentos.Queries
{
    public class OrcamentoItemQueryHandler :
        IRequestHandler<ObterOrcamentoItemQuery, OrcamentoItemViewModel>
    {
        #region fields
        private readonly IDataAccess _dataAccess;

        public OrcamentoItemQueryHandler(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        #endregion


        public async Task<OrcamentoItemViewModel> Handle(ObterOrcamentoItemQuery query, CancellationToken cancellationToken)
        {
            var sql = @"
            SET TRANSACTION ISOLATION LEVEL SNAPSHOT;
                SELECT orcamentoite.cdempresa,
                orcamentoitem.cdfilial,
                orcamentoitem.numorcamento,
                orcamentoitem.seq,
                orcamentoitem.tpregistro,
                orcamentoitem.cdproduto,
                CASE WHEN produto.dsvenda IS NULL THEN servico.descricao ELSE produto.dsvenda END as dsproduto,
                orcamentoitem.qtdproduto AS quantidade,
                orcamentoitem.vlvenda AS precotabela,
                orcamentoitem.percaltpreco,
                orcamentoitem.vlcalculado AS precovenda,
                orcamentoitem.vlcalculado * orcamentoitem.qtdproduto AS total,
                orcamentoitem.stitem AS status
            FROM orcamento
            LEFT JOIN produto ON produto.cdproduto = orcamentoitem.cdproduto AND produto.tpproduto = orcamentoitem.tpregistro
            LEFT JOIN servico on servico.cdservico = orcamentoitem.cdservico AND orcamentoitem.tpregistro = '5'
            /**where**/";
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate(sql);

            builder.Where("orcamentoitem.seq = @Seq", new { query.Seq });
            var cmd = new CommandDefinition(selector.RawSql, selector.Parameters, flags: CommandFlags.NoCache);
            return _dataAccess.Connection.QueryFirstOrDefault<OrcamentoItemViewModel>(cmd);

        }
    }
}
